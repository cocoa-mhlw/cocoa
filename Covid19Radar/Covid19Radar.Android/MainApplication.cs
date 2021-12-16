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
using Chino;
using Chino.Android.Google;
using System.Collections.Generic;
using CommonServiceLocator;

namespace Covid19Radar.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class MainApplication : Application, IExposureNotificationHandler
    {
        private const long INITIAL_BACKOFF_MILLIS = 60 * 60 * 1000;

        private readonly JobSetting _exposureDetectedV1JobSetting
            = new JobSetting(INITIAL_BACKOFF_MILLIS, Android.App.Job.BackoffPolicy.Linear, true);
        private readonly JobSetting _exposureDetectedV2JobSetting
            = new JobSetting(INITIAL_BACKOFF_MILLIS, Android.App.Job.BackoffPolicy.Linear, true);
        private readonly JobSetting _exposureNotDetectedJobSetting = null;

        private Lazy<AbsExposureNotificationApiService> _exposureNotificationApiService
            = new Lazy<AbsExposureNotificationApiService>(() => ServiceLocator.Current.GetInstance<AbsExposureNotificationApiService>());

        private Lazy<IExposureDetectionService> _exposureDetectionService
            = new Lazy<IExposureDetectionService>(() => ServiceLocator.Current.GetInstance<IExposureDetectionService>());

        private Lazy<AbsExposureDetectionBackgroundService> _exposureDetectionBackgroundService
            = new Lazy<AbsExposureDetectionBackgroundService>(() => ServiceLocator.Current.GetInstance<AbsExposureDetectionBackgroundService>());

        private Lazy<ILoggerService> _loggerService
            = new Lazy<ILoggerService>(() => ServiceLocator.Current.GetInstance<ILoggerService>());

        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public AbsExposureNotificationClient GetEnClient()
        {
            if (_exposureNotificationApiService.Value is ExposureNotificationApiService exposureNotificationApiService)
            {
                return exposureNotificationApiService.Client;
            }
            else
            {
                return null;
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();

            App.InitializeServiceLocator(RegisterPlatformTypes);

            AbsExposureNotificationClient.Handler = this;
            if (_exposureNotificationApiService.Value is ExposureNotificationApiService exposureNotificationApiService)
            {
                SetupENClient(exposureNotificationApiService.Client);
            }

            try
            {
                _exposureDetectionBackgroundService.Value.Schedule();
            }
            catch (Exception exception)
            {
                _loggerService.Value.Exception("failed to Scheduling", exception);
            }
        }

        private void SetupENClient(ExposureNotificationClient client)
        {
            client.Init(this);
            client.ExposureDetectedV1JobSetting = _exposureDetectedV1JobSetting;
            client.ExposureDetectedV2JobSetting = _exposureDetectedV2JobSetting;
            client.ExposureNotDetectedJobSetting = _exposureNotDetectedJobSetting;
        }

        private void RegisterPlatformTypes(IContainer container)
        {
            // Services
            container.Register<IBackupAttributeService, BackupAttributeService>(Reuse.Singleton);
            container.Register<ILocalPathService, LocalPathService>(Reuse.Singleton);
            container.Register<ILogPeriodicDeleteService, LogPeriodicDeleteService>(Reuse.Singleton);
            container.Register<ISecureStorageDependencyService, Services.SecureStorageService>(Reuse.Singleton);
            container.Register<IPreferencesService, PreferencesService>(Reuse.Singleton);
            container.Register<IApplicationPropertyService, ApplicationPropertyService>(Reuse.Singleton);
            container.Register<ILocalContentService, LocalContentService>(Reuse.Singleton);
            container.Register<ILocalNotificationService, LocalNotificationService>(Reuse.Singleton);
            container.Register<IMigrationProcessService, MigrationProccessService>(Reuse.Singleton);
            container.Register<AbsExposureDetectionBackgroundService, ExposureDetectionBackgroundService>(Reuse.Singleton);
            container.Register<ICloseApplicationService, CloseApplicationService>(Reuse.Singleton);
#if USE_MOCK
            container.Register<IDeviceVerifier, DeviceVerifierMock>(Reuse.Singleton);
            container.Register<AbsExposureNotificationApiService, MockExposureNotificationApiService>(Reuse.Singleton);
#else
            container.Register<IDeviceVerifier, DeviceCheckService>(Reuse.Singleton);
            container.Register<AbsExposureNotificationApiService, ExposureNotificationApiService>(Reuse.Singleton);
#endif

            container.Register<IExternalNavigationService, ExternalNavigationService>(Reuse.Singleton);
            container.Register<IPlatformService, PlatformService>(Reuse.Singleton);
        }

        public void PreExposureDetected()
        {
            var exposureConfiguration = GetEnClient().ExposureConfiguration;
            var enVersion = GetEnClient().GetVersionAsync()
                .GetAwaiter().GetResult().ToString();
            _exposureDetectionService.Value.PreExposureDetected(exposureConfiguration, enVersion);
        }

        public void ExposureDetected(IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
        {
            var exposureConfiguration = GetEnClient().ExposureConfiguration;
            var enVersion = GetEnClient().GetVersionAsync()
                .GetAwaiter().GetResult().ToString();
            _exposureDetectionService.Value.ExposureDetected(exposureConfiguration, enVersion, dailySummaries, exposureWindows);
        }

        public void ExposureDetected(ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
        {
            var exposureConfiguration = GetEnClient().ExposureConfiguration;
            var enVersion = GetEnClient().GetVersionAsync()
                .GetAwaiter().GetResult().ToString();
            _exposureDetectionService.Value.ExposureDetected(exposureConfiguration, enVersion, exposureSummary, exposureInformations);
        }

        public void ExposureNotDetected()
        {
            var exposureConfiguration = GetEnClient().ExposureConfiguration;
            var enVersion = GetEnClient().GetVersionAsync()
                .GetAwaiter().GetResult().ToString();
            _exposureDetectionService.Value.ExposureNotDetected(exposureConfiguration, enVersion);
        }
    }
}
