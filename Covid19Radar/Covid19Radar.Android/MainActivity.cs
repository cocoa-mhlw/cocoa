/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Content;
using Acr.UserDialogs;
using System;
using System.Threading.Tasks;

using FormsApplication = Xamarin.Forms.Application;
using Covid19Radar.Views;
using Prism.Navigation;

namespace Covid19Radar.Droid
{
    [Activity(
        Label = "@string/app_name",
        Icon = "@mipmap/ic_launcher",
        Theme = "@style/MainTheme.Splash",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation
        )]
    [IntentFilter(
        new[] { Intent.ActionView },
        AutoVerify = true,
        Categories = new[]
        {
            Intent.CategoryDefault,
            Intent.CategoryBrowsable
        },
        DataScheme = "https",
        DataHost = "www.mhlw.go.jp",
        DataPathPattern = "/cocoa/a/.*"
        )
    ]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const string EXTRA_KEY_DESTINATION = "key_destination";
        private const string QUERY_KEY_PROCESSING_NAME = "pn";

        internal static Intent NewIntent(Context context)
        {
            Intent intent = new Intent(context, typeof(MainActivity));
            return intent;
        }

        internal static Intent NewIntent(Context context, Destination destination)
        {
            Intent intent = new Intent(context, typeof(MainActivity));
            intent.PutExtra(EXTRA_KEY_DESTINATION, (int)destination);
            return intent;
        }

        private App? AppInstance
        {
            get
            {
                if (FormsApplication.Current is App app)
                {
                    return app;
                }
                return null;
            }
        }
        public static object dataLock = new object();

        protected override async void OnCreate(Bundle savedInstanceState)
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

            LoadApplication(new App());

            await NavigateToDestinationFromIntent(Intent);
        }


        private async Task NavigateToDestinationFromIntent(Intent intent)
        {
            if (intent.Data != null)
            {
                var processingNumber = intent.Data.GetQueryParameter(QUERY_KEY_PROCESSING_NAME);

                var param = new NavigationParameters();
                param = NotifyOtherPage.CreateNavigationParams(processingNumber, param);
                param = SplashPage.CreateNavigationParams(Destination.NotifyOtherPage, param);
                await AppInstance?.NavigateToSplashAsync(param);

            }
            else if (intent.HasExtra(EXTRA_KEY_DESTINATION))
            {
                int ordinal = intent.GetIntExtra(EXTRA_KEY_DESTINATION, (int)Destination.HomePage);
                var destination = (Destination)Enum.ToObject(typeof(Destination), ordinal);

                var param = SplashPage.CreateNavigationParams(destination, new NavigationParameters());
                await AppInstance?.NavigateToSplashAsync(param);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            Xamarin.ExposureNotifications.ExposureNotification.OnActivityResult(requestCode, resultCode, data);
        }

        protected async override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            await NavigateToDestinationFromIntent(intent);
        }

    }
}
