/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public interface IExposureConfigurationRepository
    {
        public DateTime GetExposureConfigurationDownloadedDateTime();

        public bool IsDiagnosisKeysDataMappingConfigurationUpdated();
        public void SetIsDiagnosisKeysDataMappingConfigurationUpdated(bool isUpdated);
        public void SetDiagnosisKeysDataMappingAppliedDateTime(DateTime dateTime);
        public DateTime GetDiagnosisKeysDataMappingConfigurationAppliedDateTime();

        public Task<ExposureConfiguration> GetExposureConfigurationAsync();
        public Task RemoveExposureConfigurationAsync();
    }

    public class ExposureConfigurationRepository : IExposureConfigurationRepository
    {
        private const int TIMEOUT_SECONDS = 10;

        private readonly IHttpClientService _httpClientService;
        private readonly ILocalPathService _localPathService;
        private readonly IPreferencesService _preferencesService;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly ILoggerService _loggerService;

        private readonly string _configDir;

        private string _currentExposureConfigurationPath;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public ExposureConfigurationRepository(
            IHttpClientService httpClientService,
            ILocalPathService localPathService,
            IPreferencesService preferencesService,
            IServerConfigurationRepository serverConfigurationRepository,
            IDateTimeUtility dateTimeUtility,
            ILoggerService loggerService
            )
        {
            _httpClientService = httpClientService;
            _localPathService = localPathService;
            _preferencesService = preferencesService;
            _serverConfigurationRepository = serverConfigurationRepository;
            _dateTimeUtility = dateTimeUtility;
            _loggerService = loggerService;

            _configDir = PrepareConfigDir();
            _currentExposureConfigurationPath = localPathService.CurrentExposureConfigurationPath;
        }

        private string PrepareConfigDir()
        {
            var configDir = _localPathService.ExposureConfigurationDirPath;
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
            return configDir;
        }

        public async Task<ExposureConfiguration> GetExposureConfigurationAsync()
        {
            await _semaphore.WaitAsync();

            try
            {
                return await GetExposureConfigurationInternalAsync();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<ExposureConfiguration> GetExposureConfigurationInternalAsync()
        {
            _loggerService.StartMethod();

            ExposureConfiguration currentExposureConfiguration = null;

            if (File.Exists(_currentExposureConfigurationPath))
            {
                _loggerService.Debug("ExposureConfiguration file is found.");

                try
                {
                    string exposureConfigurationAsJson = await LoadAsync(_currentExposureConfigurationPath);

                    currentExposureConfiguration = JsonConvert.DeserializeObject<ExposureConfiguration>(exposureConfigurationAsJson);

                    if (!IsDownloadedExposureConfigurationOutdated(AppConstants.ExposureConfigurationFileDownloadCacheRetentionDays))
                    {
                        _loggerService.EndMethod();
                        return currentExposureConfiguration;
                    }
                    else
                    {
                        _loggerService.Info($"ExposureConfiguration is found but the file is outdated.");
                    }
                }
                catch (IOException exception)
                {
                    _loggerService.Exception("IOException. ExposureConfiguration file has been deleted.", exception);
                    RemoveExposureConfigurationInternal();
                }
                catch (JsonException exception)
                {
                    _loggerService.Exception("JsonException. ExposureConfiguration file has been deleted.", exception);
                    RemoveExposureConfigurationInternal();
                }
            }

            if (currentExposureConfiguration is null)
            {
                currentExposureConfiguration = new ExposureConfiguration();
            }

            await _serverConfigurationRepository.LoadAsync();
            string url = _serverConfigurationRepository.ExposureConfigurationUrl;

            ExposureConfiguration newExposureConfiguration = null;

            using (var client = _httpClientService.Create())
            {
                client.Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);

                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string exposureConfigurationAsJson = await response.Content.ReadAsStringAsync();
                    _loggerService.Debug(exposureConfigurationAsJson);

                    try
                    {
                        newExposureConfiguration = JsonConvert.DeserializeObject<ExposureConfiguration>(exposureConfigurationAsJson);
                        SetExposureConfigurationDownloadedDateTime(_dateTimeUtility.UtcNow);
                    }
                    catch (JsonException exception)
                    {
                        _loggerService.Exception("JsonException.", exception);
                    }

                }
                else
                {
                    _loggerService.Warning($"Download ExposureConfiguration failed from {url}");
                }
            }

            if (newExposureConfiguration is null)
            {
                _loggerService.EndMethod();
                return currentExposureConfiguration;
            }

            if (IsUpdatedDiagnosisKeysDataMapping(currentExposureConfiguration, newExposureConfiguration))
            {
                if (IsExposureConfigurationOutdated(AppConstants.MinimumDiagnosisKeysDataMappingApplyIntervalDays))
                {
                    currentExposureConfiguration = newExposureConfiguration;
                    SetIsDiagnosisKeysDataMappingConfigurationUpdated(true);
                }
                else
                {
                    _loggerService.Info($"DiagnosisKeysDataMappingConfig has been changed but not updated, because current configuration is updated in {AppConstants.MinimumDiagnosisKeysDataMappingApplyIntervalDays} days.");
                }
            }
            else
            {
                currentExposureConfiguration = newExposureConfiguration;
            }

            string tmpFilePath = Path.Combine(_configDir, Guid.NewGuid().ToString());

            try
            {
                await SaveAsync(
                    JsonConvert.SerializeObject(currentExposureConfiguration, Formatting.Indented),
                    tmpFilePath
                    );
                Swap(tmpFilePath, _currentExposureConfigurationPath);

                return currentExposureConfiguration;
            }
            finally
            {
                File.Delete(tmpFilePath);

                _loggerService.EndMethod();

            }
        }

        private void Swap(string sourcePath, string destPath)
        {
            string tmpFilePath = Path.Combine(_configDir, Guid.NewGuid().ToString());

            if (File.Exists(destPath))
            {
                // Backup
                File.Move(destPath, tmpFilePath);
            }

            try
            {
                File.Move(sourcePath, destPath);
            }
            catch (IOException exception)
            {
                _loggerService.Exception("IOException", exception);

                // Restore
                if (File.Exists(tmpFilePath))
                {
                    File.Move(tmpFilePath, destPath);
                }
            }
            finally
            {
                File.Delete(tmpFilePath);
            }
        }

        private bool IsUpdatedDiagnosisKeysDataMapping(
            ExposureConfiguration exposureConfiguration1,
            ExposureConfiguration exposureConfiguration2
            )
        {
            return
                (exposureConfiguration1.GoogleDiagnosisKeysDataMappingConfig
                    != exposureConfiguration2.GoogleDiagnosisKeysDataMappingConfig)
                ||
                (exposureConfiguration1.AppleExposureConfigV2.InfectiousnessForDaysSinceOnsetOfSymptoms
                    != exposureConfiguration2.AppleExposureConfigV2.InfectiousnessForDaysSinceOnsetOfSymptoms);

        }

        private bool IsExposureConfigurationOutdated(int retensionDays)
        {
            DateTime appliedDate = GetDiagnosisKeysDataMappingConfigurationAppliedDateTime();
            return (_dateTimeUtility.UtcNow - appliedDate) > TimeSpan.FromDays(retensionDays);
        }

        private bool IsDownloadedExposureConfigurationOutdated(int retensionDays)
        {
            DateTime downloadedDate = GetExposureConfigurationDownloadedDateTime();
            return (_dateTimeUtility.UtcNow - downloadedDate) > TimeSpan.FromDays(retensionDays);
        }

        private async Task<string> LoadAsync(string path)
        {
            using var reader = File.OpenText(path);
            return await reader.ReadToEndAsync();
        }

        private async Task SaveAsync(string exposureConfigurationAsJson, string outPath)
            => await File.WriteAllTextAsync(outPath, exposureConfigurationAsJson);

        public async Task RemoveExposureConfigurationAsync()
        {
            await _semaphore.WaitAsync();

            try
            {
                RemoveExposureConfigurationInternal();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void RemoveExposureConfigurationInternal()
        {
            if (!File.Exists(_currentExposureConfigurationPath))
            {
                _loggerService.Debug("No ExposureConfiguration file found.");
                return;
            }

            File.Delete(_currentExposureConfigurationPath);
        }

        public void SetIsDiagnosisKeysDataMappingConfigurationUpdated(bool isUpdated)
            => _preferencesService.SetValue(PreferenceKey.IsExposureConfigurationUpdated, isUpdated);

        public bool IsDiagnosisKeysDataMappingConfigurationUpdated()
            => _preferencesService.GetValue(PreferenceKey.IsExposureConfigurationUpdated, true);

        private void SetExposureConfigurationDownloadedDateTime(DateTime dateTime)
        {
            _loggerService.StartMethod();
            _preferencesService.SetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, dateTime.ToUnixEpoch());
            _loggerService.EndMethod();
        }

        public DateTime GetExposureConfigurationDownloadedDateTime()
        {
            _loggerService.StartMethod();
            try
            {
                long epoch = _preferencesService.GetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, 0L);
                return DateTime.UnixEpoch.AddSeconds(epoch);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public void SetDiagnosisKeysDataMappingAppliedDateTime(DateTime dateTime)
        {
            _loggerService.StartMethod();
            _preferencesService.SetValue(PreferenceKey.ExposureConfigurationAppliedEpoch, dateTime.ToUnixEpoch());
            _loggerService.EndMethod();
        }

        public DateTime GetDiagnosisKeysDataMappingConfigurationAppliedDateTime()
        {
            _loggerService.StartMethod();
            try
            {
                long epoch = _preferencesService.GetValue(PreferenceKey.ExposureConfigurationAppliedEpoch, 0L);
                return DateTime.UnixEpoch.AddSeconds(epoch);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}
