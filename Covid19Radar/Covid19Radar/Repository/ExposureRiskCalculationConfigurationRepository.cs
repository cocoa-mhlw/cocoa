/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
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
        private const double TIMEOUT_SECONDS = 10.0;

        private readonly IHttpClientService _httpClientService;
        private readonly ILocalPathService _localPathService;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly ILoggerService _loggerService;

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
            _httpClientService = httpClientService;
            _localPathService = localPathService;
            _serverConfigurationRepository = serverConfigurationRepository;
            _loggerService = loggerService;

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
            await _semaphore.WaitAsync();

            try
            {
                return await GetExposureRiskCalculationConfigurationInternalAsync(preferCache);
            }
            finally
            {
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
                currentConfiguration = new V1ExposureRiskCalculationConfiguration();
            }
            else if(preferCache)
            {
                return currentConfiguration;
            }

            await _serverConfigurationRepository.LoadAsync();
            string url = _serverConfigurationRepository.ExposureRiskCalculationConfigurationUrl;

            V1ExposureRiskCalculationConfiguration newExposureRiskCalculationConfiguration = null;

            using (var client = _httpClientService.Create())
            {
                client.Timeout = TimeSpan.FromSeconds(TIMEOUT_SECONDS);
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string exposureRiskCalculationConfigurationAsJson = await response.Content.ReadAsStringAsync();
                    _loggerService.Debug(exposureRiskCalculationConfigurationAsJson);

                    try
                    {
                        newExposureRiskCalculationConfiguration = JsonConvert.DeserializeObject<V1ExposureRiskCalculationConfiguration>(exposureRiskCalculationConfigurationAsJson);
                    }
                    catch (JsonException exception)
                    {
                        _loggerService.Exception("JsonException.", exception);
                    }

                }
                else
                {
                    _loggerService.Warning($"Download ExposureRiskCalculationConfiguration failed from {url}");
                }
            }

            if (newExposureRiskCalculationConfiguration is null)
            {
                _loggerService.EndMethod();
                return currentConfiguration;
            }

            string tmpFilePath = Path.Combine(_configDir, Guid.NewGuid().ToString());

            try
            {
                await SaveAsync(
                    JsonConvert.SerializeObject(currentConfiguration, Formatting.Indented),
                    tmpFilePath
                    );
                Swap(tmpFilePath, _currentPath);

                return currentConfiguration;
            }
            finally
            {
                File.Delete(tmpFilePath);

                _loggerService.EndMethod();

            }
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
            catch(IOException exception)
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
}
