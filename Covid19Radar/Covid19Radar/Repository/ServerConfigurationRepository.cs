// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
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

        public string GetDiagnosisKeyListProvideServerUrl(string region, string listFileName = "list.json");

        public string DiagnosisKeyListProvideServerBaseEndpoint { get; set; }

        public string InquiryLogApiEndpoint { get; set; }

        public string? ExposureDataCollectServerEndpoint { get; set; }

        public Task SaveAsync();

        public Task LoadAsync();
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

        public string[] Regions
        {
            get => _serverConfiguration.Regions.Split(",");
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

        public string GetDiagnosisKeyListProvideServerUrl(string region, string listFileName = "list.json")
            => Utils.CombineAsUrl(_serverConfiguration.DiagnosisKeyListProvideServerBaseEndpoint, region, listFileName);

        public string DiagnosisKeyListProvideServerBaseEndpoint
        {
            get => _serverConfiguration.DiagnosisKeyListProvideServerBaseEndpoint;
            set => _serverConfiguration.DiagnosisKeyListProvideServerBaseEndpoint = value;
        }

        public string InquiryLogApiEndpoint
        {
            get => _serverConfiguration.InquiryLogApiEndpoint;
            set => _serverConfiguration.InquiryLogApiEndpoint = value;
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
            get => Utils.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "register");
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
            get => Utils.CombineAsUrl(AppSettings.Instance.ApiUrlBase, AppSettings.Instance.DiagnosisApiVersion, "diagnosis");
            set
            {
                // Do nothing
            }
        }

        public string GetDiagnosisKeyListProvideServerUrl(string region, string listFileName = "list.json")
            => Utils.CombineAsUrl(AppSettings.Instance.CdnUrlBase, AppSettings.Instance.BlobStorageContainerName, region, listFileName);

        public string DiagnosisKeyListProvideServerBaseEndpoint
        {
            get => null;
            set
            {
                // Do nothing
            }
        }

        public string InquiryLogApiEndpoint
        {
            get => Utils.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "inquirylog");
            set
            {
                // Do nothing
            }
        }

        public string ExposureDataCollectServerEndpoint
        {
            get => null;
            set
            {
                // Do nothing
            }
        }

        public Task SaveAsync()
        {
            throw new NotSupportedException();
        }

        public Task LoadAsync()
        {
            throw new NotSupportedException();
        }

    }

    [JsonObject]
    public class ServerConfiguration
    {
        [JsonProperty("user_register_api_endpoint")]
        public string UserRegisterApiEndpoint = Utils.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "register");

        [JsonProperty("regions")]
        public string Regions = string.Join(",", AppSettings.Instance.SupportedRegions);

        [JsonProperty("diagnosis_key_register_api_endpoint")]
        public string DiagnosisKeyRegisterApiEndpoint = Utils.CombineAsUrl(AppSettings.Instance.ApiUrlBase, AppSettings.Instance.DiagnosisApiVersion, "diagnosis");

        [JsonProperty("diagnosis_key_list_provide_server_base_endpoint")]
        public string DiagnosisKeyListProvideServerBaseEndpoint = Utils.CombineAsUrl(AppSettings.Instance.CdnUrlBase, AppSettings.Instance.BlobStorageContainerName);

        [JsonProperty("inquiry_log_api_endpoint")]
        public string? InquiryLogApiEndpoint = Utils.CombineAsUrl(AppSettings.Instance.ApiUrlBase, "inquirylog");

        [JsonProperty("exposure_data_collect_server_endpoint")]
        public string? ExposureDataCollectServerEndpoint = null;
    }

}
