// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Covid19Radar.Repository
{
    public interface IServerConfigurationRepository
    {
        public string[] Regions { get; set; }

        public string UserRegisterApiEndpoint { get; set; }

        public string DiagnosisKeyRegisterApiEndpoint { get; set; }

        public virtual IList<string> DiagnosisKeyRegisterApiUrls => Regions
                    .Select(region => DiagnosisKeyRegisterApiEndpoint.Replace(ServerConfiguration.PLACEHOLDER_REGION, region))
                    .Where(url => url != null)
                    .Distinct()
                    .ToList();

        public string DiagnosisKeyListProvideServerEndpoint { get; set; }

        public virtual string GetDiagnosisKeyListProvideServerUrl(string region)
            => DiagnosisKeyListProvideServerEndpoint.Replace(ServerConfiguration.PLACEHOLDER_REGION, region);

        public virtual IList<string> DiagnosisKeyListProvideServerUrls => Regions
                    .Select(region => DiagnosisKeyListProvideServerEndpoint.Replace(ServerConfiguration.PLACEHOLDER_REGION, region))
                    .Where(url => url != null)
                    .Distinct()
                    .ToList();

        public string InquiryLogApiEndpoint { get; set; }

        public string ExposureConfigurationUrl { get; set; }

        public string? ExposureDataCollectServerEndpoint { get; set; }

        public virtual IList<string> ExposureDataCollectServerUrls => Regions
                    .Select(region => ExposureDataCollectServerEndpoint?.Replace(ServerConfiguration.PLACEHOLDER_REGION, region))
                    .Where(url => url != null)
                    .Distinct()
                    .ToList();

        public string? EventLogApiEndpoint { get; set; }

        public Task SaveAsync();

        public Task LoadAsync();

        /// <summary>
        /// Combine paths for constructing URL.
        ///
        /// `Path.Combine method` will remove elements before, if any element have `/` as head of string.
        /// This method keep all elements and combine these for constructing URL.
        ///
        /// See `Covid19Radar.UnitTests.Repository.ServerConfigurationRepositoryTests` for check behavior the method.
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string CombineAsUrl(params string[] paths)
        {
            if (paths is null || paths.Length == 0)
            {
                return string.Empty;
            }

            var filteredPaths = paths.Where(path => !string.IsNullOrEmpty(path));
            var lastPath = filteredPaths.Last();
            var hasLastSlash = lastPath.Last() == '/';

            var combinedUrl = string.Join('/', filteredPaths.Select(path => path.TrimStart('/').TrimEnd('/')));

            if (hasLastSlash)
            {
                return combinedUrl + '/';
            }
            return combinedUrl;
        }
    }

    public class DebugServerConfigurationRepository : IServerConfigurationRepository
    {
        private const string FILE_NAME = "server_configuration.json";
        private const string CONFIGURATION_DIR = "configuration";

        private readonly string _configurationDir;
        private readonly string _serverConfigurationPath;

        private ServerConfiguration _serverConfiguration;

        public DebugServerConfigurationRepository()
        {
            _configurationDir = Path.Combine(FileSystem.AppDataDirectory, CONFIGURATION_DIR);
            if (!Directory.Exists(_configurationDir))
            {
                Directory.CreateDirectory(_configurationDir);
            }
            _serverConfigurationPath = Path.Combine(_configurationDir, FILE_NAME);

        }

        public string UserRegisterApiEndpoint
        {
            get => _serverConfiguration.UserRegisterApiEndpoint;
            set => _serverConfiguration.UserRegisterApiEndpoint = value;
        }

        public string InquiryLogApiEndpoint
        {
            get => _serverConfiguration.InquiryLogApiEndpoint;
            set => _serverConfiguration.InquiryLogApiEndpoint = value;
        }

        public string[] Regions
        {
            get => _serverConfiguration.Regions.Split(",").Where(region => !string.IsNullOrEmpty(region)).ToArray();
            set => _serverConfiguration.Regions = string.Join(",", value);
        }

        public string DiagnosisKeyRegisterApiEndpoint
        {
            get => _serverConfiguration.DiagnosisKeyRegisterApiEndpoint;
            set => _serverConfiguration.DiagnosisKeyRegisterApiEndpoint = value;
        }

        public string ExposureDataCollectServerEndpoint
        {
            get => _serverConfiguration.ExposureDataCollectServerEndpoint;
            set => _serverConfiguration.ExposureDataCollectServerEndpoint = value;
        }

        public string ExposureConfigurationUrl
        {
            get => _serverConfiguration.ExposureConfigurationUrl;
            set => _serverConfiguration.ExposureConfigurationUrl = value;
        }

        public string DiagnosisKeyListProvideServerEndpoint
        {
            get => _serverConfiguration.DiagnosisKeyListProvideServerEndpoint;
            set => _serverConfiguration.DiagnosisKeyListProvideServerEndpoint = value;
        }

        public string? EventLogApiEndpoint
        {
            get => _serverConfiguration.EventLogApiEndpoint;
            set => _serverConfiguration.EventLogApiEndpoint = value;
        }

        public async Task LoadAsync()
        {
            if (File.Exists(_serverConfigurationPath))
            {
                string config = await File.ReadAllTextAsync(_serverConfigurationPath);
                _serverConfiguration = JsonConvert.DeserializeObject<ServerConfiguration>(config);
                return;
            }

            _serverConfiguration = new ServerConfiguration();
            var configJson = JsonConvert.SerializeObject(_serverConfiguration, Formatting.Indented);
            await File.WriteAllTextAsync(_serverConfigurationPath, configJson);
        }

        public async Task SaveAsync()
        {
            var configJson = JsonConvert.SerializeObject(_serverConfiguration, Formatting.Indented);
            await File.WriteAllTextAsync(_serverConfigurationPath, configJson);
        }
    }

    public class ReleaseServerConfigurationRepository : IServerConfigurationRepository
    {
        public string UserRegisterApiEndpoint
        {
            get => IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "register");
            set
            {
                // Do nothing
            }
        }

        public string InquiryLogApiEndpoint
        {
            get => IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "inquirylog");
            set
            {
                // Do nothing
            }
        }

        public string[] Regions
        {
            get => AppSettings.Instance.SupportedRegions;
            set
            {
                //  Do nothing
            }
        }

        public string DiagnosisKeyRegisterApiEndpoint
        {
            get => IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ApiUrlBase, AppConstants.DiagnosisApiVersionCode, "diagnosis");
            set
            {
                // Do nothing
            }
        }

        public string DiagnosisKeyListProvideServerEndpoint
        {
            get => null;
            set
            {
                // Do nothing
            }
        }

        public string ExposureConfigurationUrl
        {
            // TODO: Replace url for RELEASE.
            get => IServerConfigurationRepository.CombineAsUrl(
                AppSettings.Instance.ExposureConfigurationUrlBase,
                "exposure_configuration/Cappuccino",
                "configuration.json"
                );
            set
            {
                // Do nothing
            }
        }

        public string? ExposureDataCollectServerEndpoint
        {
            get => null;
            set
            {
                // Do nothing
            }
        }

        public string? EventLogApiEndpoint
        {
            get => IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "v1", "event_log");
            set
            {
                // Do nothing
            }
        }

        public Task SaveAsync()
        {
            // Do nothing
            return Task.CompletedTask;
        }

        public Task LoadAsync()
        {
            // Do nothing
            return Task.CompletedTask;
        }

    }

    [JsonObject]
    public class ServerConfiguration
    {
        public const string PLACEHOLDER_REGION = "{region}";

        /// <summary>
        /// Specifies the format version of configuration file.
        /// If any events will be occurred that require migration process.(e.g. Add/delete member, change structure, or rename key)
        /// This value will be increment.
        /// </summary>
        [JsonProperty("format_version")]
        public int FormatVersion = 1;

        [JsonProperty("user_register_api_endpoint")]
        public string UserRegisterApiEndpoint = IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "register");

        [JsonProperty("inquiry_log_api_endpoint")]
        public string? InquiryLogApiEndpoint = IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.LogStorageEndpoint, "inquirylog");

        [JsonProperty("regions")]
        public string Regions = string.Join(",", AppSettings.Instance.SupportedRegions);

        [JsonProperty("diagnosis_key_register_api_endpoint")]
        public string DiagnosisKeyRegisterApiEndpoint
            = IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ApiUrlBase, AppConstants.DiagnosisApiVersionCode, "diagnosis");

        [JsonProperty("diagnosis_key_list_provide_server_endpoint")]
        public string DiagnosisKeyListProvideServerEndpoint
            = IServerConfigurationRepository.CombineAsUrl(
                AppSettings.Instance.CdnUrlBase,
                AppSettings.Instance.BlobStorageContainerName,
                PLACEHOLDER_REGION,
                "list.json"
                );

        [JsonProperty("event_log_api_endpoint")]
        public string? EventLogApiEndpoint
            = IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "v1", "event_log");

        [JsonProperty("exposure_configuration_url")]
        public string? ExposureConfigurationUrl
            = IServerConfigurationRepository.CombineAsUrl(AppSettings.Instance.ExposureConfigurationUrlBase, "exposure_configuration", "configuration.json");

        [JsonProperty("exposure_data_collect_server_endpoint")]
        public string? ExposureDataCollectServerEndpoint = null;
    }

}
