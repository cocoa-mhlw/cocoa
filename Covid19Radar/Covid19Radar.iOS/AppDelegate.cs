/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Chino;
using CommonServiceLocator;
using Covid19Radar.Common;
using Covid19Radar.iOS.Services;
using Covid19Radar.iOS.Services.Logs;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using DryIoc;
using Foundation;
using UIKit;
using UserNotifications;
using Xamarin.Forms;

namespace Covid19Radar.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IExposureNotificationHandler
    {
        private Lazy<AbsExposureNotificationApiService> _exposureNotificationClient
            = new Lazy<AbsExposureNotificationApiService>(() => ServiceLocator.Current.GetInstance<AbsExposureNotificationApiService>());

        public static AppDelegate Instance { get; private set; }
        public AppDelegate()
        {
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            NSUrlCache.SharedCache.RemoveAllCachedResponses();

            App.InitializeServiceLocator(RegisterPlatformTypes);

            InitializeExposureNotificationClient();

            Xamarin.Forms.Forms.SetFlags("RadioButton_Experimental");

            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            global::FFImageLoading.ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration());

            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();

            LoadApplication(new App());

            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
            return base.FinishedLaunching(app, options);
        }

        public AbsExposureNotificationClient GetEnClient() => _exposureNotificationClient.Value;

        private void InitializeExposureNotificationClient()
        {
            AbsExposureNotificationClient.Handler = this;

            if (GetEnClient() is ExposureNotificationApiService exposureNotificationApiService)
            {
                exposureNotificationApiService.UserExplanation = AppResources.LocalNotificationDescription;

#if DEBUG
                exposureNotificationApiService.IsTest = true;
#else
                exposureNotificationApiService.IsTest = false;
#endif
            }
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);
            MessagingCenter.Send((object)this, AppConstants.IosOnActivatedMessage);
        }

        private void RegisterPlatformTypes(IContainer container)
        {
            // Services
            container.Register<ILogPathDependencyService, LogPathServiceIos>(Reuse.Singleton);
            container.Register<ISecureStorageDependencyService, SecureStorageServiceIos>(Reuse.Singleton);
            container.Register<IPreferencesService, PreferencesService>(Reuse.Singleton);
            container.Register<IApplicationPropertyService, ApplicationPropertyService>(Reuse.Singleton);
            container.Register<ILocalContentService, LocalContentService>(Reuse.Singleton);
            container.Register<ILocalNotificationService, LocalNotificationService>(Reuse.Singleton);

            container.Register<ICloseApplication, CloseApplication>(Reuse.Singleton);
            container.Register<IBackgroundService, BackgroundService>(Reuse.Singleton);

#if USE_MOCK
            container.Register<IDeviceVerifier, DeviceVerifierMock>(Reuse.Singleton);
            container.Register<AbsExposureNotificationApiService, MockExposureNotificationApiService>(Reuse.Singleton);
#else
            container.Register<IDeviceVerifier, DeviceCheckService>(Reuse.Singleton);
            container.Register<AbsExposureNotificationApiService, ExposureNotificationApiService>(Reuse.Singleton);
#endif
        }

        public void PreExposureDetected()
        {
        }

        public void ExposureDetected(IList<DailySummary> dailySummaries, IList<ExposureWindow> exposureWindows)
        {
        }

        public void ExposureDetected(ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformations)
        {
        }

        public void ExposureNotDetected()
        {
        }
    }
}

public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
{
    public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, System.Action<UNNotificationPresentationOptions> completionHandler)
    {
        if (UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
        {
            completionHandler(UNNotificationPresentationOptions.Banner | UNNotificationPresentationOptions.List);
        }
        else
        {
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
        
    }
}
