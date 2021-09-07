/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Content;
using Acr.UserDialogs;
using Covid19Radar.Droid.Services;
using System;
using Prism.Common;

namespace Covid19Radar.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme.Splash", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static Intent NewIntent(Context context)
        {
            Intent intent = new Intent(context, typeof(MainActivity));
            return intent;
        }

        public static object dataLock = new object();

        private App _app;

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

            //NotificationCenter.CreateNotificationChannel();
            _app = new App();
            LoadApplication(_app);
            //NotificationCenter.NotifyNotificationTapped(base.Intent);
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

            var isOk = (resultCode == Result.Ok);

            FireExposureNotificationEvent(requestCode, isOk);
        }

        private void FireExposureNotificationEvent(int requestCode, bool isOk)
        {
            Action<IExposureNotificationEventCallback> action = requestCode switch
            {
                ExposureNotificationApiService.REQUEST_EN_START
                    => new Action<IExposureNotificationEventCallback>(callback =>
                    {
                        if(isOk)
                        {
                            callback.OnEnabled();
                        }
                        else
                        {
                            callback.OnDeclined();
                        }
                    }),
                ExposureNotificationApiService.REQUEST_GET_TEK_HISTORY
                    => new Action<IExposureNotificationEventCallback>(callback => { callback.OnGetTekHistoryAllowed(); }),
                ExposureNotificationApiService.REQUEST_PREAUTHORIZE_KEYS
                    => new Action<IExposureNotificationEventCallback>(callback => { callback.OnPreauthorizeAllowed(); }),
                _ => new Action<IExposureNotificationEventCallback>(callback => { /* do nothing */ }),
            };
            PageUtilities.InvokeViewAndViewModelAction(PageUtilities.GetCurrentPage(_app.MainPage), action);
        }

        //protected override void OnNewIntent(Intent intent)
        //{
        //    NotificationCenter.NotifyNotificationTapped(intent);

        //    base.OnNewIntent(intent);
        //}

    }
}
