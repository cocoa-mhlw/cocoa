/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Common;
using Covid19Radar.iOS.Services;
using Covid19Radar.iOS.Services.Logs;
using Covid19Radar.iOS.Services.Migration;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Services.Migration;
using DryIoc;
using Foundation;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
using System.Linq;

using FormsApplication = Xamarin.Forms.Application;
using Prism.Navigation;
using Covid19Radar.Views;
using System;
using CommonServiceLocator;

namespace Covid19Radar.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private Lazy<ILoggerService> _loggerService
                    = new Lazy<ILoggerService>(() => ServiceLocator.Current.GetInstance<ILoggerService>());

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

        private readonly UserNotificationCenterDelegate _notificationCenterDelegate = new UserNotificationCenterDelegate();

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

            App.InitExposureNotification();

            Xamarin.Forms.Forms.SetFlags("RadioButton_Experimental");

            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.Forms.FormsMaterial.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            global::FFImageLoading.ImageService.Instance.Initialize(new FFImageLoading.Config.Configuration());


            _notificationCenterDelegate.OnRecieved += async (UserNotificationCenterDelegate sender, UNNotificationResponse response) =>
            {
                NavigationParameters navigationParameters = new NavigationParameters();
                await AppInstance?.NavigateToSplashAsync(Destination.ContactedNotifyPage, navigationParameters);
            };
            UNUserNotificationCenter.Current.Delegate = _notificationCenterDelegate;

            LoadApplication(new App());

            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
            return base.FinishedLaunching(app, options);
        }

        public override bool ContinueUserActivity(UIApplication application, NSUserActivity userActivity, UIApplicationRestorationHandler completionHandler)
        {
            if (userActivity.ActivityType == NSUserActivityType.BrowsingWeb && userActivity.WebPageUrl != null)
            {
                NavigateUniversalLinks(userActivity.WebPageUrl);
                return true;
            }
            else
            {
                _loggerService.Value.Info($"Failed to handle ContinueUserActivity.");
                return base.ContinueUserActivity(application, userActivity, completionHandler);
            }
        }

        private void NavigateUniversalLinks(NSUrl url)
        {
            try
            {
                var urlComponents = new NSUrlComponents(url, true);
                if (urlComponents.Path?.StartsWith("/cocoa/a/") == true)
                {
                    var processingNumber = urlComponents
                        .QueryItems?
                        .FirstOrDefault(item => item.Name == AppConstants.LinkQueryKeyProcessingNumber)?
                        .Value;

                    if (processingNumber != null && Validator.IsValidProcessingNumber(processingNumber))
                    {
                        var navigationParameters = new NavigationParameters();
                        navigationParameters = NotifyOtherPage.BuildNavigationParams(processingNumber, navigationParameters);
                        InvokeOnMainThread(async () => await AppInstance?.NavigateToSplashAsync(Destination.NotifyOtherPage, navigationParameters));
                    }
                    else
                    {
                        _loggerService.Value.Error("Failed to navigate NotifyOtherPage with invalid processingNumber");
                    }
                }
            }
            catch(Exception e)
            {
                _loggerService.Value.Exception("Failed to NavigateUniversalLinks", e);
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
            container.Register<IBackupAttributeService, BackupAttributeService>(Reuse.Singleton);
            container.Register<ILogPathPlatformService, LogPathPlatformService>(Reuse.Singleton);
            container.Register<ILogPeriodicDeleteService, LogPeriodicDeleteService>(Reuse.Singleton);
            container.Register<ISecureStorageDependencyService, Services.SecureStorageService>(Reuse.Singleton);
            container.Register<IPreferencesService, PreferencesService>(Reuse.Singleton);
            container.Register<IApplicationPropertyService, ApplicationPropertyService>(Reuse.Singleton);
            container.Register<ILocalContentService, LocalContentService>(Reuse.Singleton);
            container.Register<ILocalNotificationService, LocalNotificationService>(Reuse.Singleton);
            container.Register<IMigrationProcessService, MigrationProcessService>(Reuse.Singleton);
            container.Register<ICloseApplicationService, CloseApplicationService>(Reuse.Singleton);
#if USE_MOCK
            container.Register<IDeviceVerifier, DeviceVerifierMock>(Reuse.Singleton);
#else
            container.Register<IDeviceVerifier, DeviceCheckService>(Reuse.Singleton);
#endif
            container.Register<IExposureNotificationStatusPlatformService, ExposureNotificationStatusPlatformService>(Reuse.Singleton);
            container.Register<IExternalNavigationService, ExternalNavigationService>(Reuse.Singleton);
        }
    }
}

public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
{
    public delegate void NotificationCenterReceivedEventHandler(UserNotificationCenterDelegate sender, UNNotificationResponse response);
    public event NotificationCenterReceivedEventHandler OnRecieved;

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

    public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, System.Action completionHandler)
    {
        OnRecieved?.Invoke(this, response);
        completionHandler();
    }
}
