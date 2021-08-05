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
using Covid19Radar.Services.Migration;
using Covid19Radar.Droid.Services.Migration;
using AndroidX.Work;
using Xamarin.ExposureNotifications;
using Java.Util.Concurrent;

namespace Covid19Radar.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {
        private const int WORKER_REPEATED_INTERVAL_HOURS = 6;
        private const int WORKER_BACKOFF_DELAY_HOURS = 1;

        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            App.InitializeServiceLocator(RegisterPlatformTypes);

            // Override WorkRequest configuration
            // Must be run before being scheduled with `ExposureNotification.Init()` in `App.OnInitialized()`
            var repeatInterval = TimeSpan.FromHours(WORKER_REPEATED_INTERVAL_HOURS);
            static void requestBuilder(PeriodicWorkRequest.Builder b) =>
               b.SetConstraints(new Constraints.Builder()
                   .SetRequiresBatteryNotLow(true)
                   .SetRequiredNetworkType(NetworkType.Connected)
                   .Build())
               .SetBackoffCriteria(
                   BackoffPolicy.Linear,
                   WORKER_BACKOFF_DELAY_HOURS,
                   TimeUnit.Hours
                   );
            ExposureNotification.ConfigureBackgroundWorkRequest(repeatInterval, requestBuilder);

            App.InitExposureNotification();
        }

        private void RegisterPlatformTypes(IContainer container)
        {
            // Services
            container.Register<IBackupAttributeService, BackupAttributeService>(Reuse.Singleton);
            container.Register<ILogPathPlatformService, LogPathPlatformService>(Reuse.Singleton);
            container.Register<ILogPeriodicDeleteService, LogPeriodicDeleteService>(Reuse.Singleton);
            container.Register<ISecureStorageDependencyService, Services.SecureStorageService>(Reuse.Singleton);
            container.Register<IPreferencesService, PreferencesService>(Reuse.Singleton);
            container.Register<IApplicationPropertyService, ApplicationPropertyService>(Reuse.Singleton);
            container.Register<ILocalContentService, LocalContentService>(Reuse.Singleton);
            container.Register<ILocalNotificationService, LocalNotificationService>(Reuse.Singleton);
            container.Register<IMigrationProcessService, MigrationProccessService>(Reuse.Singleton);
            container.Register<ICloseApplicationService, CloseApplicationService>(Reuse.Singleton);
#if USE_MOCK
            container.Register<IDeviceVerifier, DeviceVerifierMock>(Reuse.Singleton);
#else
            container.Register<IDeviceVerifier, DeviceCheckService>(Reuse.Singleton);
#endif
        }
    }
}
