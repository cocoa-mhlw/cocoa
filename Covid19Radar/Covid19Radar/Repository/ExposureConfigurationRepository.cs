/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

        private readonly ILocalPathService _localPathService;
        private readonly IPreferencesService _preferencesService;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly ILoggerService _loggerService;

        private readonly HttpClient _httpClient;

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
            _localPathService = localPathService;
            _preferencesService = preferencesService;
            _serverConfigurationRepository = serverConfigurationRepository;
            _dateTimeUtility = dateTimeUtility;
            _loggerService = loggerService;

            _httpClient = httpClientService.Create();
            _httpClient.Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);

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

                    _loggerService.Info("Cached:" + exposureConfigurationAsJson);

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

            // Cache not exist (first time. probably...)
            if (currentExposureConfiguration is null)
            {
                currentExposureConfiguration = CreateDefaultConfiguration();
                SetExposureConfigurationDownloadedDateTime(_dateTimeUtility.UtcNow);
                SetIsDiagnosisKeysDataMappingConfigurationUpdated(true);
            }

            await _serverConfigurationRepository.LoadAsync();
            string url = _serverConfigurationRepository.ExposureConfigurationUrl;

            ExposureConfiguration newExposureConfiguration = null;

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string exposureConfigurationAsJson = await response.Content.ReadAsStringAsync();

                try
                {
                    newExposureConfiguration = JsonConvert.DeserializeObject<ExposureConfiguration>(exposureConfigurationAsJson);

                    _loggerService.Info("Downloaded:" + exposureConfigurationAsJson);

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

        private static ExposureConfiguration CreateDefaultConfiguration()
        {
            return new ExposureConfiguration()
            {
                #region Exposure Window mode(ENv2) Configuration
                GoogleDailySummariesConfig = new DailySummariesConfig()
                {
                    AttenuationBucketThresholdDb = new List<int>() {
                        46,
                        60,
                        65
                    },
                    AttenuationBucketWeights = new List<double>() {
                        1.0,
                        2.5,
                        1.3,
                        0.01
                    },
                    DaysSinceExposureThreshold = 0,
                    InfectiousnessWeights = new Dictionary<Infectiousness, double>()
                    {
                        { Infectiousness.High, 1.0 },
                        { Infectiousness.Standard, 1.0 },
                    },
                    MinimumWindowScore = 0.0,
                    ReportTypeWeights = new Dictionary<ReportType, double>()
                    {
                        { ReportType.ConfirmedTest, 1.0 },
                        { ReportType.ConfirmedClinicalDiagnosis, 1.0 },
                        { ReportType.SelfReport, 1.0 },
                        { ReportType.Recursive, 1.0 },
                    }
                },
                GoogleDiagnosisKeysDataMappingConfig = new ExposureConfiguration.GoogleDiagnosisKeysDataMappingConfiguration()
                {
                    InfectiousnessWhenDaysSinceOnsetMissing = Infectiousness.High,
                    ReportTypeWhenMissing = ReportType.ConfirmedTest,
                    InfectiousnessForDaysSinceOnsetOfSymptoms = new Dictionary<int, Infectiousness>()
                    {
                        { -14, Infectiousness.None },
                        { -13, Infectiousness.None },
                        { -12, Infectiousness.None },
                        { -11, Infectiousness.None },
                        { -10, Infectiousness.None },
                        { -9, Infectiousness.None },
                        { -8, Infectiousness.None },
                        { -7, Infectiousness.None },
                        { -6, Infectiousness.None },
                        { -5, Infectiousness.None },
                        { -4, Infectiousness.None },
                        { -3, Infectiousness.High },
                        { -2, Infectiousness.High },
                        { -1, Infectiousness.High },
                        { 0, Infectiousness.High },
                        { +1, Infectiousness.High },
                        { +2, Infectiousness.High },
                        { +3, Infectiousness.High },
                        { +4, Infectiousness.High },
                        { +5, Infectiousness.High },
                        { +6, Infectiousness.High },
                        { +7, Infectiousness.High },
                        { +8, Infectiousness.High },
                        { +9, Infectiousness.High },
                        { +10, Infectiousness.High },
                        { +11, Infectiousness.None },
                        { +12, Infectiousness.None },
                        { +13, Infectiousness.None },
                        { +14, Infectiousness.None },
                    },
                },
                AppleExposureConfigV2 = new ExposureConfiguration.AppleExposureConfigurationV2()
                {
                    InfectiousnessWhenDaysSinceOnsetMissing = Infectiousness.High,
                    ReportTypeNoneMap = ReportType.ConfirmedTest,
                    AttenuationDurationThresholds = new int[] {
                        46,
                        60,
                        65
                    },
                    ImmediateDurationWeight = 100.0,
                    NearDurationWeight = 250.0,
                    MediumDurationWeight = 130.0,
                    OtherDurationWeight = 1.0,
                    DaysSinceLastExposureThreshold = 0,
                    InfectiousnessForDaysSinceOnsetOfSymptoms = new Dictionary<long, Infectiousness>()
                    {
                        { -14, Infectiousness.None },
                        { -13, Infectiousness.None },
                        { -12, Infectiousness.None },
                        { -11, Infectiousness.None },
                        { -10, Infectiousness.None },
                        { -9, Infectiousness.None },
                        { -8, Infectiousness.None },
                        { -7, Infectiousness.None },
                        { -6, Infectiousness.None },
                        { -5, Infectiousness.None },
                        { -4, Infectiousness.None },
                        { -3, Infectiousness.High },
                        { -2, Infectiousness.High },
                        { -1, Infectiousness.High },
                        { 0, Infectiousness.High },
                        { +1, Infectiousness.High },
                        { +2, Infectiousness.High },
                        { +3, Infectiousness.High },
                        { +4, Infectiousness.High },
                        { +5, Infectiousness.High },
                        { +6, Infectiousness.High },
                        { +7, Infectiousness.High },
                        { +8, Infectiousness.High },
                        { +9, Infectiousness.High },
                        { +10, Infectiousness.High },
                        { +11, Infectiousness.None },
                        { +12, Infectiousness.None },
                        { +13, Infectiousness.None },
                        { +14, Infectiousness.None },
                    },
                    InfectiousnessHighWeight = 100.0,
                    InfectiousnessStandardWeight = 100.0,
                    ReportTypeConfirmedClinicalDiagnosisWeight = 100.0,
                    ReportTypeConfirmedTestWeight = 100.0,
                    ReportTypeRecursiveWeight = 100.0,
                    ReportTypeSelfReportedWeight = 100.0,
                },
                #endregion
                #region Legacy-V1 Configuration
                GoogleExposureConfig = new ExposureConfiguration.GoogleExposureConfiguration()
                {
                    AttenuationScores = new int[]
                    {
                        1,
                        2,
                        3,
                        4,
                        5,
                        6,
                        7,
                        8,
                    },
                    AttenuationWeight = 50,
                    DaysSinceLastExposureScores = new int[]
                    {
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                    },
                    DaysSinceLastExposureWeight = 50,
                    DurationAtAttenuationThresholds = new int[]
                    {
                        50,
                        70,
                    },
                    DurationScores = new int[]
                    {
                        0,
                        0,
                        0,
                        0,
                        1,
                        1,
                        1,
                        1,
                    },
                    DurationWeight = 50,
                    MinimumRiskScore = 21,
                    TransmissionRiskScores = new int[]
                    {
                        7,
                        7,
                        7,
                        7,
                        7,
                        7,
                        7,
                        7,
                    },
                    TransmissionRiskWeight = 50,
                },
                AppleExposureConfigV1 = new ExposureConfiguration.AppleExposureConfigurationV1()
                {
                    AttenuationLevelValues = new int[]
                    {
                        1,
                        2,
                        3,
                        4,
                        5,
                        6,
                        7,
                        8,
                    },
                    DaysSinceLastExposureLevelValues = new int[]
                    {
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                        1,
                    },
                    DurationLevelValues = new int[]
                    {
                        0,
                        0,
                        0,
                        0,
                        1,
                        1,
                        1,
                        1,
                    },
                    TransmissionRiskLevelValues = new int[]
                    {
                        0,
                        0,
                        7,
                        7,
                        7,
                        7,
                        7,
                        7,
                    },
                    MinimumRiskScore = 21,
                    MinimumRiskScoreFullRange = 0.0,
                }
                #endregion
            };
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
            var googleDiagnosisKeysDataMappingConfigUpdated = !exposureConfiguration1.GoogleDiagnosisKeysDataMappingConfig.Equals(exposureConfiguration2.GoogleDiagnosisKeysDataMappingConfig);
            var appleInfectiousnessForDaysSinceOnsetOfSymptoms = !exposureConfiguration1.AppleExposureConfigV2.InfectiousnessForDaysSinceOnsetOfSymptoms
                .SequenceEqual(exposureConfiguration2.AppleExposureConfigV2.InfectiousnessForDaysSinceOnsetOfSymptoms);
            return googleDiagnosisKeysDataMappingConfigUpdated || appleInfectiousnessForDaysSinceOnsetOfSymptoms;
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
            => _preferencesService.SetValue(PreferenceKey.IsDiagnosisKeysDataMappingConfigurationUpdated, isUpdated);

        public bool IsDiagnosisKeysDataMappingConfigurationUpdated()
            => _preferencesService.GetValue(PreferenceKey.IsDiagnosisKeysDataMappingConfigurationUpdated, true);

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
