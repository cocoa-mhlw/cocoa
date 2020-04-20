using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace Covid19Radar.Droid.BackgroundService
{
    [Service(Name = "net.Kzmx.Covid19Radar.BackgroundService", Process = ":BleProcess")]
    public class BackgroundService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                //Android 8 Oreo
                this.RegisterForegroundService();
            }

            Task task = new Task(() =>
            {
                // Do Back Ground Task
                while (true)
                {
                    System.Threading.Thread.Sleep(10000);
                    System.Diagnostics.Debug.WriteLine("Do Background");
                }
            });
            task.Start();

            return StartCommandResult.Sticky;
        }
        public void StartBackgroundService()
        {
            Intent serviceIntent = new Intent(this, typeof(BackgroundService));
            serviceIntent.AddFlags(ActivityFlags.NewTask);
            serviceIntent.SetPackage(this.PackageManager.GetPackageInfo(this.PackageName, 0).PackageName);
            base.StartService(serviceIntent);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            System.Diagnostics.Debug.WriteLine("OnDestroy");
            this.StartBackgroundService();
        }

        void RegisterForegroundService()
        {
            NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);

            //Andorid8.0 Oreo 以降の通知で必要なChannel
            string channelId = "Covid19RadarNotification";
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                string channelNm = "Covid19Radar";
                NotificationChannel channel = new NotificationChannel(channelId, channelNm, NotificationImportance.Default)
                {
                    Description = "任意の説明"
                };
                manager.CreateNotificationChannel(channel);
            }
            var notification = new Notification.Builder(this)
                    .SetContentTitle(Resources.GetString(Resource.String.app_name))
                    .SetContentText("Started ForegroundService.")
                    .SetSmallIcon(Resource.Drawable.notification_icon_background) //Android 7
                    .SetColor(ActivityCompat.GetColor(Android.App.Application.Context, Resource.Color.notification_material_background_media_default_color))  //Android7.0対応
                    .SetOngoing(true)
                    .SetChannelId(channelId) // Android8
                    .Build();

            // Enlist this instance of the service as a foreground service
            this.StartForeground(2451, notification);
        }
    }
}