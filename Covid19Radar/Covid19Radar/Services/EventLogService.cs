// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services
{
    public interface IEventLogService
    {
        public Task SendExposureDataAsync(
            string idempotencyKey,
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformation
            );

        public Task SendExposureDataAsync(
            string idempotencyKey,
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            IList<DailySummary> dailySummaries,
            IList<ExposureWindow> exposureWindows
            );

        public Task SendExposureDataAsync(
            string idempotencyKey,
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion
            );
    }

    public class EventLogService : IEventLogService
    {
        private readonly IUserDataRepository _userDataRepository;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly IEssentialsService _essentialsService;
        private readonly IDeviceVerifier _deviceVerifier;
        private readonly IDateTimeUtility _dateTimeUtility;

        private readonly ILoggerService _loggerService;

        private readonly HttpClient _httpClient;

        public EventLogService(
            IUserDataRepository userDataRepository,
            IServerConfigurationRepository serverConfigurationRepository,
            IEssentialsService essentialsService,
            IDeviceVerifier deviceVerifier,
            IDateTimeUtility dateTimeUtility,
            IHttpClientService httpClientService,
            ILoggerService loggerService
            )
        {
            _userDataRepository = userDataRepository;
            _serverConfigurationRepository = serverConfigurationRepository;
            _essentialsService = essentialsService;
            _deviceVerifier = deviceVerifier;
            _dateTimeUtility = dateTimeUtility;
            _loggerService = loggerService;

            _httpClient = httpClientService.Create();
        }

        public async Task SendExposureDataAsync(
            string idempotencyKey,
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformation
            )
        {
            var data = new ExposureData(exposureConfiguration,
                exposureSummary, exposureInformation
                )
            {
                Device = deviceModel,
                EnVersion = enVersion,
            };

            await SendExposureDataAsync(idempotencyKey, data);
        }

        public async Task SendExposureDataAsync(
            string idempotencyKey,
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion,
            IList<DailySummary> dailySummaries,
            IList<ExposureWindow> exposureWindows
            )
        {
            var data = new ExposureData(exposureConfiguration,
                dailySummaries, exposureWindows
                )
            {
                Device = deviceModel,
                EnVersion = enVersion,
            };

            await SendExposureDataAsync(idempotencyKey, data);
        }

        public async Task SendExposureDataAsync(
            string idempotencyKey,
            ExposureConfiguration exposureConfiguration,
            string deviceModel,
            string enVersion
            )
        {
            var data = new ExposureData(
                exposureConfiguration
                )
            {
                Device = deviceModel,
                EnVersion = enVersion,
            };

            await SendExposureDataAsync(idempotencyKey, data);
        }


        private async Task SendExposureDataAsync(
            string idempotencyKey,
            ExposureData exposureData
            )
        {
            _loggerService.StartMethod();

            SendEventLogState sendEventLogState = _userDataRepository.GetSendEventLogState();
            bool isEnabled = sendEventLogState != SendEventLogState.Enable;

            if (isEnabled)
            {
                _loggerService.Debug($"Send event-log function is not enabled.");
                _loggerService.EndMethod();
                return;
            }

            await _serverConfigurationRepository.LoadAsync();

            string exposureDataCollectServerEndpoint = _serverConfigurationRepository.EventLogApiEndpoint;
            _loggerService.Debug($"exposureDataCollectServerEndpoint: {exposureDataCollectServerEndpoint}");

            try
            {
                var contentJson = exposureData.ToJsonString();

                var eventLog = new V1EventLogRequest.EventLog() {
                    HasConsent = isEnabled,
                    Epoch = _dateTimeUtility.UtcNow.ToUnixEpoch(),
                    Type = "ExposureData",
                    Subtype = "Debug",
                    Content = contentJson,
                };
                var eventLogs = new[] { eventLog };

                var request = new V1EventLogRequest()
                {
                    IdempotencyKey = idempotencyKey,
                    Platform = _essentialsService.Platform,
                    AppPackageName = _essentialsService.AppPackageName,
                    EventLogs = eventLogs,
                };

                request.DeviceVerificationPayload = await _deviceVerifier.VerifyAsync(request);

                var requestJson = request.ToJsonString();

                var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                Uri uri = new Uri(exposureDataCollectServerEndpoint);

                HttpResponseMessage response = await _httpClient.PutAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    _loggerService.Debug($"{responseJson}");
                }
                else
                {
                    _loggerService.Info($"UploadExposureDataAsync {response.StatusCode}");
                }
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }

    public class ExposureData
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

        public ExposureData(ExposureConfiguration exposureConfiguration)
            : this(exposureConfiguration, null, null, null, null) { }

        public ExposureData(ExposureConfiguration exposureConfiguration,
            ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
            : this(exposureConfiguration, exposureSummary, exposureInformations, null, null) { }

        public ExposureData(ExposureConfiguration exposureConfiguration,
            IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
            : this(exposureConfiguration, null, null, dailySummaries, exposureWindows) { }

        public ExposureData(ExposureConfiguration exposureConfiguration,
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
}
