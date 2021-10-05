/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services
{
    public interface IExposureDataCollectServer
    {
        public Task<List<ExposureDataResponse>> UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformation
            );

        public Task<List<ExposureDataResponse>> UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            IList<DailySummary> dailySummaries,
            IList<ExposureWindow> exposureWindows
            );

        public Task<List<ExposureDataResponse>> UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion
            );
    }

    public class ReleaseExposureDataCollectServer : IExposureDataCollectServer
    {
        public Task<List<ExposureDataResponse>> UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformation
            )
            => Task.FromResult(new List<ExposureDataResponse>());

        public Task<List<ExposureDataResponse>> UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            IList<DailySummary> dailySummaries,
            IList<ExposureWindow> exposureWindows
            )
            => Task.FromResult(new List<ExposureDataResponse>());

        public Task<List<ExposureDataResponse>> UploadExposureDataAsync(
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion
            )
            => Task.FromResult(new List<ExposureDataResponse>());
    }

#if DEBUG
    public class DebugExposureDataCollectServer : IExposureDataCollectServer
    {
        private readonly ILoggerService _loggerService;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly HttpClient _httpClient;

        public DebugExposureDataCollectServer(
            ILoggerService loggerService,
            IServerConfigurationRepository serverConfigurationRepository,
            IHttpClientService httpClientService
            )
        {
            _loggerService = loggerService;
            _serverConfigurationRepository = serverConfigurationRepository;
            _httpClient = httpClientService.Create();
        }

        public async Task<List<ExposureDataResponse>> UploadExposureDataAsync(
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

            return await UploadExposureDataAsync(exposureResult);
        }

        public async Task<List<ExposureDataResponse>> UploadExposureDataAsync(
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

            return await UploadExposureDataAsync(exposureResult);
        }

        public async Task<List<ExposureDataResponse>> UploadExposureDataAsync(
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

            return await UploadExposureDataAsync(exposureResult);
        }

        public async Task<List<ExposureDataResponse>> UploadExposureDataAsync(
            ExposureRequest exposureRequest
            )
        {
            await _serverConfigurationRepository.LoadAsync();

            var tasks = _serverConfigurationRepository.Regions.Select(async region => {
                return await UploadExposureDataAsync(exposureRequest, region);
            });

            ExposureDataResponse[] responses = await Task.WhenAll(tasks);
            var filteredResponse = responses.Where(response => response != null);
            return filteredResponse.ToList();
        }

        private async Task<ExposureDataResponse?> UploadExposureDataAsync(
            ExposureRequest exposureRequest,
            string region
            )
        {
            _loggerService.StartMethod();

            try
            {
                var exposureDataCollectServerEndpoint = _serverConfigurationRepository.GetExposureDataCollectServerUrl(region);
                _loggerService.Debug($"exposureDataCollectServerEndpoint: {exposureDataCollectServerEndpoint}");

                var requestJson = exposureRequest.ToJsonString();
                var httpContent = new StringContent(requestJson);

                Uri uri = new Uri(exposureDataCollectServerEndpoint);

                HttpResponseMessage response = await _httpClient.PutAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    _loggerService.Debug($"{responseJson}");

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
#endif

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
}
