// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public interface IEventLogService
    {
        public Task<List<EventLog>> SendAllAsync(long maxSize, int maxRetry);
    }

    public class EventLogService : IEventLogService
    {
        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly IEssentialsService _essentialsService;
        private readonly IDeviceVerifier _deviceVerifier;

        private readonly ILoggerService _loggerService;
        private readonly HttpClient _httpClient;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public EventLogService(
            ISendEventLogStateRepository sendEventLogStateRepository,
            IEventLogRepository eventLogRepository,
            IServerConfigurationRepository serverConfigurationRepository,
            IEssentialsService essentialsService,
            IDeviceVerifier deviceVerifier,
            IHttpClientService httpClientService,
            ILoggerService loggerService
            )
        {
            _sendEventLogStateRepository = sendEventLogStateRepository;
            _eventLogRepository = eventLogRepository;
            _serverConfigurationRepository = serverConfigurationRepository;
            _essentialsService = essentialsService;
            _deviceVerifier = deviceVerifier;
            _loggerService = loggerService;

            _httpClient = httpClientService.CreateApiClient();
        }

        public async Task<List<EventLog>> SendAllAsync(long maxSize, int maxRetry)
        {
            await _semaphore.WaitAsync();

            try
            {
                return await SendAllInternalAsync(maxSize, maxRetry);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<List<EventLog>> SendAllInternalAsync(long maxSize, int maxRetry)
        {
            _loggerService.StartMethod();

            try
            {
                List<EventLog> eventLogList = await _eventLogRepository.GetLogsAsync(maxSize);
                if (eventLogList.Count == 0)
                {
                    _loggerService.Info($"No Event-logs found.");
                    return new List<EventLog>();
                }

                IDictionary<string, SendEventLogState> eventStateDict = new Dictionary<string, SendEventLogState>();
                foreach (var eventType in ISendEventLogStateRepository.EVENT_TYPE_ALL)
                {
                    eventStateDict[eventType.ToString()] = _sendEventLogStateRepository.GetSendEventLogState(eventType);
                }

                foreach (var eventLog in eventLogList)
                {
                    eventLog.HasConsent = eventStateDict[eventLog.GetEventType()] == SendEventLogState.Enable;
                }

                string idempotencyKey = Guid.NewGuid().ToString();

                for (var retryCount = 0; retryCount < maxRetry; retryCount++)
                {
                    try
                    {
                        List<EventLog> sentEventLogList = await SendAsync(
                            idempotencyKey,
                            eventLogList
                                .Where(eventLog => eventLog.HasConsent)
                                .ToList()
                        );
                        _loggerService.Info($"Send complete.");

                        _loggerService.Info($"Clean up...");
                        foreach (var eventLog in eventLogList)
                        {
                            await _eventLogRepository.RemoveAsync(eventLog);
                        }
                        _loggerService.Info($"Done.");

                        return sentEventLogList;
                    }
                    catch (Exception exception)
                    {
                        _loggerService.Exception("Exception occurred, SendAsync", exception);
                    }
                }

                return new List<EventLog>();
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        private async Task<List<EventLog>> SendAsync(
            string idempotencyKey,
            List<EventLog> eventLogList
            )
        {
            _loggerService.StartMethod();

            await _serverConfigurationRepository.LoadAsync();

            string eventLogApiEndpoint = _serverConfigurationRepository.EventLogApiEndpoint;
            _loggerService.Debug($"eventLogApiEndpoint: {eventLogApiEndpoint}");

            try
            {
                var request = new V1EventLogRequest()
                {
                    IdempotencyKey = idempotencyKey,
                    Platform = _essentialsService.Platform,
                    AppPackageName = _essentialsService.AppPackageName,
                    EventLogs = eventLogList,
                };

                request.DeviceVerificationPayload = await _deviceVerifier.VerifyAsync(request);

                var requestJson = request.ToJsonString();

                var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

                Uri uri = new Uri(eventLogApiEndpoint);

                HttpResponseMessage response = await _httpClient.PutAsync(uri, httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    _loggerService.Debug($"{responseJson}");

                    return eventLogList;
                }
                else
                {
                    _loggerService.Info($"UploadExposureDataAsync {response.StatusCode}");
                }

                return new List<EventLog>();
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}
