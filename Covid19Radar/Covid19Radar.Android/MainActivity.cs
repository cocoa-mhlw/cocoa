using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;
using Android.Runtime;
using Android.Content;
using Plugin.CurrentActivity;
using Covid19Radar.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using Covid19Radar.Common;
using Covid19Radar.Droid.Services;
using Covid19Radar.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using Acr.UserDialogs;
using Covid19Radar.Renderers;
using Covid19Radar.Droid.Renderers;
using Plugin.LocalNotification;

namespace Covid19Radar.Droid
{
    [Activity(Label = "Covid19Radar", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme.Splash", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static object dataLock = new object();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(savedInstanceState);

            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            global::FFImageLoading.ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration()
            {
                Logger = new DebugLogger()
            });

            UserDialogs.Init(this);

            NotificationCenter.CreateNotificationChannel();
            LoadApplication(new App(new AndroidInitializer()));
            NotificationCenter.NotifyNotificationTapped(base.Intent);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                containerRegistry.RegisterSingleton<ISQLiteConnectionProvider, SQLiteConnectionProvider>();
                containerRegistry.RegisterSingleton<UserDataService, UserDataService>();
            }
        }

        private void RequestPermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                string[] permissions = new string[] {
                    Android.Manifest.Permission.Bluetooth,
                    Android.Manifest.Permission.BluetoothPrivileged,
                    Android.Manifest.Permission.BluetoothAdmin,
                };

                RequestPermissions(permissions, 0);
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            NotificationCenter.NotifyNotificationTapped(intent);

            base.OnNewIntent(intent);
        }

        /*
        #region IActivityLifecycleCallbacks
        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
        }

        public void OnActivityDestroyed(Activity activity)
        {
        }

        public void OnActivityPaused(Activity activity)
        {
        }

        public void OnActivityResumed(Activity activity)
        {
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
        }

        public void OnActivityStopped(Activity activity)
        {
        }
        #endregion

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        */
    }
}

