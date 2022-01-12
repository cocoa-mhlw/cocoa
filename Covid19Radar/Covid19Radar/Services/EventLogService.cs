// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
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
        public Task SendAllAsync(long maxSize, int maxRetry);

        public Task SendAsync(
            string idempotencyKey,
            List<EventLog> eventLogList
            );
    }

    public class EventLogService : IEventLogService
    {
        private readonly IUserDataRepository _userDataRepository;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IServerConfigurationRepository _serverConfigurationRepository;
        private readonly IEssentialsService _essentialsService;
        private readonly IDeviceVerifier _deviceVerifier;
        private readonly IHttpClientService _httpClientService;

        private readonly ILoggerService _loggerService;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public EventLogService(
            IUserDataRepository userDataRepository,
            IEventLogRepository eventLogRepository,
            IServerConfigurationRepository serverConfigurationRepository,
            IEssentialsService essentialsService,
            IDeviceVerifier deviceVerifier,
            IHttpClientService httpClientService,
            ILoggerService loggerService
            )
        {
            _userDataRepository = userDataRepository;
            _eventLogRepository = eventLogRepository;
            _serverConfigurationRepository = serverConfigurationRepository;
            _essentialsService = essentialsService;
            _deviceVerifier = deviceVerifier;
            _httpClientService = httpClientService;
            _loggerService = loggerService;
        }

        public async Task SendAllAsync(long maxSize, int maxRetry)
        {
            await _semaphore.WaitAsync();

            try
            {
                await SendAllInternalAsync(maxSize, maxRetry);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task SendAllInternalAsync(long maxSize, int maxRetry)
        {
            _loggerService.StartMethod();

            try
            {
                List<EventLog> eventLogList
                    = await _eventLogRepository.GetLogsAsync(maxSize);

                if (eventLogList.Count == 0)
                {
                    _loggerService.Info($"No Event-logs found.");
                    return;
                }

                _loggerService.Info($"{eventLogList.Count} found Event-logs.");

                string idempotencyKey = Guid.NewGuid().ToString();

                for (var retryCount = 0; retryCount < maxRetry; retryCount++)
                {
                    try
                    {
                        await SendAsync(idempotencyKey, eventLogList);
                        _loggerService.Info($"Send complete.");

                        _loggerService.Info($"Clean up...");
                        foreach (var eventLog in eventLogList)
                        {
                            await _eventLogRepository.RemoveAsync(eventLog);
                        }
                        _loggerService.Info($"Done.");
                        break;
                    }
                    catch (Exception exception)
                    {
                        _loggerService.Exception("Exception occurred, SendAsync", exception);
                    }
                }
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task SendAsync(
            string idempotencyKey,
            List<EventLog> eventLogList
            )
        {
            _loggerService.StartMethod();

            bool hasConsent = _userDataRepository.IsSendEventLogEnabled();

            if (!hasConsent)
            {
                _loggerService.Debug($"No consent log.");
                _loggerService.EndMethod();
                return;
            }

            await _serverConfigurationRepository.LoadAsync();

            string exposureDataCollectServerEndpoint = _serverConfigurationRepository.EventLogApiEndpoint;
            _loggerService.Debug($"exposureDataCollectServerEndpoint: {exposureDataCollectServerEndpoint}");

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

                Uri uri = new Uri(exposureDataCollectServerEndpoint);

                using (var client = _httpClientService.Create())
                {
                    HttpResponseMessage response = await client.PutAsync(uri, httpContent);
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
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}
