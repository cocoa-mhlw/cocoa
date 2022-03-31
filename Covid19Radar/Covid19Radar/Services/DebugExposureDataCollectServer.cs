/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services
{
    public interface IDebugExposureDataCollectServer
    {
        public Task UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformation
            );

        public Task UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            IList<DailySummary> dailySummaries,
            IList<ExposureWindow> exposureWindows
            );

        public Task UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion
            );
    }

    public class DebugExposureDataCollectServerNop : IDebugExposureDataCollectServer
    {
        public Task UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformation
            )
            => Task.CompletedTask;

        public Task UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            IList<DailySummary> dailySummaries,
            IList<ExposureWindow> exposureWindows
            )
            => Task.CompletedTask;

        public Task UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion
            )
            => Task.CompletedTask;
    }

/// For user-privacy, this class is used for DEBUG only.
/// Below #if is intented safe-guard for prevent contamination.
/// DO NOT REMOVE.
#if DEBUG
    public class DebugExposureDataCollectServer : IDebugExposureDataCollectServer
    {
        private readonly ILoggerService _loggerService;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly IHttpClientService _httpClientService;

        public DebugExposureDataCollectServer(
            ILoggerService loggerService,
            IServerConfigurationRepository serverConfigurationRepository,
            IHttpClientService httpClientService
            )
        {
            _loggerService = loggerService;
            _serverConfigurationRepository = serverConfigurationRepository;
            _httpClientService = httpClientService;
        }

        public async Task UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformation
            )
        {
            var exposureResult = new ExposureRequest(exposureConfiguration,
                exposureSummary, exposureInformation
                )
            {
                Device = deviceModel,
                EnVersion = enVersion,
            };

            await UploadExposureDataAsync(exposureResult);
        }

        public async Task UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            IList<DailySummary> dailySummaries,
            IList<ExposureWindow> exposureWindows
            )
        {
            var exposureResult = new ExposureRequest(exposureConfiguration,
                dailySummaries, exposureWindows
                )
            {
                Device = deviceModel,
                EnVersion = enVersion,
            };

            await UploadExposureDataAsync(exposureResult);
        }

        public async Task UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion
            )
        {
            var exposureResult = new ExposureRequest(
                exposureConfiguration
                )
            {
                Device = deviceModel,
                EnVersion = enVersion,
            };

            await UploadExposureDataAsync(exposureResult);
        }

        public async Task UploadExposureDataAsync(
            ExposureRequest exposureRequest
            )
        {
            await _serverConfigurationRepository.LoadAsync();

            foreach (var url in _serverConfigurationRepository.ExposureDataCollectServerUrls)
            {
                await UploadExposureDataAsync(exposureRequest, url);
            }
        }

        private async Task<ExposureDataResponse?> UploadExposureDataAsync(
            ExposureRequest exposureRequest,
            string exposureDataCollectServerEndpoint
            )
        {
            _loggerService.StartMethod();
            _loggerService.Debug($"exposureDataCollectServerEndpoint: {exposureDataCollectServerEndpoint}");

            try
            {
                var requestJson = exposureRequest.ToJsonString();

                _loggerService.Info(requestJson);

                var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                Uri uri = new Uri(exposureDataCollectServerEndpoint);

                HttpResponseMessage response = await _httpClientService.HttpClient.PutAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    _loggerService.Debug($"response {responseJson}");

                    return JsonConvert.DeserializeObject<ExposureDataResponse>(responseJson);
                }
                else
                {
                    _loggerService.Info($"UploadExposureDataAsync {response.StatusCode}");
                    return null;
                }
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }

    public class ExposureRequest
    {
        private string _device = "unknown_device";

        [JsonProperty("device")]
        public string Device
        {
            get
            {
                return _device;
            }
            set
            {
                string device = value.Replace(" ", "_");
                _device = device;
            }
        }

        [JsonProperty("en_version")]
        public string? EnVersion;

        [JsonProperty("exposure_summary")]
        public readonly ExposureSummary? exposureSummary;

        [JsonProperty("exposure_informations")]
        public readonly IList<ExposureInformation>? exposureInformations;

        [JsonProperty("daily_summaries")]
        public readonly IList<DailySummary>? dailySummaries;

        [JsonProperty("exposure_windows")]
        public readonly IList<ExposureWindow>? exposureWindows;

        [JsonProperty("exposure_configuration")]
        public readonly ExposureConfiguration exposureConfiguration;

        public ExposureRequest(ExposureConfiguration exposureConfiguration)
            : this(exposureConfiguration, null, null, null, null) { }

        public ExposureRequest(ExposureConfiguration exposureConfiguration,
            ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
            : this(exposureConfiguration, exposureSummary, exposureInformations, null, null) { }

        public ExposureRequest(ExposureConfiguration exposureConfiguration,
            IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
            : this(exposureConfiguration, null, null, dailySummaries, exposureWindows) { }

        public ExposureRequest(ExposureConfiguration exposureConfiguration,
            ExposureSummary? exposureSummary, IList<ExposureInformation>? exposureInformations,
            IList<DailySummary>? dailySummaries, IList<ExposureWindow>? exposureWindows)
        {
            this.exposureConfiguration = exposureConfiguration;
            this.exposureSummary = exposureSummary;
            this.exposureInformations = exposureInformations;
            this.dailySummaries = dailySummaries;
            this.exposureWindows = exposureWindows;
        }

        public string ToJsonString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class ExposureDataResponse
    {
        [JsonProperty("device")]
        public string? Device;

        [JsonProperty("en_version")]
        public string? EnVersion;

        [JsonProperty("exposure_summary")]
        public readonly ExposureSummary? ExposureSummary;

        [JsonProperty("exposure_informations")]
        public readonly IList<ExposureInformation>? ExposureInformations;

        [JsonProperty("daily_summaries")]
        public readonly IList<DailySummary>? DailySummaries;

        [JsonProperty("exposure_windows")]
        public readonly IList<ExposureWindow>? ExposureWindows;

        [JsonProperty("generated_at")]
        public readonly string? GeneratedAt;

        [JsonProperty("exposure_configuration")]
        public readonly ExposureConfiguration? ExposureConfiguration;

        [JsonProperty("file_name")]
        public readonly string? FileName;

        [JsonProperty("uri")]
        public readonly string? Uri;

    }
#endif
}
