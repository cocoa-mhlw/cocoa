/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Java.Util.Concurrent;
using Prism.Ioc;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services.Logs
{
    public class LogPeriodicDeleteService : AbsLogPeriodicDeleteService
    {
        private const string CURRENT_WORK_NAME = "log_periodic_delete_worker_20220622";

        private static readonly long INTERVAL_IN_HOURS = 24;
        private static readonly long BACKOFF_DELAY_IN_MINUTES = 60;

        public LogPeriodicDeleteService(
            ILoggerService loggerService
            ) : base(loggerService)
        {
        }

        public override void Schedule()
        {
            loggerService.StartMethod();

            WorkManager workManager = WorkManager.GetInstance(Platform.AppContext);

            PeriodicWorkRequest periodicWorkRequest = CreatePeriodicWorkRequest();
            workManager.EnqueueUniquePeriodicWork(
                CURRENT_WORK_NAME,
                ExistingPeriodicWorkPolicy.Replace,
                periodicWorkRequest
                );

            loggerService.EndMethod();
        }

        private static PeriodicWorkRequest CreatePeriodicWorkRequest()
        {
            var workRequestBuilder = new PeriodicWorkRequest.Builder(
                typeof(BackgroundWorker),
                INTERVAL_IN_HOURS, TimeUnit.Hours
                )
                .SetConstraints(new Constraints.Builder()
                   .SetRequiresBatteryNotLow(true)
                   .Build())
                .SetBackoffCriteria(BackoffPolicy.Linear, BACKOFF_DELAY_IN_MINUTES, TimeUnit.Minutes);
            return workRequestBuilder.Build();
        }
    }

    [Preserve]
    public class BackgroundWorker : Worker
    {
        private Lazy<ILoggerService> _loggerService => new Lazy<ILoggerService>(() => ContainerLocator.Current.Resolve<ILoggerService>());
        private Lazy<ILogFileService> _logFileService => new Lazy<ILogFileService>(() => ContainerLocator.Current.Resolve<ILogFileService>());

        public BackgroundWorker(Context context, WorkerParameters workerParameters)
            : base(context, workerParameters)
        {
        }

        public override Result DoWork()
        {
            var loggerService = _loggerService.Value;
            var logFileService = _logFileService.Value;

            loggerService.StartMethod();

            try
            {
                logFileService.Rotate();
                loggerService.Info("Success: Periodic deletion of old logs.");

                return Result.InvokeSuccess();
            }
            catch (IOException exception)
            {
                loggerService.Exception("IOException", exception);
                return Result.InvokeRetry();
            }
            catch (Exception exception)
            {
                loggerService.Exception("Exception", exception);
                return Result.InvokeFailure();
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        public override void OnStopped()
        {
            base.OnStopped();

            _loggerService.Value.Warning("OnStopped");
        }

    }
}
