/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Android.App;
using Android.Runtime;
using DryIoc;
using Covid19Radar.Services.Logs;
using Covid19Radar.Droid.Services.Logs;
using Covid19Radar.Services;
using Covid19Radar.Droid.Services;
using AndroidX.Core.App;
using Covid19Radar.Resources;
using Android.OS;
using CommonServiceLocator;

namespace Covid19Radar.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {
        public const string NOTIFICATION_CHANNEL_ID = "notification_channel_cocoa_202107";

        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            App.InitializeServiceLocator(RegisterPlatformTypes);
            App.UseMockExposureNotificationImplementationIfNeeded();

            CreateNotificationChannel();

#if DEBUG
            ILocalNotificationService localNotificationService = ServiceLocator.Current.GetInstance<ILocalNotificationService>();
            localNotificationService.ShowExposureNotification();
#endif
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            NotificationChannelCompat notificationChannel = new NotificationChannelCompat
                .Builder(NOTIFICATION_CHANNEL_ID, NotificationManagerCompat.ImportanceDefault)
                .SetName(AppResources.AndroidNotificationChannelName)
                .SetShowBadge(false)
                .Build();

            var nm = NotificationManagerCompat.From(this);
            nm.CreateNotificationChannel(notificationChannel);
        }

        private void RegisterPlatformTypes(IContainer container)
        {
            // Services
            container.Register<ILogPathDependencyService, LogPathServiceAndroid>(Reuse.Singleton);
            container.Register<ISecureStorageDependencyService, SecureStorageServiceAndroid>(Reuse.Singleton);
            container.Register<IPreferencesService, PreferencesService>(Reuse.Singleton);
            container.Register<IApplicationPropertyService, ApplicationPropertyService>(Reuse.Singleton);
            container.Register<ILocalNotificationService, LocalNotificationService>(Reuse.Singleton);

#if USE_MOCK
            container.Register<IDeviceVerifier, DeviceVerifierMock>(Reuse.Singleton);
#else
            container.Register<IDeviceVerifier, DeviceCheckService>(Reuse.Singleton);
#endif
        }
    }
}
