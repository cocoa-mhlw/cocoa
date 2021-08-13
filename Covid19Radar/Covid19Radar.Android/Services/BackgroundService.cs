using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using Chino;
using CommonServiceLocator;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Java.Util.Concurrent;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services
{
    public class BackgroundService : IBackgroundService
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
            ILoggerService loggerService
            )
        {
            _loggerService = loggerService;
        }

        public void ScheduleExposureDetection()
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
        private const string DIAGNOSIS_KEYS_DIR = "diagnosis_keys";

        private readonly IDiagnosisKeyRepository _diagnosisKeyRepository
            = ServiceLocator.Current.GetInstance<IDiagnosisKeyRepository>();

        private readonly AbsExposureNotificationApiService _exposureNotificationApiService
            = ServiceLocator.Current.GetInstance<AbsExposureNotificationApiService>();

        private readonly ILoggerService _loggerService
            = ServiceLocator.Current.GetInstance<ILoggerService>();

        private readonly ExposureConfiguration _exposureConfiguration = new ExposureConfiguration();

        private readonly IList<ServerConfiguration> _serverConfigurations = AppSettings.Instance.SupportedRegions.Select(
                    region => new ServerConfiguration()
                    {
                        ApiEndpoint = $"{AppSettings.Instance.CdnUrlBase}/{AppSettings.Instance.BlobStorageContainerName}",
                        Region = region
                    }).ToList();

        private string _diagnosisKeysDir;

        public BackgroundWorker(Context context, WorkerParameters workerParameters)
            : base(context, workerParameters)
        {
        }

        private void PrepareDirs()
        {
            var appDir = FileSystem.AppDataDirectory;

            _diagnosisKeysDir = Path.Combine(appDir, DIAGNOSIS_KEYS_DIR);
            if (!Directory.Exists(_diagnosisKeysDir))
            {
                Directory.CreateDirectory(_diagnosisKeysDir);
            }
        }

        public override Result DoWork()
        {
            _loggerService.StartMethod();

            PrepareDirs();

            if (!_exposureNotificationApiService.IsEnabledAsync().GetAwaiter().GetResult())
            {
                _loggerService.Debug($"EN API is not enabled." +
                    $" worker will retry after {BackgroundService.BACKOFF_DELAY_IN_MINUTES} minutes later.");
                return Result.InvokeRetry();
            }

            try
            {
                foreach(var serverConfiguration in _serverConfigurations)
                {
                    var diagnosisKeyEntryList = _diagnosisKeyRepository.GetDiagnosisKeysListAsync(serverConfiguration)
                        .GetAwaiter().GetResult();

                    List<string> downloadedFileNameList = new List<string>();

                    foreach (var diagnosisKeyEntry in diagnosisKeyEntryList)
                    {
                        string filePath = _diagnosisKeyRepository.DownloadDiagnosisKeysAsync(diagnosisKeyEntry, _diagnosisKeysDir)
                            .GetAwaiter().GetResult();

                        _loggerService.Debug($"URL {diagnosisKeyEntry.Url} have been downloaded.");

                        downloadedFileNameList.Add(filePath);
                    }

                    var downloadedFileNames = string.Join("\n", downloadedFileNameList);
                    _loggerService.Debug(downloadedFileNames);

                    _exposureNotificationApiService.ProvideDiagnosisKeysAsync(
                        downloadedFileNameList,
                        _exposureConfiguration
                        ).GetAwaiter().GetResult();
                }

                return Result.InvokeSuccess();
            }
            catch (IOException exception)
            {
                _loggerService.Exception("IOException", exception);
                return Result.InvokeRetry();
            }
            catch (ENException exception)
            {
                _loggerService.Exception("ENException", exception);
                return Result.InvokeFailure();
            }
            catch (Exception exception)
            {
                _loggerService.Exception("Exception", exception);
                return Result.InvokeFailure();
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public override void OnStopped() {
            base.OnStopped();

            _loggerService.Warning("OnStopped");
        }

    }
}
