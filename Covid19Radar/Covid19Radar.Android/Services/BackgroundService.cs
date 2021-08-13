using System;
using System.IO;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using Chino;
using CommonServiceLocator;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Java.Util.Concurrent;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services
{
    public class BackgroundService : AbsBackgroundService
    {
#if DEBUG
        internal const int INTERVAL_IN_MINUTES = 16;
        internal const int BACKOFF_DELAY_IN_MINUTES = 3;
#else
        internal const int INTERVAL_IN_MINUTES = 4 * 60;
        internal const int BACKOFF_DELAY_IN_MINUTES = 1 * 60;
#endif
        internal const string CURRENT_WORK_NAME = "cappuccino_worker";

        internal static string[] OLD_WORK_NAMES = {
            "exposurenotification",
            "cocoaexposurenotification",
        };

        private readonly ILoggerService _loggerService;

        public BackgroundService(
            IDiagnosisKeyRepository diagnosisKeyRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository
            ) : base(
                diagnosisKeyRepository,
                exposureNotificationApiService,
                loggerService,
                userDataRepository
                )
        {
            _loggerService = loggerService;
        }

        public override void ScheduleExposureDetection()
        {
            _loggerService.StartMethod();

            WorkManager workManager = WorkManager.GetInstance(Platform.AppContext);

            CancelOldWorks(workManager);

            PeriodicWorkRequest periodicWorkRequest = CreatePeriodicWorkRequest();
            workManager.EnqueueUniquePeriodicWork(
                CURRENT_WORK_NAME,
                ExistingPeriodicWorkPolicy.Keep,
                periodicWorkRequest
                );

            _loggerService.EndMethod();
        }

        private static void CancelOldWorks(WorkManager workManager)
        {
            foreach (var oldWorkName in OLD_WORK_NAMES)
            {
                workManager.CancelUniqueWork(oldWorkName);
            }
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
            = new Lazy<AbsExposureNotificationApiService>(() => ServiceLocator.Current.GetInstance<AbsExposureNotificationApiService>());

        private readonly Lazy<ILoggerService> _loggerService
            = new Lazy<ILoggerService>(() => ServiceLocator.Current.GetInstance<ILoggerService>());

        private readonly Lazy<AbsBackgroundService> _backgroundService
            = new Lazy<AbsBackgroundService>(() => ServiceLocator.Current.GetInstance<AbsBackgroundService>());

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
                    $" worker will retry after {BackgroundService.BACKOFF_DELAY_IN_MINUTES} minutes later.");
                return Result.InvokeRetry();
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

        public override void OnStopped() {
            base.OnStopped();

            _loggerService.Value.Warning("OnStopped");
        }

    }
}
