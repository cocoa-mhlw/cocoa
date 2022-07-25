// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Java.Util.Concurrent;
using Prism.Ioc;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services
{
    public class DataMaintainanceBackgroundService : AbsDataMaintainanceBackgroundService
    {
        private const string CURRENT_WORK_NAME = "data_maintainance_worker_20220628";

        private const long INTERVAL_IN_HOURS = 24;
        private const long BACKOFF_DELAY_IN_MINUTES = 60;

        public DataMaintainanceBackgroundService(
            ILoggerService loggerService,
            ILogFileService logFileService,
            IEventLogRepository eventLogRepository
            ) : base(loggerService, logFileService, eventLogRepository)
        {
            // do nothing
        }

        public override void Schedule()
        {
            LoggerService.StartMethod();

            WorkManager workManager = WorkManager.GetInstance(Platform.AppContext);

            PeriodicWorkRequest periodicWorkRequest = CreatePeriodicWorkRequest();
            workManager.EnqueueUniquePeriodicWork(
                CURRENT_WORK_NAME,
                ExistingPeriodicWorkPolicy.Keep,
                periodicWorkRequest
                );

            LoggerService.EndMethod();
        }

        private PeriodicWorkRequest CreatePeriodicWorkRequest()
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

        [Preserve]
        public class BackgroundWorker : Worker
        {
            private Lazy<AbsDataMaintainanceBackgroundService> _dataMaintainanceBackgroundService
                => new Lazy<AbsDataMaintainanceBackgroundService>(() => ContainerLocator.Current.Resolve<AbsDataMaintainanceBackgroundService>());
            private Lazy<ILoggerService> _loggerService => new Lazy<ILoggerService>(() => ContainerLocator.Current.Resolve<ILoggerService>());

            public BackgroundWorker(Context context, WorkerParameters workerParameters)
                : base(context, workerParameters)
            {
                // do nothing
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
}

