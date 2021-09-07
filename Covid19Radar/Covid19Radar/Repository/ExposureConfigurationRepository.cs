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
        public string ResourceUrl { get; }
        public Task<ExposureConfiguration> GetExposureConfigurationAsync();
    }

    public class ExposureConfigurationRepository : IExposureConfigurationRepository
    {
        private const string CONFIG_DIR = "config";
        private const string EXPOSURE_CONFIGURATION_FILENAME = "exposure_configuration.json";

        private readonly HttpClient _client;
        private readonly ILoggerService _loggerService;

        private readonly string _configDir;
        private readonly string _exposureConfigurationPath;

        public ExposureConfigurationRepository(
            IHttpClientService httpClientService,
            ILoggerService loggerService
            )
        {
            _client = httpClientService.Create();
            _loggerService = loggerService;

            _configDir = PrepareConfigDir();
            _exposureConfigurationPath = Path.Combine(_configDir, EXPOSURE_CONFIGURATION_FILENAME);
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

        // TODO: We should make consideration later.
        public string ResourceUrl
            => "https://raw.githubusercontent.com/keiji/chino/master/Chino.Common.Tests/files/exposure_configuration.json";

        public async Task<ExposureConfiguration> GetExposureConfigurationAsync()
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
            string url = ResourceUrl;

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
    }
}
