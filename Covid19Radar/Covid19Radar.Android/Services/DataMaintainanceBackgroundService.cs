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
    public class DataMaintainanceBackgroundService : AbsDataMaintainanceBackgroundService
    {
        private const string CURRENT_WORK_NAME = "data_maintainance_worker_20220626";

        private static readonly long INTERVAL_IN_HOURS = 24;
        private static readonly long BACKOFF_DELAY_IN_MINUTES = 60;

        public DataMaintainanceBackgroundService(
            ILogFileService logFileService,
            ILoggerService loggerService
            ) : base(logFileService, loggerService)
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
        private Lazy<AbsDataMaintainanceBackgroundService> _dataMaintainanceBackgroundService
            => new Lazy<AbsDataMaintainanceBackgroundService>(() => ContainerLocator.Current.Resolve<AbsDataMaintainanceBackgroundService>());
        private Lazy<ILoggerService> _loggerService => new Lazy<ILoggerService>(() => ContainerLocator.Current.Resolve<ILoggerService>());

        public BackgroundWorker(Context context, WorkerParameters workerParameters)
            : base(context, workerParameters)
        {
        }

        public override Result DoWork()
        {
            var dataMaintainanceBackgroundService = _dataMaintainanceBackgroundService.Value;
            var loggerService = _loggerService.Value;

            loggerService.StartMethod();

            try
            {
                dataMaintainanceBackgroundService.ExecuteAsync().GetAwaiter().GetResult();
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
