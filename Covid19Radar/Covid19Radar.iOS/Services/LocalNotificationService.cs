/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using System.Threading.Tasks;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using UserNotifications;

namespace Covid19Radar.iOS.Services
{
    public class LocalNotificationService : ILocalNotificationService
    {
        private const string NOTIFICATION_ID = "notification_id_cocoa";

        private readonly ILoggerService _loggerService;

        public LocalNotificationService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task PrepareAsync()
        {
            _loggerService.StartMethod();

            AskPermissionForUserNotification();

            _loggerService.EndMethod();
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public async Task ShowExposureNotificationAsync()
        {
            _loggerService.StartMethod();

            await ScheduleLocalNotificationAsync();

            _loggerService.EndMethod();
        }

        private void AskPermissionForUserNotification()
        {
            _loggerService.StartMethod();

            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (granted, error) =>
            {
                _loggerService.Info($"Ask permission for user notification: {granted}");
            });

            _loggerService.EndMethod();
        }

        private async Task ScheduleLocalNotificationAsync()
        {
            _loggerService.StartMethod();

            try
            {
                var settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync();
                if (settings.AuthorizationStatus != UNAuthorizationStatus.Authorized)
                {
                    throw new Exception($"UserNotification is not authorized: {settings.AuthorizationStatus}");
                }

                var content = new UNMutableNotificationContent();

                content.Title = AppResources.LocalExposureNotificationTitle;
                content.Body = AppResources.LocalExposureNotificationContent;

                var request = UNNotificationRequest.FromIdentifier(NOTIFICATION_ID, content, null);
                var notificationCenter = UNUserNotificationCenter.Current;
                await notificationCenter.AddNotificationRequestAsync(request);
            }
            catch (Exception e)
            {
                _loggerService.Exception("Exception occurred", e);
            }

            _loggerService.EndMethod();
        }

        public async Task DismissExposureNotificationAsync()
        {
            _loggerService.StartMethod();

            var notifications = await UNUserNotificationCenter.Current.GetDeliveredNotificationsAsync();
            var targetNotifications = notifications
                .Select(notification => notification.Request.Identifier)
                .Where(identifier => identifier == NOTIFICATION_ID)
                .ToArray();

            if (targetNotifications.Count() != 0)
            {
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(targetNotifications);
            }

            _loggerService.EndMethod();
        }
    }
}
