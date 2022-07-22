// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;
using Polly;

namespace Covid19Radar.Services
{
    public interface IEventLogService
    {
        public Task SendAllAsync(long maxSize, int maxRetry);
    }

    public class EventLogService : IEventLogService
    {
        private readonly ISendEventLogStateRepository _sendEventLogStateRepository;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly IEssentialsService _essentialsService;
        private readonly IDeviceVerifier _deviceVerifier;
        private readonly IHttpDataService _httpDataService;
        private readonly ILoggerService _loggerService;

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public EventLogService(
            ISendEventLogStateRepository sendEventLogStateRepository,
            IEventLogRepository eventLogRepository,
            IEssentialsService essentialsService,
            IDeviceVerifier deviceVerifier,
            IHttpDataService httpDataService,
            ILoggerService loggerService
            )
        {
            _sendEventLogStateRepository = sendEventLogStateRepository;
            _eventLogRepository = eventLogRepository;
            _essentialsService = essentialsService;
            _deviceVerifier = deviceVerifier;
            _httpDataService = httpDataService;
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
                List<EventLog> eventLogList = await _eventLogRepository.GetLogsAsync(maxSize);
                if (eventLogList.Count == 0)
                {
                    _loggerService.Info($"Event-logs not found.");
                    return;
                }

                IDictionary<string, SendEventLogState> eventStateDict = new Dictionary<string, SendEventLogState>();
                foreach (var eventType in EventType.All)
                {
                    eventStateDict[eventType.ToString()] = _sendEventLogStateRepository.GetSendEventLogState(eventType);
                }

                foreach (var eventLog in eventLogList)
                {
                    if (eventStateDict[eventLog.GetEventType()] != SendEventLogState.Enable)
                    {
                        eventLog.HasConsent = false;
                    }
                }

                string idempotencyKey = Guid.NewGuid().ToString();
                List<EventLog> agreedEventLogList
                    = eventLogList.Where(eventLog => eventLog.HasConsent).ToList();

                if (agreedEventLogList.Count == 0)
                {
                    _loggerService.Info($"Agreed event-logs not found.");
                    return;
                }

                PolicyResult<bool> policyResult = await Policy
                    .HandleResult<bool>(result => !result)
                    .WaitAndRetryAsync(maxRetry, retryAttempt => {
                        double delay = Math.Pow(2, retryAttempt - 1);
                        _loggerService.Warning($"Event log send failed. retryAttempt:{retryAttempt} delay:{delay}sec");
                        return TimeSpan.FromSeconds(delay);
                    })
                    .ExecuteAndCaptureAsync(() => SendAsync(idempotencyKey, agreedEventLogList));

                if (policyResult.Outcome == OutcomeType.Failure)
                {
                    _loggerService.Error("Event log send failed all.");
                    return;
                }

                _loggerService.Info($"Event log send successful.");

                _loggerService.Info($"Clean up...");
                foreach (var eventLog in eventLogList)
                {
                    await _eventLogRepository.RemoveAsync(eventLog);
                }

                _loggerService.Info($"Done.");
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        private async Task<bool> SendAsync(string idempotencyKey, List<EventLog> eventLogList)
        {
            _loggerService.StartMethod();

            try
            {
                var request = new V1EventLogRequest()
                {
                    IdempotencyKey = idempotencyKey,
                    Platform = _essentialsService.Platform.ToLowerInvariant(),
                    AppPackageName = _essentialsService.AppPackageName,
                    EventLogs = eventLogList,
                };

                // Create device verification payload
                PolicyResult<string> policyResult = await Policy
                    .HandleResult<string>(result => _deviceVerifier.IsErrorPayload(result))
                    .WaitAndRetryAsync(3, retryAttempt => {
                        double delay = Math.Pow(2, retryAttempt + 1);
                        _loggerService.Warning($"Payload creation failed. retryAttempt:{retryAttempt} delay:{delay}sec");
                        return TimeSpan.FromSeconds(delay);
                    })
                    .ExecuteAndCaptureAsync(() => _deviceVerifier.VerifyAsync(request));

                if (policyResult.Outcome == OutcomeType.Failure)
                {
                    _loggerService.Error("Payload creation failed all.");
                    return false;
                }

                _loggerService.Info("Payload creation successful.");
                request.DeviceVerificationPayload = policyResult.Result;

                ApiResponse<string> response = await _httpDataService.PutEventLog(request);
                _loggerService.Info($"PutEventLog() StatusCode:{response.StatusCode}");

                if (response.StatusCode == (int)HttpStatusCode.Created)
                {
                    _loggerService.Info("Send event log succeeded");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.Exception("Exception occurred, SendAsync", ex);
            }
            finally
            {
                _loggerService.EndMethod();
            }

            _loggerService.Error("Send event log failure");
            return false;
        }
    }
}
