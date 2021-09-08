/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using BackgroundTasks;
using Covid19Radar.iOS.Services.Logs;
using Covid19Radar.Services.Logs;
using Foundation;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LogPeriodicDeleteServiceIos))]
namespace Covid19Radar.iOS.Services.Logs
{
    public class LogPeriodicDeleteServiceIos : ILogPeriodicDeleteService
    {
        #region Static Fields

        private static readonly string identifier = AppInfo.PackageName + ".delete-old-logs";

        #endregion

        #region Instance Fields

        private readonly ILoggerService loggerService;
        private readonly ILogFileService logFileService;

        #endregion

        #region Constructors

        public LogPeriodicDeleteServiceIos()
        {
            loggerService = DependencyService.Resolve<ILoggerService>();
            logFileService = DependencyService.Resolve<ILogFileService>();
        }

        #endregion

        #region ILogPeriodicDeleteService Methods

        public void Init()
        {
            loggerService.StartMethod();

            _ = BGTaskScheduler.Shared.Register(identifier, null, task =>
              {
                  HandleAppRefresh((BGAppRefreshTask)task);
              });

            ScheduleAppRefresh();

            loggerService.EndMethod();
        }

        #endregion

        #region Other Private Methods

        private void HandleAppRefresh(BGAppRefreshTask task)
        {
            loggerService.StartMethod();
            try
            {

                ScheduleAppRefresh();

                var queue = new NSOperationQueue();
                queue.MaxConcurrentOperationCount = 1;

                task.ExpirationHandler = () =>
                {
                    loggerService.Info("Task expired.");
                    queue.CancelAllOperations();
                };

                var operation = new DeleteOldLogsOperation(loggerService, logFileService);
                operation.CompletionBlock = () =>
                {
                    loggerService.Info($"Operation completed. operation.IsCancelled: {operation.IsCancelled}");
                    task.SetTaskCompleted(!operation.IsCancelled);
                };

                queue.AddOperation(operation);
            }
            catch (Exception e)
            {
                loggerService.Exception("Failed to handle app refresh.", e);
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        private void ScheduleAppRefresh()
        {
            loggerService.StartMethod();

            var oneDay = 1 * 24 * 60 * 60;
            var request = new BGAppRefreshTaskRequest(identifier);
            request.EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(oneDay); // Fetch no earlier than 1 day from now

            loggerService.Info($"request.EarliestBeginDate: {request.EarliestBeginDate}");

            _ = BGTaskScheduler.Shared.Submit(request, out var error);

            if (error != null)
            {
                loggerService.Error($"Could not schedule app refresh. Error: {error}");
            }

            loggerService.EndMethod();
        }

        #endregion
    }

    class DeleteOldLogsOperation : NSOperation
    {
        #region Instance Fields

        private readonly ILoggerService loggerService;
        private readonly ILogFileService logFileService;

        #endregion

        #region Constructors

        public DeleteOldLogsOperation(ILoggerService loggerService, ILogFileService logFileService)
        {
            this.loggerService = loggerService;
            this.logFileService = logFileService;
        }

        #endregion

        #region NSOperation Methods

        public override void Main()
        {
            loggerService.StartMethod();
            base.Main();
            try
            {
                logFileService.Rotate();
                loggerService.Info("Periodic deletion of old logs.");
            }
            catch (Exception e)
            {
                loggerService.Exception("Failed to rotate logs.", e);
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        #endregion
    }
}
