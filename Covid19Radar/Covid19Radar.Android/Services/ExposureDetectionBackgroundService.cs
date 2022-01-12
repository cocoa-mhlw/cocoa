/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Java.Util.Concurrent;
using Prism.Ioc;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services
{
    public class ExposureDetectionBackgroundService : AbsExposureDetectionBackgroundService
    {
        internal const int INTERVAL_IN_MINUTES = 4 * 60;
        internal const int BACKOFF_DELAY_IN_MINUTES = 1 * 60;

        internal const string CURRENT_WORK_NAME = "cappuccino_worker";

        private readonly ILoggerService _loggerService;

        public ExposureDetectionBackgroundService(
            IDiagnosisKeyRepository diagnosisKeyRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IServerConfigurationRepository serverConfigurationRepository,
            ILocalPathService localPathService,
            IDateTimeUtility dateTimeUtility
            ) : base(
                diagnosisKeyRepository,
                exposureNotificationApiService,
                exposureConfigurationRepository,
                loggerService,
                userDataRepository,
                serverConfigurationRepository,
                localPathService,
                dateTimeUtility
                )
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
                ExistingPeriodicWorkPolicy.Keep,
                periodicWorkRequest
                );

            _loggerService.EndMethod();
        }

        private static PeriodicWorkRequest CreatePeriodicWorkRequest()
        {
            var workRequestBuilder = new PeriodicWorkRequest.Builder(
                typeof(BackgroundWorker),
                INTERVAL_IN_MINUTES, TimeUnit.Minutes)
                .SetConstraints(new Constraints.Builder()
                   .SetRequiresBatteryNotLow(true)
                   .SetRequiredNetworkType(NetworkType.Connected)
                   .Build())
                .SetBackoffCriteria(BackoffPolicy.Linear, BACKOFF_DELAY_IN_MINUTES, TimeUnit.Minutes);
            return workRequestBuilder.Build();
        }
    }

    [Preserve]
    public class BackgroundWorker : Worker
    {
        private readonly Lazy<AbsExposureNotificationApiService> _exposureNotificationApiService
            = new Lazy<AbsExposureNotificationApiService>(() => ContainerLocator.Current.Resolve<AbsExposureNotificationApiService>());

        private readonly Lazy<ILoggerService> _loggerService
            = new Lazy<ILoggerService>(() => ContainerLocator.Current.Resolve<ILoggerService>());

        private readonly Lazy<AbsExposureDetectionBackgroundService> _backgroundService
            = new Lazy<AbsExposureDetectionBackgroundService>(() => ContainerLocator.Current.Resolve<AbsExposureDetectionBackgroundService>());

        public BackgroundWorker(Context context, WorkerParameters workerParameters)
            : base(context, workerParameters)
        {
        }

        public override Result DoWork()
        {
            var exposureNotificationApiService = _exposureNotificationApiService.Value;
            var loggerService = _loggerService.Value;
            var backgroundService = _backgroundService.Value;

            loggerService.StartMethod();

            if (!exposureNotificationApiService.IsEnabledAsync().GetAwaiter().GetResult())
            {
                loggerService.Debug($"EN API is not enabled." +
                    $" worker will start after {ExposureDetectionBackgroundService.INTERVAL_IN_MINUTES} minutes later.");
                return Result.InvokeSuccess();
            }

            try
            {
                backgroundService.ExposureDetectionAsync().GetAwaiter().GetResult();
                return Result.InvokeSuccess();
            }
            catch (IOException exception)
            {
                loggerService.Exception("IOException", exception);
                return Result.InvokeRetry();
            }
            catch (ENException exception)
            {
                loggerService.Exception("ENException", exception);
                return Result.InvokeFailure();
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
