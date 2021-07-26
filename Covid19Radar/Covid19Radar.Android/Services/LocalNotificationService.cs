/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using AndroidX.Core.App;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services
{
    public class LocalNotificationService : ILocalNotificationService
    {
        private const int NOTIFICATION_ID_EXPOSURE = 0x1234;

        public void ShowExposureNotification()
        {
            var notification = new NotificationCompat
                .Builder(Platform.AppContext, MainApplication.NOTIFICATION_CHANNEL_ID)
                .SetSmallIcon(Resource.Drawable.ic_notification)
                .SetContentTitle(AppResources.AndroidNotificationTitle)
                .SetContentText(AppResources.AndroidNotificationText)
                .SetVisibility(NotificationCompat.VisibilitySecret)
                .Build();

            var nm = NotificationManagerCompat.From(Platform.AppContext);
            nm.Notify(NOTIFICATION_ID_EXPOSURE, notification);
        }
    }
}
