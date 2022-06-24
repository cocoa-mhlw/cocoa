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
        private static readonly string BGTASK_IDENTIFIER = AppInfo.PackageName + ".eventlog-submission";

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
            _loggerService.StartMethod();

            var result = BGTaskScheduler.Shared.Register(BGTASK_IDENTIFIER, null, task =>
            {
                _loggerService.Info("Background task has been started.");

                ScheduleBgTask();

                var cancellationTokenSource = new CancellationTokenSource();
                task.ExpirationHandler = cancellationTokenSource.Cancel;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _eventLogService.SendAllAsync(
                            AppConstants.EventLogMaxRequestSizeInBytes,
                            AppConstants.EventLogMaxRetry);
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
                    }
                }, cancellationTokenSource.Token);
            });

            ScheduleBgTask();

            _loggerService.EndMethod();
        }

        private void ScheduleBgTask()
        {
            _loggerService.StartMethod();

            try
            {
                BGProcessingTaskRequest bgTaskRequest = new BGProcessingTaskRequest(BGTASK_IDENTIFIER)
                {
                    RequiresNetworkConnectivity = true,
                    EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(ONE_DAY_IN_SECONDS)
                };

                BGTaskScheduler.Shared.Submit(bgTaskRequest, out var error);
                if (error != null)
                {
                    NSErrorException exception = new NSErrorException(error);
                    _loggerService.Exception("BGTaskScheduler submit failed.", exception);
                    throw exception;
                }
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}
