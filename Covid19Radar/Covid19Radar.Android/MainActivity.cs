/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Acr.UserDialogs;
using System;
using System.Threading.Tasks;

using FormsApplication = Xamarin.Forms.Application;
using Covid19Radar.Views;
using Prism.Navigation;
using Covid19Radar.Common;
using Covid19Radar.Services.Logs;
using CommonServiceLocator;

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

        private Lazy<ILoggerService> _loggerService
                    = new Lazy<ILoggerService>(() => ServiceLocator.Current.GetInstance<ILoggerService>());

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
            _loggerService.Value.StartMethod();


            if (intent.Data != null)
            {
                _loggerService.Value.Info("Intent has data.");

                var processingNumber = intent.Data.GetQueryParameter(AppConstants.LinkQueryKeyProcessingNumber);

                if (processingNumber != null && Validator.IsValidProcessingNumber(processingNumber))
                {
                    _loggerService.Value.Info("ProcessingNumber is valid.");

                    var navigationParameters = new NavigationParameters();
                    navigationParameters = NotifyOtherPage.BuildNavigationParams(processingNumber, navigationParameters);
                    await AppInstance?.NavigateToSplashAsync(Destination.NotifyOtherPage, navigationParameters);
                }
                else
                {
                    _loggerService.Value.Error("Failed to navigate NotifyOtherPage with invalid processingNumber");
                    await AppInstance?.NavigateToSplashAsync(Destination.HomePage, new NavigationParameters());
                }
            }
            else if (intent.HasExtra(EXTRA_KEY_DESTINATION))
            {
                int ordinal = intent.GetIntExtra(EXTRA_KEY_DESTINATION, (int)Destination.HomePage);
                var destination = (Destination)Enum.ToObject(typeof(Destination), ordinal);

                _loggerService.Value.Info($"Intent has destination: {destination}");

                var navigationParameters = new NavigationParameters();
                await AppInstance?.NavigateToSplashAsync(destination, navigationParameters);
            }
            else
            {
                await AppInstance?.NavigateToSplashAsync(Destination.HomePage, new NavigationParameters());
            }

            _loggerService.Value.EndMethod();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            Xamarin.ExposureNotifications.ExposureNotification.OnActivityResult(requestCode, resultCode, data);
        }

        protected async override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (intent.Data != null)
            {
                await NavigateToDestinationFromIntent(intent);
            }
        }

    }
}
