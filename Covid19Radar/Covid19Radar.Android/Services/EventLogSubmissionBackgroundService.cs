// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using Covid19Radar.Common;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Java.Util.Concurrent;
using Prism.Ioc;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services
{
    public class EventLogSubmissionBackgroundService : AbsEventLogSubmissionBackgroundService
    {
        private const string CURRENT_WORK_NAME = "eventlog_submission_worker_20220112";

        private static readonly long INTERVAL_IN_HOURS = 24;
        private static readonly long BACKOFF_DELAY_IN_MINUTES = 60;

        private readonly ILoggerService _loggerService;

        public EventLogSubmissionBackgroundService(
            ILoggerService loggerService
            ) : base()
        {
            _loggerService = loggerService;
        }

        public override void Schedule()
        {
            _loggerService.StartMethod();

            WorkManager workManager = WorkManager.GetInstance(Platform.AppContext);

            PeriodicWorkRequest periodicWorkRequest = CreatePeriodicWorkRequest();
            workManager.EnqueueUniquePeriodicWork(
                CURRENT_WORK_NAME,
                ExistingPeriodicWorkPolicy.Replace,
                periodicWorkRequest
                );

            _loggerService.EndMethod();
        }

        private static PeriodicWorkRequest CreatePeriodicWorkRequest()
        {
            var workRequestBuilder = new PeriodicWorkRequest.Builder(
                typeof(BackgroundWorker),
                INTERVAL_IN_HOURS, TimeUnit.Hours
                )
                .SetConstraints(new Constraints.Builder()
                   .SetRequiresBatteryNotLow(true)
                   .SetRequiredNetworkType(NetworkType.Connected)
                   .Build())
                .SetBackoffCriteria(BackoffPolicy.Linear, BACKOFF_DELAY_IN_MINUTES, TimeUnit.Minutes);
            return workRequestBuilder.Build();
        }

        [Preserve]
        public class BackgroundWorker : Worker
        {
            private Lazy<ILoggerService> _loggerService => new Lazy<ILoggerService>(() => ContainerLocator.Current.Resolve<ILoggerService>());
            private Lazy<IEventLogService> _eventLogService => new Lazy<IEventLogService>(() => ContainerLocator.Current.Resolve<IEventLogService>());

            public BackgroundWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
            {
                // do nothing
            }

            public override Result DoWork()
            {
                var loggerService = _loggerService.Value;
                var eventLogService = _eventLogService.Value;

                loggerService.StartMethod();

                try
                {
                    eventLogService.SendAllAsync(AppConstants.MAX_LOG_REQUEST_SIZE_IN_BYTES, AppConstants.MAX_RETRY);
                    return Result.InvokeSuccess();
                }
                catch(Exception exception)
                {
                    loggerService.Exception("Exception occurred, SendAllAsync", exception);
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
