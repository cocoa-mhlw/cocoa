﻿// This Source Code Form is subject to the terms of the Mozilla Public
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
        private const string CURRENT_WORK_NAME = "eventlog_submission_worker_20220628";

        private const long INTERVAL_IN_HOURS = 24;
        private const long BACKOFF_DELAY_IN_MINUTES = 60;

        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly ILoggerService _loggerService;

        public EventLogSubmissionBackgroundService(
            IDateTimeUtility dateTimeUtility,
            ILoggerService loggerService
            ) : base()
        {
            _dateTimeUtility = dateTimeUtility;
            _loggerService = loggerService;
        }

        public override void Schedule()
        {
            _loggerService.StartMethod();

            WorkManager workManager = WorkManager.GetInstance(Platform.AppContext);

            PeriodicWorkRequest periodicWorkRequest = CreatePeriodicWorkRequest();
            workManager.EnqueueUniquePeriodicWork(
                CURRENT_WORK_NAME,
                ExistingPeriodicWorkPolicy.Keep,
                periodicWorkRequest
                );

            _loggerService.EndMethod();
        }

        private PeriodicWorkRequest CreatePeriodicWorkRequest()
        {
            DateTime tommorow = _dateTimeUtility.UtcNow.Date.AddDays(1);
            var interval = tommorow - _dateTimeUtility.UtcNow;

            var workRequestBuilder = new PeriodicWorkRequest.Builder(
                typeof(BackgroundWorker),
                INTERVAL_IN_HOURS, TimeUnit.Hours
                )
                .SetPeriodStartTime((long)interval.TotalSeconds, TimeUnit.Seconds)
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
                    eventLogService.SendAllAsync(
                        AppConstants.EventLogMaxRequestSizeInBytes,
                        AppConstants.EventLogMaxRetry);
                    return Result.InvokeSuccess();
                }
                catch (Exception exception)
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

