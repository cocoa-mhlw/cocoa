using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Covid19Radar.Common;

namespace Covid19Radar.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Xamarin.ExposureNotifications.ExposureNotification.ConfigureBackgroundWorkRequest(TimeSpan.FromHours(6), b =>
                b.SetConstraints(new AndroidX.Work.Constraints.Builder()
                    .SetRequiresBatteryNotLow(true)
                    .SetRequiredNetworkType(AndroidX.Work.NetworkType.Connected)
                    .Build())
                );
        }
    }
}