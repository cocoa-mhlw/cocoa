using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;
using Android.Runtime;
using Android.Content;
using Covid19Radar.Droid.Services.Logs;
using Covid19Radar.Services.Logs;
using Acr.UserDialogs;
using Covid19Radar.Services;
using Covid19Radar.Droid.Services;
using System;
using System.IO;
using AndroidX.Core.Content;
using Android;
using AndroidX.Core.App;
using System.Collections.Generic;
using System.Diagnostics;
//using Plugin.LocalNotification;

namespace Covid19Radar.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme.Splash", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static object dataLock = new object();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(savedInstanceState);

            Xamarin.Forms.Forms.SetFlags("RadioButton_Experimental");
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Forms.FormsMaterial.Init(this, savedInstanceState);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            global::FFImageLoading.ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration());

            UserDialogs.Init(this);

            // files へのファイルアクセスに必要かもしれない
            /*
            var permission = ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage);
            if (permission != (int)Permission.Granted)
            {
                // We don't have permission so prompt the user
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.WriteExternalStorage },1);
            }
            */

            /*
            // ログ用のパスを一度取得する
            // こうしないと files フォルダが作られない
            var contextRef = new System.WeakReference<Context>(this);
            contextRef.TryGetTarget(out var c);
            var dir = c.GetExternalFilesDir(null).AbsolutePath;
            var filename = Path.Combine(dir, $"trace-droid-{DateTime.Now.ToString("yyyyMMdd-HHmm")}.txt");
            var tw = System.IO.File.OpenWrite(filename);
            var tr1 = new System.Diagnostics.TextWriterTraceListener(tw);
            DroidTrace.AutoFlush = true;
            DroidTrace.Listeners.Add(tr1);
            DroidTrace.WriteLine("START: " + DateTime.Now.ToString());
            */


            //NotificationCenter.CreateNotificationChannel();
            LoadApplication(new App(new AndroidInitializer()));
            //NotificationCenter.NotifyNotificationTapped(base.Intent);


            // Appの初期化以降は、App.LoggerService.Debug が使える
            App.LoggerService.Info("START in Android: ");
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
                // Services
                containerRegistry.RegisterSingleton<ILogPathDependencyService, LogPathServiceAndroid>();
                containerRegistry.RegisterSingleton<ISecureStorageDependencyService, SecureStorageServiceAndroid>();
                containerRegistry.RegisterSingleton<IPreferencesService, PreferencesService>();
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

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            Xamarin.ExposureNotifications.ExposureNotification.OnActivityResult(requestCode, resultCode, data);
        }

        //protected override void OnNewIntent(Intent intent)
        //{
        //    NotificationCenter.NotifyNotificationTapped(intent);

        //    base.OnNewIntent(intent);
        //}

    }
    /// <summary>
    /// 自前の Andorid版 Trace
    /// </summary>
    public class DroidTrace
    {
        static DroidTrace()
        {
            Listeners = new List<TraceListener>();
        }
        public static List<TraceListener> Listeners { get; }
        public static bool AutoFlush { get; set; } = true;
        public static void WriteLine(string message)
        {
            foreach (var it in Listeners)
            {
                it.WriteLine(message);
                if (AutoFlush == true) it.Flush();
            }
        }
    }
}

