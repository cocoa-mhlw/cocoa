// This Source Code Form is subject to the terms of the Mozilla Public
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
    public class DataMaintainanceBackgroundService : AbsDataMaintainanceBackgroundService
    {
        private const double BGTASK_INTERVAL = 24 * 60 * 60; // one day
        private static readonly string BGTASK_IDENTIFIER = AppInfo.PackageName + ".data-maintainance";

        public DataMaintainanceBackgroundService(
            ILoggerService loggerService,
            ILogFileService logFileService,
            IEventLogRepository eventLogRepository
            ) : base(loggerService, logFileService, eventLogRepository)
        {
        }

        public override void Schedule()
        {
            LoggerService.StartMethod();

            var result = BGTaskScheduler.Shared.Register(BGTASK_IDENTIFIER, null, task =>
            {
                LoggerService.Info("Background task has been started.");

                ScheduleBgTask();

                var cancellationTokenSource = new CancellationTokenSource();
                task.ExpirationHandler = cancellationTokenSource.Cancel;

                _ = Task.Run(async () =>
                {
                    LoggerService.Info("Task.Run() start");

                    try
                    {
                        await ExecuteAsync();
                        task.SetTaskCompleted(true);
                    }
                    catch (OperationCanceledException exception)
                    {
                        LoggerService.Exception($"Background task canceled.", exception);
                        task.SetTaskCompleted(false);
                    }
                    catch (Exception exception)
                    {
                        LoggerService.Exception($"Exception", exception);
                        task.SetTaskCompleted(false);
                    }
                    finally
                    {
                        cancellationTokenSource.Dispose();
                        LoggerService.Info("Task.Run() end");
                    }
                }, cancellationTokenSource.Token);
            });

            if (result)
            {
                LoggerService.Info("BGTaskScheduler.Shared.Register succeeded.");
            }
            else
            {
                LoggerService.Error("BGTaskScheduler.Shared.Register failed.");
            }

            ScheduleBgTask();

            LoggerService.EndMethod();
        }

        private void ScheduleBgTask()
        {
            LoggerService.StartMethod();

            try
            {
                BGProcessingTaskRequest bgTaskRequest = new BGProcessingTaskRequest(BGTASK_IDENTIFIER)
                {
                    EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(BGTASK_INTERVAL),
                };

                LoggerService.Info($"request.EarliestBeginDate: {bgTaskRequest.EarliestBeginDate}");

                BGTaskScheduler.Shared.Submit(bgTaskRequest, out var error);
                if (error != null)
                {
                    NSErrorException exception = new NSErrorException(error);
                    LoggerService.Exception("BGTaskScheduler submit failed.", exception);
                    throw exception;
                }
            }
            finally
            {
                LoggerService.EndMethod();
            }
        }
    }
}

