using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Covid19Radar.Services;
using Foundation;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

[assembly: Dependency(typeof(Covid19Radar.iOS.Services.NotificationService))]
namespace Covid19Radar.iOS.Services
{
    public class NotificationService : INotificationService
    {
        int messageId = -1;
       // bool hasNotificationsPermission;

        private static event EventHandler _notificationReceived;
        public EventHandler NotificationReceived
        {
            get { return _notificationReceived; }
            set { _notificationReceived = value; }
        }

        public void Initialize()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNAuthorizationOptions types = UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Alert;

                UNUserNotificationCenter.Current.RequestAuthorization(types, (granted, err) =>
                {
                    // Handle approval
                    if (err != null)
                    {
                        System.Diagnostics.Debug.WriteLine(err.LocalizedFailureReason + System.Environment.NewLine + err.LocalizedDescription);
                    }
                    if (granted)
                    {
                    }
                });
            }
        }

        public void ReceiveNotification(string title, string message)
        {
            var args = new NotificationEventArgs()
            {
                Title = title,
                Message = message
            };
            NotificationReceived?.Invoke(null, args);
        }

        public int ScheduleNotification(string title, string message)
        {
            // EARLY OUT: app doesn't have permissions
            /*
            if (!hasNotificationsPermission)
            {
                return -1;
            }
            */

            messageId++;

            var requestID = "notifyKey";
            var content = new UNMutableNotificationContent()
            {
                Title = title,
                Subtitle = title,
                Body = message,
                Sound = UNNotificationSound.Default,
                Badge = 1,
                UserInfo = NSDictionary.FromObjectAndKey(new NSString("notifyValue"), new NSString("notifyKey"))
            };

            // Local notifications can be time or location based
            // Create a time-based trigger, interval is in seconds and must be greater than 0
            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(5, false);
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                    throw new Exception($"Failed to schedule notification: {err}");
                }
            });
            UIApplication.SharedApplication.ApplicationIconBadgeNumber += 1;
            return messageId;
        }
    }
}