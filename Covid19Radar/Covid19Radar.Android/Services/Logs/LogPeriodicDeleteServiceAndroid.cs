using System;
using Android.App;
using Android.Content;
using Covid19Radar.Common;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services.Logs
{
    public class LogPeriodicDeleteServiceAndroid : ILogPeriodicDeleteService
    {
        private static readonly int requestCode = 1000;
        private static readonly long executionIntervalMillis = 60 * 60 * 24 * 1000; // 24hours

        private readonly ILoggerService loggerService;

        public LogPeriodicDeleteServiceAndroid(ILoggerService loggerService)
        {
            this.loggerService = loggerService;
        }

        public void Init()
        {
            var nextScheduledTime = SetNextSchedule();
            loggerService.Info($"Next scheduled time: {DateTimeOffset.FromUnixTimeMilliseconds(nextScheduledTime).ToOffset(new TimeSpan(9, 0, 0))}");
        }

        public static long SetNextSchedule()
        {
            var nextScheduledTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + executionIntervalMillis;

            var context = Platform.AppContext;
            var intent = new Intent(context, typeof(LogPeriodicDeleteReceiver));
            var pendingIntent = PendingIntent.GetBroadcast(context, requestCode, intent, PendingIntentFlags.CancelCurrent);

            var alermService = context.GetSystemService(Context.AlarmService) as AlarmManager;
            if (alermService != null)
            {
                alermService.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, nextScheduledTime, pendingIntent);
            }

            return nextScheduledTime;
        }
    }

    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class LogPeriodicDeleteReceiver : BroadcastReceiver
    {
        private ILoggerService LoggerService => (ILoggerService)ContainerLocator.Current.Resolve(typeof(ILoggerService));
        private ILogFileService LogFileService => (ILogFileService)ContainerLocator.Current.Resolve(typeof(ILogFileService));

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                LoggerService.Info($"Action: {intent.Action}");
                LogFileService.Rotate();
                LoggerService.Info("Periodic deletion of old logs.");
                var nextScheduledTime = LogPeriodicDeleteServiceAndroid.SetNextSchedule();
                LoggerService.Info($"Next scheduled time: {DateTimeOffset.FromUnixTimeMilliseconds(nextScheduledTime).ToOffset(new TimeSpan(9, 0, 0))}");
            }
            catch
            {
                // do nothing
            }
        }
    }
}
