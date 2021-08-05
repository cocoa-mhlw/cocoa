/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Android.App;
using Android.Content;
using CommonServiceLocator;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services.Logs
{
    public class LogPeriodicDeleteServiceAndroid : ILogPeriodicDeleteService
    {
        private static readonly int requestCode = 1000;
        private static readonly long executionIntervalMillis = 60 * 60 * 24 * 1000; // 24hours

        private readonly ILoggerService loggerService;

        public LogPeriodicDeleteServiceAndroid(
            ILoggerService loggerService
            )
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
        private ILoggerService loggerService => ServiceLocator.Current.GetInstance<ILoggerService>();
        private ILogFileService logFileService => ServiceLocator.Current.GetInstance<ILogFileService>();

        public LogPeriodicDeleteReceiver()
        {
            // do nothing
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
