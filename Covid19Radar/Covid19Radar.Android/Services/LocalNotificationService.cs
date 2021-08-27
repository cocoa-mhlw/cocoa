/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services
{
    public class LocalNotificationService : ILocalNotificationService
    {
        private const string NOTIFICATION_CHANNEL_ID = "notification_channel_cocoa_202107";

        private const int REQUEST_CODE = 0x01;

        private const int NOTIFICATION_ID_EXPOSURE = 0x1234;

        private readonly ILoggerService _loggerService;

        public LocalNotificationService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task PrepareAsync()
        {
            _loggerService.StartMethod();

            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                _loggerService.Info($"Platform version {Build.VERSION.SdkInt} is not supported NotificationChannel.");
                _loggerService.EndMethod();
                return;
            }

            NotificationChannelCompat notificationChannel = new NotificationChannelCompat
                .Builder(NOTIFICATION_CHANNEL_ID, NotificationManagerCompat.ImportanceDefault)
                .SetName(AppResources.AndroidNotificationChannelName)
                .SetShowBadge(false)
                .Build();

            var nm = NotificationManagerCompat.From(Platform.AppContext);
            nm.CreateNotificationChannel(notificationChannel);

            _loggerService.Info($"NotificationChannel created.");
            _loggerService.EndMethod();
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task ShowExposureNotificationAsync()
        {
            _loggerService.StartMethod();

            Intent intent = MainActivity.NewIntent(Platform.AppContext, Destination.ContactedNotifyPage);
            PendingIntent pendingIntent = PendingIntent.GetActivity(
                Platform.AppContext,
                REQUEST_CODE,
                intent,
                PendingIntentFlags.UpdateCurrent
                );

            var notification = new NotificationCompat
                .Builder(Platform.AppContext, NOTIFICATION_CHANNEL_ID)
                .SetStyle(new NotificationCompat.BigTextStyle())
                .SetSmallIcon(Resource.Drawable.ic_notification)
                .SetContentTitle(AppResources.LocalExposureNotificationTitle)
                .SetContentText(AppResources.LocalExposureNotificationContent)
                .SetVisibility(NotificationCompat.VisibilitySecret)
                .SetContentIntent(pendingIntent)
                .SetLocalOnly(true)
                .SetAutoCancel(true)
                .Build();

            var nm = NotificationManagerCompat.From(Platform.AppContext);
            nm.Notify(NOTIFICATION_ID_EXPOSURE, notification);

            _loggerService.Info("Local exposure notification shown.");
            _loggerService.EndMethod();
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
