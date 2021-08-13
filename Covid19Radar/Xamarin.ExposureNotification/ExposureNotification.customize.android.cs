using System.Threading.Tasks;
using AndroidX.Work;

namespace Xamarin.ExposureNotifications
{
    public static partial class ExposureNotification
    {
        private static readonly string[] OldWorkNames = {
            "exposurenotification",
            "cocoaexposurenotification",
        }; // Array of old work-name.

        // Current work-name.
        private static readonly string CurrentWorkName = "cocoaexposurenotification-202107";

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
