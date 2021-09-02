using System.Threading.Tasks;
using AndroidX.Work;

namespace Xamarin.ExposureNotifications
{
    public static partial class ExposureNotification
    {
        // Current work-name.
        private static readonly string CurrentWorkName = "cocoaexposurenotification-202107";

        // Schedule background work (Customization by COCOA)
        static Task PlatformScheduleFetch()
        {
            var workRequest = CreatePeriodicWorkRequest();
            EnqueueUniquePeriodicWork(workRequest);

            return Task.CompletedTask;
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
