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

namespace Covid19Radar.Droid
{
    [Activity(Label = "Covid19Radar", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme.Splash", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, Android.App.Application.IActivityLifecycleCallbacks
    {
        //public static MainActivity Instance { get; private set; }
        public static object dataLock = new object();

        private SQLiteConnection _connection;

        protected override void OnCreate(Bundle bundle)
        {
            base.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(bundle);

            CrossCurrentActivity.Current.Init(this, bundle);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            //Instance = this;

            Xamarin.Essentials.Platform.Init(this, bundle);
            //            global::Rg.Plugins.Popup.Popup.Init(this, bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            global::Xamarin.Forms.FormsMaterial.Init(this, bundle);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            global::FFImageLoading.ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration()
            {
                Logger = new DebugLogger()
            });

            UserDialogs.Init(this);

            LoadApplication(new App(new AndroidInitializer()));
            CreateNotificationFromIntent(Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            CreateNotificationFromIntent(intent);
        }

        void CreateNotificationFromIntent(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string title = intent.Extras.GetString(Services.NotificationService.TitleKey);
                string message = intent.Extras.GetString(Services.NotificationService.MessageKey);
                // DependencyService.Get<NotificationService>().ReceiveNotification(title, message);
                App.Current.Container.Resolve<INotificationService>().ReceiveNotification(title, message);
            }
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
                containerRegistry.RegisterSingleton<INotificationService, NotificationService>();

            }
        }

        private void RequestPermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                string[] permissions = new string[] {
                    Android.Manifest.Permission.Bluetooth,
                    Android.Manifest.Permission.BluetoothAdmin,
                    Android.Manifest.Permission.AccessCoarseLocation,
                    Android.Manifest.Permission.AccessFineLocation
                };

                RequestPermissions(permissions, 0);
            }
        }

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
    }
}

