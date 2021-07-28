/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Foundation;
using UserNotifications;

namespace Covid19Radar.iOS.Services
{
    public class LocalNotificationService : ILocalNotificationService
    {
        public void ShowExposureNotification()
        {
            // TODO: iOS側は非同期なのでとりあえずasync/awaitでTaskで囲んでおく
            _ = Task.Run(() => ScheduleLocalNotification());
        }

        private async Task ScheduleLocalNotification()
        {
            try
            {
                var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
                if (settings.AuthorizationStatus != UNAuthorizationStatus.Authorized)
                {
                    // TODO: 通知許諾がされていない場合のエラーハンドリングは握りつぶす？
                    throw new NotImplementedException();
                }

                var content = new UNMutableNotificationContent();

                // TODO: 文言
                content.Title = AppResources.LocalExposureNotificationTitle;
                content.Body = AppResources.LocalExposureNotificationContent;

                // TODO: 発動タイミングは即時で大丈夫か
                var request = UNNotificationRequest.FromIdentifier(new NSUuid().AsString(), content, null);
                var notificationCenter = UNUserNotificationCenter.Current;
                await notificationCenter.AddNotificationRequestAsync(request);
            }
            catch
            {
            }
        }
    }
}