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

using FormsApplication = Xamarin.Forms.Application;

namespace Covid19Radar.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme.Splash", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private const string EXTRA_KEY_DESTINATION = "key_destination";

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

            LoadApplication(new App());

            Destination destination = GetDestinationFromIntent(Intent);
            AppInstance.NavigateToSplash(destination);
        }

        private static Destination GetDestinationFromIntent(Intent intent)
        {
            int ordinal = intent.GetIntExtra(EXTRA_KEY_DESTINATION, (int)Destination.HomePage);
            return (Destination)Enum.ToObject(typeof(Destination), ordinal);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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

        protected async override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            var destination = GetDestinationFromIntent(Intent);
            await AppInstance.NavigateTo(destination);
        }

    }
}
