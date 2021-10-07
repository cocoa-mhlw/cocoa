/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Covid19Radar.Repository
{
    public interface IExposureConfigurationRepository
    {
        public Task<ExposureConfiguration> GetExposureConfigurationAsync(string region);
        public void RemoveExposureConfiguration();
    }

    public class ExposureConfigurationRepository : IExposureConfigurationRepository
    {
        private const string CONFIG_DIR = "exposure_configuration";

        private readonly HttpClient _client;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly ILoggerService _loggerService;

        private readonly string _configDir;
        private readonly string _exposureConfigurationPath;

        public ExposureConfigurationRepository(
            IHttpClientService httpClientService,
            IServerConfigurationRepository serverConfigurationRepository,
            ILoggerService loggerService
            )
        {
            _client = httpClientService.Create();
            _serverConfigurationRepository = serverConfigurationRepository;
            _loggerService = loggerService;

            _configDir = PrepareConfigDir();
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

        public async Task<ExposureConfiguration> GetExposureConfigurationAsync(string region)
        {
            _loggerService.StartMethod();

            if (File.Exists(_exposureConfigurationPath))
            {
                _loggerService.Debug("ExposureConfiguration file is found in cache-dir.");

                string exposureConfigurationAsJson = await LoadAsync();

                try
                {
                    return JsonConvert.DeserializeObject<ExposureConfiguration>(exposureConfigurationAsJson);
                }
                catch (JsonException exception)
                {
                    _loggerService.Exception("JsonException. Cached ExposureConfiguration file has been deleted.", exception);
                    File.Delete(_exposureConfigurationPath);
                }
            }

            ExposureConfiguration exposureConfiguration;
            string url = _serverConfigurationRepository.GetExposureConfigurationUrl(region);

            var response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string exposureConfigurationAsJson = await response.Content.ReadAsStringAsync();
                _loggerService.Debug(exposureConfigurationAsJson);

                try
                {
                    exposureConfiguration = JsonConvert.DeserializeObject<ExposureConfiguration>(exposureConfigurationAsJson);
                    await SaveAsync(exposureConfigurationAsJson);
                }
                catch (JsonException exception)
                {
                    _loggerService.Exception("JsonException. Default configuration will be loaded.", exception);
                    exposureConfiguration = new ExposureConfiguration();
                }

            }
            else
            {
                _loggerService.Warning($"Download ExposureConfiguration failed from {url}. Default configuration will be loaded.");
                exposureConfiguration = new ExposureConfiguration();
            }

            _loggerService.EndMethod();

            return exposureConfiguration;
        }

        private async Task<string> LoadAsync()
        {
            using var reader = File.OpenText(_exposureConfigurationPath);
            return await reader.ReadToEndAsync();
        }

        private async Task SaveAsync(string exposureConfigurationAsJson)
            => await File.WriteAllTextAsync(_exposureConfigurationPath, exposureConfigurationAsJson);

        public void RemoveExposureConfiguration()
        {
            if (!File.Exists(_exposureConfigurationPath))
            {
                _loggerService.Debug("No ExposureConfiguration file found.");
                return;
            }

            File.Delete(_exposureConfigurationPath);
        }
    }
}
