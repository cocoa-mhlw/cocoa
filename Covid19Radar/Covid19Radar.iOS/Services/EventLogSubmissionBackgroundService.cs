﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using System.Threading.Tasks;
using BackgroundTasks;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Foundation;
using Xamarin.Essentials;

namespace Covid19Radar.iOS.Services
{
    public class EventLogSubmissionBackgroundService : AbsEventLogSubmissionBackgroundService
    {
        private const int TASK_INTERVAL_IN_HOURS = 24;
        private static readonly string BGTASK_IDENTIFIER = AppInfo.PackageName + ".eventlog-submission";

        private readonly IEventLogService _eventLogService;
        private readonly IEventLogRepository _eventLogRepository;
        private readonly ILoggerService _loggerService;
        private readonly IDateTimeUtility _dateTimeUtility;

        public EventLogSubmissionBackgroundService(
            IEventLogService eventLogService,
            IEventLogRepository eventLogRepository,
            ILoggerService loggerService,
            IDateTimeUtility dateTimeUtility
            ) : base()
        {
            _eventLogService = eventLogService;
            _eventLogRepository = eventLogRepository;
            _loggerService = loggerService;
            _dateTimeUtility = dateTimeUtility;
        }

        public override void Schedule()
        {
            _loggerService.StartMethod();

            bool result = BGTaskScheduler.Shared.Register(BGTASK_IDENTIFIER, null, task =>
            {
                _loggerService.Info("Background task has been started.");

                DateTime nextDateTime = _dateTimeUtility.UtcNow.Date.AddHours(TASK_INTERVAL_IN_HOURS);
                ScheduleBgTask(nextDateTime);

                var cancellationTokenSource = new CancellationTokenSource();
                task.ExpirationHandler = cancellationTokenSource.Cancel;

                _ = Task.Run(async () =>
                {
                    _loggerService.Info("Task.Run() start");

                    try
                    {
                        await _eventLogRepository.RotateAsync(
                            AppConstants.EventLogFileExpiredSeconds);

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
                        _loggerService.Info("Task.Run() end");
                    }
                }, cancellationTokenSource.Token);
            });

            ScheduleBgTask(_dateTimeUtility.UtcNow);

            if (result)
            {
                _loggerService.Info("BGTaskScheduler.Shared.Register succeeded.");
            }
            else
            {
                _loggerService.Error("BGTaskScheduler.Shared.Register failed.");
            }

            _loggerService.EndMethod();
        }

        private void ScheduleBgTask(DateTime nextDateTime)
        {
            _loggerService.StartMethod();

            try
            {
                var bgTaskRequest = new BGProcessingTaskRequest(BGTASK_IDENTIFIER)
                {
                    EarliestBeginDate = NSDate.FromTimeIntervalSince1970(nextDateTime.ToUnixEpoch()),
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
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}

