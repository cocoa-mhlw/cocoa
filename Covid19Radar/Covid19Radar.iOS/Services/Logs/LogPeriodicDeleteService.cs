/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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

        private static readonly string identifier = AppInfo.PackageName + ".delete-old-logs";

        #endregion

        #region Instance Fields

        private readonly ILogFileService logFileService;

        #endregion

        #region Constructors

        public LogPeriodicDeleteService(
            ILoggerService loggerService,
            ILogFileService logFileService
            ) : base(loggerService)
        {
            this.logFileService = logFileService;
        }

        #endregion

        #region ILogPeriodicDeleteService Methods

        public override void Schedule()
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
            try
            {
                loggerService.StartMethod();

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

                loggerService.EndMethod();
            }
            catch
            {
                // do nothing
            }
        }

        private void ScheduleAppRefresh()
        {
            loggerService.StartMethod();

            var request = new BGAppRefreshTaskRequest(identifier);
            request.EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(ONE_DAY_IN_SECONDS); // Fetch no earlier than 1 day from now

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
            base.Main();

            try
            {
                loggerService.StartMethod();

                logFileService.Rotate();

                loggerService.Info("Periodic deletion of old logs.");
                loggerService.EndMethod();
            }
            catch
            {
                // do nothing
            }
        }

        #endregion
    }
}
