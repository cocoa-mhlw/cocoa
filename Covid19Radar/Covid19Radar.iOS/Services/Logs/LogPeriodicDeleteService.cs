/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading;
using System.Threading.Tasks;
using BackgroundTasks;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Foundation;
using Xamarin.Essentials;

namespace Covid19Radar.iOS.Services.Logs
{
    public class LogPeriodicDeleteService : AbsLogPeriodicDeleteService
    {
        #region Constants

        private const double ONE_DAY_IN_SECONDS = 1 * 24 * 60 * 60;

        #endregion

        #region Static Fields

        private static readonly string BGTASK_IDENTIFIER = AppInfo.PackageName + ".delete-old-logs";

        #endregion

        #region Instance Fields

        private readonly ILogFileService _logFileService;

        #endregion

        #region Constructors

        public LogPeriodicDeleteService(
            ILoggerService loggerService,
            ILogFileService logFileService
            ) : base(loggerService)
        {
            _logFileService = logFileService;
        }

        #endregion

        #region ILogPeriodicDeleteService Methods

        public override void Schedule()
        {
            loggerService.StartMethod();

            var result = BGTaskScheduler.Shared.Register(BGTASK_IDENTIFIER, null, task =>
            {
                loggerService.Info("Background task has been started.");

                ScheduleBgTask();

                var cancellationTokenSource = new CancellationTokenSource();
                task.ExpirationHandler = cancellationTokenSource.Cancel;

                _ = Task.Run(() =>
                {
                    try
                    {
                        _logFileService.Rotate();
                        task.SetTaskCompleted(true);
                    }
                    catch (OperationCanceledException exception)
                    {
                        loggerService.Exception($"Background task canceled.", exception);
                        task.SetTaskCompleted(false);
                    }
                    catch (Exception exception)
                    {
                        loggerService.Exception($"Exception", exception);
                        task.SetTaskCompleted(false);
                    }
                    finally
                    {
                        cancellationTokenSource.Dispose();
                    }
                }, cancellationTokenSource.Token);
            });

            if (result)
            {
                loggerService.Debug("BGTaskScheduler.Shared.Register succeeded.");
            }
            else
            {
                loggerService.Info("BGTaskScheduler.Shared.Register failed.");
            }

            ScheduleBgTask();

            loggerService.EndMethod();
        }

        #endregion

        #region Other Private Methods

        private void ScheduleBgTask()
        {
            loggerService.StartMethod();

            try
            {
                BGProcessingTaskRequest bgTaskRequest = new BGProcessingTaskRequest(BGTASK_IDENTIFIER)
                {
                    EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(ONE_DAY_IN_SECONDS)
                };

                BGTaskScheduler.Shared.Submit(bgTaskRequest, out var error);
                if (error != null)
                {
                    NSErrorException exception = new NSErrorException(error);
                    loggerService.Exception("BGTaskScheduler submit failed.", exception);
                    throw exception;
                }
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        #endregion
    }

}
