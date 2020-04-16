using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;
using Android.Runtime;
using Android.Content;
using Plugin.CurrentActivity;
using Covid19Radar.Model;
using AltBeaconOrg.BoundBeacon;
using System.Collections.Generic;
using System.Linq;
using System;
using Covid19Radar.Common;
using Covid19Radar.Droid.Services;
using Covid19Radar.Services;

namespace Covid19Radar.Droid
{
    [Activity(Label = "Covid19Radar", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IBeaconConsumer
    {
        public static MainActivity Instance { get; private set; }
        public static SQLiteConnectionProvider sqliteConnectionProvider { get; private set; }
        protected override void OnCreate(Bundle bundle)
        {
            base.SetTheme(Resource.Style.MainTheme);
            base.OnCreate(bundle);

            CrossCurrentActivity.Current.Init(this, bundle);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Instance = this;
            sqliteConnectionProvider = new SQLiteConnectionProvider();

            Xamarin.Essentials.Platform.Init(this, bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new AndroidInitializer()));
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                containerRegistry.RegisterSingleton<IBeaconService,BeaconService>();

                containerRegistry.RegisterInstance<SQLiteConnectionProvider>(MainActivity.sqliteConnectionProvider);
            }
        }

        public void OnBeaconServiceConnect()
        {
            BeaconService beaconService = Xamarin.Forms.DependencyService.Get<BeaconService>();
            UserDataService userDataService = new UserDataService();
            beaconService.StartBeacon();

            if (userDataService.IsExistUserData())
            {
                UserDataModel userDataModel = userDataService.Get();
                beaconService.StartAdvertising(userDataModel);
            }
        }

    }
}

