/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.iOS.Services;
using Covid19Radar.iOS.Services.Logs;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using DryIoc;
using Foundation;
using UIKit;

namespace Covid19Radar.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
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
            App.UseMockExposureNotificationImplementationIfNeeded();

            Xamarin.Forms.Forms.SetFlags("RadioButton_Experimental");

            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            global::FFImageLoading.ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration()
            {
                Logger = new Covid19Radar.Services.DebugLogger()
            });

            //Plugin.LocalNotification.NotificationCenter.AskPermission();

            LoadApplication(new App());

            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
            return base.FinishedLaunching(app, options);
        }

        //public override void WillEnterForeground(UIApplication uiApplication)
        //{
        //    Plugin.LocalNotification.NotificationCenter.ResetApplicationIconBadgeNumber(uiApplication);
        //}

        private void RegisterPlatformTypes(IContainer container)
        {
            // Services
            container.Register<ILogPathDependencyService, LogPathServiceIos>(Reuse.Singleton);
            container.Register<ISecureStorageDependencyService, SecureStorageServiceIos>(Reuse.Singleton);
            container.Register<IPreferencesService, PreferencesService>(Reuse.Singleton);
            container.Register<IApplicationPropertyService, ApplicationPropertyService>(Reuse.Singleton);

#if USE_MOCK
            container.Register<IDeviceVerifier, DeviceVerifierMock>(Reuse.Singleton);
#else
            container.Register<IDeviceVerifier, DeviceCheckService>(Reuse.Singleton);
#endif
        }
    }
}
