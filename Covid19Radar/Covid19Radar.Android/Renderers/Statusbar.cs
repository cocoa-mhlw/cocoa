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
using Covid19Radar.Renderers;
using Plugin.CurrentActivity;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(Covid19Radar.Droid.Renderers.Statusbar))]
namespace Covid19Radar.Droid.Renderers
{
    public class Statusbar : IStatusBarPlatformSpecific
    {
        public Statusbar()
        {
        }

        public void SetStatusBarColor(Color color)
        {
            // The SetStatusBarcolor is new since API 21
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var androidColor = color.ToAndroid();
                CrossCurrentActivity.Current.Activity.Window.SetStatusBarColor(androidColor);
            }
            else
            {
                // Here you will just have to set your color in styles.xml file as above.
            }
        }
    }
}
