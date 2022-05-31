// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using System.Threading.Tasks;
using BackgroundTasks;
using Covid19Radar.Common;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Foundation;
using Xamarin.Essentials;

namespace Covid19Radar.iOS.Services
{
    public class EventLogSubmissionBackgroundService : AbsEventLogSubmissionBackgroundService
    {
        private static readonly string IDENTIFIER = AppInfo.PackageName + ".eventlog-submission";

        private const double ONE_DAY_IN_SECONDS = 1 * 24 * 60 * 60;

        private readonly IEventLogService _eventLogService;
        private readonly ILoggerService _loggerService;

        public EventLogSubmissionBackgroundService(
            IEventLogService eventLogService,
            ILoggerService loggerService
            ) : base()
        {
            _eventLogService = eventLogService;
            _loggerService = loggerService;
        }

        public override void Schedule()
        {
            _ = BGTaskScheduler.Shared.Register(IDENTIFIER, null, task =>
            {
                HandleSendLogAsync((BGAppRefreshTask)task);
            });

            ScheduleSendEventLog();

            _loggerService.EndMethod();
        }

        private void HandleSendLogAsync(BGAppRefreshTask task)
        {
            _loggerService.StartMethod();

            ScheduleSendEventLog();

            var cancellationTokenSource = new CancellationTokenSource();
            task.ExpirationHandler = cancellationTokenSource.Cancel;

            _ = Task.Run(async () =>
            {
                try
                {
                    await _eventLogService.SendAllAsync(AppConstants.MAX_LOG_REQUEST_SIZE_IN_BYTES, AppConstants.MAX_RETRY);
                    task.SetTaskCompleted(true);
                }
                catch (OperationCanceledException exception)
                {
                    _loggerService.Exception($"Background task canceled.", exception);
                    task.SetTaskCompleted(false);
                }
                catch (Exception exception)
                {
                    _loggerService.Exception($"Exception", exception);
                    task.SetTaskCompleted(false);
                }
                finally
                {
                    cancellationTokenSource.Dispose();
                    _loggerService.EndMethod();
                }
            });
        }

        private void ScheduleSendEventLog()
        {
            _loggerService.StartMethod();

            var bgTaskRequest = new BGProcessingTaskRequest(IDENTIFIER)
            {
                EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(ONE_DAY_IN_SECONDS),
                RequiresNetworkConnectivity = true
            };

            _loggerService.Info($"request.EarliestBeginDate: {bgTaskRequest.EarliestBeginDate}");

            _ = BGTaskScheduler.Shared.Submit(bgTaskRequest, out var error);
            if (error != null)
            {
                NSErrorException exception = new NSErrorException(error);
                _loggerService.Exception("BGTaskScheduler submit failed.", exception);
                throw exception;
            }

            _loggerService.EndMethod();
        }
    }
}
