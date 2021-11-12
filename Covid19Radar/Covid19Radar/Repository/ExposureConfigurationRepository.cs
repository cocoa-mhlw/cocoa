/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Covid19Radar.Repository
{
    public interface IExposureConfigurationRepository
    {
        public DateTime? GetExposureConfigurationDownloadedDateTime();

        public bool IsDiagnosisKeysDataMappingConfigurationUpdated();
        public void SetDiagnosisKeysDataMappingConfigurationUpdated(bool updated);
        public void SetDiagnosisKeysDataMappingAppliedDateTime(DateTime dateTime);
        public DateTime? GetDiagnosisKeysDataMappingConfigurationAppliedDateTime();

        public Task<ExposureConfiguration> GetExposureConfigurationAsync();
        public void RemoveExposureConfiguration();
    }

    public class ExposureConfigurationRepository : IExposureConfigurationRepository
    {
        private const string CONFIG_DIR = "exposure_configuration";
        private const string CURRENT_CONFIG_FILENAME = "current.json";

        private const int EXPOSURE_CONFIGURATION_FILE_RETENTION_DAYS = 2;
        private const int EXPOSURE_CONFIGURATION_RETENTION_DAYS = 7 + 1;

        private readonly HttpClient _client;
        private readonly IPreferencesService _preferencesService;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly ILoggerService _loggerService;

        private readonly string _configDir;

        private string _currentExposureConfigurationPath;

        public ExposureConfigurationRepository(
            IHttpClientService httpClientService,
            IPreferencesService preferencesService,
            IServerConfigurationRepository serverConfigurationRepository,
            ILoggerService loggerService
            )
        {
            _client = httpClientService.Create();
            _preferencesService = preferencesService;
            _serverConfigurationRepository = serverConfigurationRepository;
            _loggerService = loggerService;

            _configDir = PrepareConfigDir();

            _currentExposureConfigurationPath = Path.Combine(_configDir, CURRENT_CONFIG_FILENAME);
        }

        private string PrepareConfigDir()
        {
            var dir = FileSystem.AppDataDirectory;

            var configDir = Path.Combine(dir, CONFIG_DIR);
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
            return configDir;
        }

        public async Task<ExposureConfiguration> GetExposureConfigurationAsync()
        {
            _loggerService.StartMethod();

            ExposureConfiguration currentExposureConfiguration = null;

            if (File.Exists(_currentExposureConfigurationPath))
            {
                _loggerService.Debug("ExposureConfiguration file is found.");

                string exposureConfigurationAsJson = await LoadAsync(_currentExposureConfigurationPath);

                try
                {
                    currentExposureConfiguration = JsonConvert.DeserializeObject<ExposureConfiguration>(exposureConfigurationAsJson);

                    if (!IsDownloadedExposureConfigurationOutdated(EXPOSURE_CONFIGURATION_FILE_RETENTION_DAYS))
                    {
                        return currentExposureConfiguration;
                    }
                    else
                    {
                        _loggerService.Info($"ExposureConfiguration is found but the file is outdated.");
                    }
                }
                catch (JsonException exception)
                {
                    _loggerService.Exception("JsonException. ExposureConfiguration file has been deleted.", exception);
                    RemoveExposureConfiguration(_currentExposureConfigurationPath);
                }
            }

            if (currentExposureConfiguration is null)
            {
                currentExposureConfiguration = new ExposureConfiguration();
            }

            await _serverConfigurationRepository.LoadAsync();
            string url = _serverConfigurationRepository.ExposureConfigurationUrl;

            ExposureConfiguration newExposureConfiguration = null;

            var response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string exposureConfigurationAsJson = await response.Content.ReadAsStringAsync();
                _loggerService.Debug(exposureConfigurationAsJson);

                try
                {
                    newExposureConfiguration = JsonConvert.DeserializeObject<ExposureConfiguration>(exposureConfigurationAsJson);
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

            if (newExposureConfiguration is null)
            {
                return currentExposureConfiguration;
            }

            if (IsUpdatedDiagnosisKeysDataMapping(currentExposureConfiguration, newExposureConfiguration))
            {
                if (IsExposureConfigurationOutdated(EXPOSURE_CONFIGURATION_RETENTION_DAYS))
                {
                    currentExposureConfiguration = newExposureConfiguration;
                    SetDiagnosisKeysDataMappingConfigurationUpdated(true);
                }
                else
                {
                    _loggerService.Info($"DiagnosisKeysDataMappingConfig has been changed but not updated, because current configuration is updated in {EXPOSURE_CONFIGURATION_RETENTION_DAYS} days.");
                }
            }
            else
            {
                currentExposureConfiguration = newExposureConfiguration;
            }

            await SaveAsync(
                JsonConvert.SerializeObject(currentExposureConfiguration, Formatting.Indented),
                _currentExposureConfigurationPath);
            SetExposureConfigurationDownloadedDateTime(DateTime.UtcNow);

            _loggerService.EndMethod();

            return currentExposureConfiguration;
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
            DateTime? appliedDate = GetDiagnosisKeysDataMappingConfigurationAppliedDateTime();
            if (appliedDate is null)
            {
                return true;
            }
            return (DateTime.UtcNow - appliedDate) > TimeSpan.FromDays(retensionDays);
        }

        private bool IsDownloadedExposureConfigurationOutdated(int retensionDays)
        {
            DateTime? downloadedDate = GetExposureConfigurationDownloadedDateTime();
            if (downloadedDate is null)
            {
                return true;
            }
            return (DateTime.UtcNow - downloadedDate) > TimeSpan.FromDays(retensionDays);
        }

        private async Task<string> LoadAsync(string path)
        {
            using var reader = File.OpenText(path);
            return await reader.ReadToEndAsync();
        }

        private async Task SaveAsync(string exposureConfigurationAsJson, string outPath)
            => await File.WriteAllTextAsync(outPath, exposureConfigurationAsJson);

        public void RemoveExposureConfiguration()
        {
            RemoveExposureConfiguration(_currentExposureConfigurationPath);
        }

        private void RemoveExposureConfiguration(string path)
        {
            if (!File.Exists(path))
            {
                _loggerService.Debug("No ExposureConfiguration file found.");
                return;
            }

            File.Delete(path);
        }

        public void SetDiagnosisKeysDataMappingConfigurationUpdated(bool updated)
            => _preferencesService.SetValue(PreferenceKey.IsExposureConfigurationUpdated, updated);

        public bool IsDiagnosisKeysDataMappingConfigurationUpdated()
            => _preferencesService.GetValue(PreferenceKey.IsExposureConfigurationUpdated, true);

        private void SetExposureConfigurationDownloadedDateTime(DateTime dateTime)
        {
            _loggerService.StartMethod();
            _preferencesService.SetValue(PreferenceKey.ExposureConfigurationDownloadedEpoch, dateTime.ToUnixEpoch());
            _loggerService.EndMethod();
        }

        public DateTime? GetExposureConfigurationDownloadedDateTime()
        {
            _loggerService.StartMethod();
            try
            {
                if (!_preferencesService.ContainsKey(PreferenceKey.ExposureConfigurationDownloadedEpoch))
                {
                    return null;
                }

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

        public DateTime? GetDiagnosisKeysDataMappingConfigurationAppliedDateTime()
        {
            _loggerService.StartMethod();
            try
            {
                if (!_preferencesService.ContainsKey(PreferenceKey.ExposureConfigurationAppliedEpoch))
                {
                    return null;
                }

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
