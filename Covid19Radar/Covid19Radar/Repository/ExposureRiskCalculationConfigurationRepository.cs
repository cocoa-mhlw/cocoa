/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public interface IExposureRiskCalculationConfigurationRepository
    {
        public Task<V1ExposureRiskCalculationConfiguration> GetExposureRiskCalculationConfigurationAsync(bool preferCache);
    }

    public class ExposureRiskCalculationConfigurationRepository : IExposureRiskCalculationConfigurationRepository
    {
        private readonly ILocalPathService _localPathService;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly ILoggerService _loggerService;

        private readonly IHttpClientService _httpClientService;

        private readonly string _configDir;

        private string _currentPath;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public ExposureRiskCalculationConfigurationRepository(
            IHttpClientService httpClientService,
            ILocalPathService localPathService,
            IServerConfigurationRepository serverConfigurationRepository,
            ILoggerService loggerService
            )
        {
            _localPathService = localPathService;
            _serverConfigurationRepository = serverConfigurationRepository;
            _loggerService = loggerService;

            _httpClientService = httpClientService;

            _configDir = PrepareConfigDir();
            _currentPath = localPathService.CurrentExposureRiskCalculationConfigurationPath;
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

        public async Task<V1ExposureRiskCalculationConfiguration> GetExposureRiskCalculationConfigurationAsync(bool preferCache)
        {
            _loggerService.StartMethod();

            await _semaphore.WaitAsync();

            try
            {
                return await GetExposureRiskCalculationConfigurationInternalAsync(preferCache);
            }
            finally
            {
                _loggerService.EndMethod();
                _semaphore.Release();
            }
        }

        private async Task<V1ExposureRiskCalculationConfiguration> GetExposureRiskCalculationConfigurationInternalAsync(bool preferCache)
        {
            _loggerService.StartMethod();

            V1ExposureRiskCalculationConfiguration currentConfiguration = null;

            if (File.Exists(_currentPath))
            {
                _loggerService.Debug("ExposureConfiguration file is found.");

                try
                {
                    string exposureRiskCalculationConfigurationAsJson = await LoadAsync(_currentPath);

                    currentConfiguration = JsonConvert.DeserializeObject<V1ExposureRiskCalculationConfiguration>(exposureRiskCalculationConfigurationAsJson);

                }
                catch (IOException exception)
                {
                    _loggerService.Exception("IOException. ExposureRiskCalculationConfiguration file has been deleted.", exception);
                    RemoveExposureRiskCalculationConfiguration();
                }
                catch (JsonException exception)
                {
                    _loggerService.Exception("JsonException. ExposureRiskCalculationConfiguration file has been deleted.", exception);
                    RemoveExposureRiskCalculationConfiguration();
                }
            }

            if (currentConfiguration is null)
            {
                currentConfiguration = CreateDefaultConfiguration();
            }
            else if (preferCache)
            {
                _loggerService.EndMethod();
                return currentConfiguration;
            }

            await _serverConfigurationRepository.LoadAsync();
            string url = _serverConfigurationRepository.ExposureRiskCalculationConfigurationUrl;

            V1ExposureRiskCalculationConfiguration newExposureRiskCalculationConfiguration = null;

            try
            {
                var response = await _httpClientService.StaticJsonContentClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string exposureRiskCalculationConfigurationAsJson = await response.Content.ReadAsStringAsync();
                    _loggerService.Debug(exposureRiskCalculationConfigurationAsJson);
                    newExposureRiskCalculationConfiguration
                        = JsonConvert.DeserializeObject<V1ExposureRiskCalculationConfiguration>(exposureRiskCalculationConfigurationAsJson);
                }
                else
                {
                    _loggerService.Warning($"Download ExposureRiskCalculationConfiguration failed from {url}");
                }
            }
            catch (JsonException exception)
            {
                _loggerService.Exception("JsonException.", exception);
            }
            catch (HttpRequestException exception)
            {
                _loggerService.Exception("HttpRequestException.", exception);
            }

            if (newExposureRiskCalculationConfiguration is null)
            {
                _loggerService.EndMethod();
                return currentConfiguration;
            }

            if (newExposureRiskCalculationConfiguration.Equals(currentConfiguration))
            {
                _loggerService.Info("ExposureRiskCalculationConfiguration have not been changed.");
                _loggerService.EndMethod();

                return currentConfiguration;
            }

            _loggerService.Info("ExposureRiskCalculationConfiguration have been changed.");

            string tmpFilePath = Path.Combine(_configDir, Guid.NewGuid().ToString());

            try
            {
                await SaveAsync(
                    JsonConvert.SerializeObject(newExposureRiskCalculationConfiguration, Formatting.Indented),
                    tmpFilePath
                    );
                Swap(tmpFilePath, _currentPath);

                return newExposureRiskCalculationConfiguration;
            }
            finally
            {
                File.Delete(tmpFilePath);

                _loggerService.EndMethod();

            }
        }

        private static V1ExposureRiskCalculationConfiguration CreateDefaultConfiguration()
        {
            return new V1ExposureRiskCalculationConfiguration()
            {
                DailySummary_DaySummary_ScoreSum = new V1ExposureRiskCalculationConfiguration.Threshold()
                {
                    Op = V1ExposureRiskCalculationConfiguration.Threshold.OPERATION_GREATER_EQUAL,
                    Value = 1350.0,
                },
                ExposureWindow_ScanInstance_SecondsSinceLastScanSum = new V1ExposureRiskCalculationConfiguration.Threshold()
                {
                    Op = V1ExposureRiskCalculationConfiguration.Threshold.OPERATION_GREATER_EQUAL,
                    Value = 900.0,
                },
            };
        }

        private void RemoveExposureRiskCalculationConfiguration()
        {
            if (!File.Exists(_currentPath))
            {
                _loggerService.Debug("No ExposureRiskCalculationConfiguration file found.");
                return;
            }

            File.Delete(_currentPath);
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

        private async Task<string> LoadAsync(string path)
        {
            using var reader = File.OpenText(path);
            return await reader.ReadToEndAsync();
        }

        private async Task SaveAsync(string exposureConfigurationAsJson, string outPath)
            => await File.WriteAllTextAsync(outPath, exposureConfigurationAsJson);
    }

    public class ExposureRiskCalculationConfigurationRepositoryMock : IExposureRiskCalculationConfigurationRepository
    {
        public Task<V1ExposureRiskCalculationConfiguration> GetExposureRiskCalculationConfigurationAsync(bool preferCache)
            => Task.FromResult(new V1ExposureRiskCalculationConfiguration());
    }
}
