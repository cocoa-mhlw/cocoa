using System.Threading.Tasks;
using AndroidX.Work;

namespace Xamarin.ExposureNotifications
{
    public static partial class ExposureNotification
    {
        private static readonly string[] OldWorkNames = { "exposurenotification" }; // Array of old work-name.
        private static readonly string CurrentWorkName = "cocoaexposurenotification"; // Current work-name. (changed policy from `replace` to `keep`)

        // Schedule background work (Customization by COCOA)
        static Task PlatformScheduleFetch()
        {
            CancelOldWork();

            var workRequest = CreatePeriodicWorkRequest();
            EnqueueUniquePeriodicWork(workRequest);

            return Task.CompletedTask;
        }

        private static void CancelOldWork()
        {
            var workManager = WorkManager.GetInstance(Essentials.Platform.AppContext);
            foreach (var oldWorkName in OldWorkNames)
            {
                workManager.CancelUniqueWork(oldWorkName);
            }
        }

        private static PeriodicWorkRequest CreatePeriodicWorkRequest()
        {
            var workRequestBuilder = new PeriodicWorkRequest.Builder(
                typeof(BackgroundFetchWorker),
                bgRepeatInterval);
            bgRequestBuilder.Invoke(workRequestBuilder);
            return workRequestBuilder.Build();
        }

        private static void EnqueueUniquePeriodicWork(PeriodicWorkRequest workRequest)
        {
            var workManager = WorkManager.GetInstance(Essentials.Platform.AppContext);
            workManager.EnqueueUniquePeriodicWork(CurrentWorkName,
                ExistingPeriodicWorkPolicy.Keep,
                workRequest);
        }
    }
}
