using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Covid19Radar.Droid.BackgroundService
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted,
        "android.intent.action.QUICKBOOT_POWERON",
        "com.htc.intent.action.QUICKBOOT_POWERON",
        "android.intent.action.PACKAGE_INSTALL",
        "android.intent.action.PACKAGE_ADDED",
        Intent.ActionMyPackageReplaced
    })]
    public class BootReceiver : BroadcastReceiver
    {
        public BootReceiver() : base()
        {
        }
        public override void OnReceive(Context context, Intent intent)
        {
            //アプリを起動する場合はこちら
            //Intent activityIntent = new Intent(context, typeof(MainActivity));
            //activityIntent.AddFlags(ActivityFlags.NewTask);
            //context.StartActivity(activityIntent);

            //サービスを起動する
            Intent serviceIntent = new Intent(context, typeof(BackgroundService));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop &&
                Build.VERSION.SdkInt <= BuildVersionCodes.LollipopMr1)
            {
                // Android5 Lollipop
                string packageName = context.PackageManager.GetPackageInfo(context.PackageName, 0).PackageName;
                serviceIntent.SetPackage(packageName);
                serviceIntent.SetClassName(context, packageName + ".BackgroundService");
            }
            else
            {
                serviceIntent.AddFlags(ActivityFlags.NewTask);
            }

            serviceIntent.SetPackage(context.PackageManager.GetPackageInfo(context.PackageName, 0).PackageName);
            // Android 8 Oreo
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                context.StartForegroundService(serviceIntent);
            }
            else
            {
                context.StartService(serviceIntent);
            }

        }
    }
}