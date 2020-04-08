using Android.App;
using Android.Content.PM;
using Android.OS;
using Prism;
using Prism.Ioc;
using Android.Runtime;
using Android.Content;
using Plugin.CurrentActivity;
using Plugin.Permissions;
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
    [Activity(Label = "Covid19Radar", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IBeaconConsumer
    {
        internal static MainActivity Instance { get; private set; }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            CrossCurrentActivity.Current.Init(this, bundle);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            Instance = this;
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
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                BeaconService beaconService = new BeaconService();
                containerRegistry.RegisterSingleton<IBeaconService,BeaconService>();
            }
        }


        public void OnBeaconServiceConnect()
        {
            /*
            BeaconService beaconService = Xamarin.Forms.DependencyService.Get<BeaconService>();

            beaconService.StartBeacon();
            beaconService.StartAdvertising();
            */
        }

    }
}

