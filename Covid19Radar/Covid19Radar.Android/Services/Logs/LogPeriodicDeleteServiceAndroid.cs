using System;
using Android.App;
using Android.Content;
using Covid19Radar.Droid.Services.Logs;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(LogPeriodicDeleteServiceAndroid))]
namespace Covid19Radar.Droid.Services.Logs
{
    public class LogPeriodicDeleteServiceAndroid : ILogPeriodicDeleteService
    {
        private static readonly int requestCode = 1000;
        private static readonly long executionIntervalMillis = 60 * 60 * 24 * 1000; // 24hours

        private readonly ILoggerService loggerService;

        public LogPeriodicDeleteServiceAndroid()
        {
            // loggerService = DependencyService.Resolve<ILoggerService>();
            // App から直接参照する
            loggerService = App.LoggerService;
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
        private readonly ILoggerService loggerService;
        private readonly ILogFileService logFileService;

        public LogPeriodicDeleteReceiver()
        {
            var essensialService = new EssentialsService();
            var logPathService = new LogPathService(new LogPathServiceAndroid());
            loggerService = new LoggerService(logPathService, essensialService);
            logFileService = new Covid19Radar.Services.Logs.LogFileService(loggerService, logPathService);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                loggerService.Info($"Action: {intent.Action}");
                logFileService.Rotate();
                loggerService.Info("Periodic deletion of old logs.");
                var nextScheduledTime = LogPeriodicDeleteServiceAndroid.SetNextSchedule();
                loggerService.Info($"Next scheduled time: {DateTimeOffset.FromUnixTimeMilliseconds(nextScheduledTime).ToOffset(new TimeSpan(9, 0, 0))}");
            }
            catch
            {
                // do nothing
            }
        }
    }
}
