using System;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Mvvm;
using DryIoc;
using ImTools;
using Covid19Radar.Model;
using System.Threading.Tasks;
using Prism.Navigation;
using Covid19Radar.Services;
using Prism.Services;
using Covid19Radar.Common;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using System.Net.Http;
using Prism.Logging.AppCenter;
using Prism.Logging;
using System.Collections.Generic;
using System.Text;
using Microsoft.AppCenter.Push;
using FFImageLoading.Helpers;
using FFImageLoading;
using Xamarin.ExposureNotifications;

/*
 * Our mission...is
 * Empower every person and every organization on the planet achieve more.
 * Put an end to Covid 19
 */

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Covid19Radar
{
    public partial class App : PrismApplication
    {

        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor.
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer, setFormsDependencyResolver: true) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            LogUnobservedTaskExceptions();

            Distribute.ReleaseAvailable = OnReleaseAvailable;
            Push.PushNotificationReceived += OnPushNotificationReceived;
            AppCenter.Start($"android={AppConstants.AppCenterTokensAndroid};ios={AppConstants.AppCenterTokensIOS};", typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
            Container.Resolve<ILogger>().Log("Started App Center");

            INavigationResult result;
            // Check user data and skip tutorial
            UserDataService userDataService = Container.Resolve<UserDataService>();
            if (userDataService.IsExistUserData)
            {
                // TODO Wire Start Exposure Notification
                /*
                                var isStart = await ExposureNotification.IsEnabledAsync();
                                if (!isStart)
                                {
                                    await ExposureNotification.StartAsync();
                                }
                */
                UserDataModel _userData = userDataService.Get();
                result = await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(StartTutorialPage));
            }
            else
            {
                result = await NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
            }

            if (!result.Success)
            {
                MainPage = new ExceptionPage
                {
                    BindingContext = new ExceptionPageViewModel()
                    {
                        Message = result.Exception.Message
                    }
                };
                System.Diagnostics.Debugger.Break();
            }
        }

        protected void OnLocalNotificationTaped(object sender, EventArgs eventArgs)
        {
            var e = (NotificationEventArgs)eventArgs;
            System.Diagnostics.Debug.WriteLine(e.Title);
            System.Diagnostics.Debug.WriteLine(e.Message);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // logger
            var logger = new AppCenterLogger();
            containerRegistry.RegisterInstance<ILogger>(logger);
            containerRegistry.RegisterInstance<ILoggerFacade>(logger);
            containerRegistry.RegisterSingleton<IMiniLogger, FFImageLoadingLogger>();

            // Viewmodel
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MenuPage, MenuPageViewModel>();
            containerRegistry.RegisterForNavigation<StartTutorialPage, StartTutorialPageViewModel>();
            containerRegistry.RegisterForNavigation<DescriptionPage1, DescriptionPage1ViewModel>();
            containerRegistry.RegisterForNavigation<DescriptionPage2, DescriptionPage2ViewModel>();
            containerRegistry.RegisterForNavigation<DescriptionPage3, DescriptionPage3ViewModel>();
            containerRegistry.RegisterForNavigation<DescriptionPage4, DescriptionPage4ViewModel>();
            containerRegistry.RegisterForNavigation<PrivacyPolicyPage, PrivacyPolicyPageViewModel>();
            containerRegistry.RegisterForNavigation<InitSettingPage, InitSettingPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<ContributorsPage, ContributorsPageViewModel>();
            containerRegistry.RegisterForNavigation<SetupCompletedPage, SetupCompletedPageViewModel>();
            containerRegistry.RegisterForNavigation<LicenseAgreementPage, LicenseAgreementPageViewModel>();
            containerRegistry.RegisterForNavigation<NotifyOtherPage, NotifyOtherPageViewModel>();
            containerRegistry.RegisterForNavigation<ExposuresPage, ExposuresPageViewModel>();
            containerRegistry.RegisterForNavigation<SharePositiveDiagnosisPage, SharePositiveDiagnosisPageViewModel>();
            containerRegistry.RegisterForNavigation<UpdateInfomationPage, UpdateInfomationPageViewModel>();
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();

            containerRegistry.RegisterSingleton<UserDataService>();
            containerRegistry.RegisterSingleton<HttpDataService>();
        }

        protected override void OnStart()
        {
            ImageService.Instance.Config.Logger = Container.Resolve<IMiniLogger>();
            //UserDataService userDataService = Xamarin.Forms.DependencyService.Resolve<UserDataService>();
        }

        protected override void OnResume()
        {
            //UserDataService userDataService = Xamarin.Forms.DependencyService.Resolve<UserDataService>();
            base.OnResume();

        }

        protected override void OnSleep()
        {
            //UserDataService userDataService = Xamarin.Forms.DependencyService.Resolve<UserDataService>();
            base.OnSleep();
        }


        private void LogUnobservedTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Container.Resolve<ILogger>().Report(e.Exception);
            };
        }

        private void OnPushNotificationReceived(object sender, PushNotificationReceivedEventArgs e)
        {
            // Add the notification message and title to the message
            var summary = $"Push notification received:" +
                $"\n\tNotification title: {e.Title}" +
                $"\n\tMessage: {e.Message}";

            // If there is custom data associated with the notification,
            // print the entries
            if (e.CustomData != null)
            {
                summary += "\n\tCustom data:\n";
                foreach (var key in e.CustomData.Keys)
                {
                    summary += $"\t\t{key} : {e.CustomData[key]}\n";
                }
            }

            // Send the notification summary to debug output
            System.Diagnostics.Debug.WriteLine(summary);
            Container.Resolve<ILoggerFacade>().Log(summary, Category.Debug, Priority.None);
        }

        private bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            // Look at releaseDetails public properties to get version information, release notes text or release notes URL
            string versionName = releaseDetails.ShortVersion;
            string versionCodeOrBuildNumber = releaseDetails.Version;
            string releaseNotes = releaseDetails.ReleaseNotes;
            Uri releaseNotesUrl = releaseDetails.ReleaseNotesUrl;

            // custom dialog
            var title = "Version " + versionName + " available!";
            Task answer;

            // On mandatory update, user cannot postpone
            if (releaseDetails.MandatoryUpdate)
            {
                answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install");
            }
            else
            {
                answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install", "Maybe tomorrow...");
            }
            answer.ContinueWith((task) =>
            {
                // If mandatory or if answer was positive
                if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
                {
                    // Notify SDK that user selected update
                    Distribute.NotifyUpdateAction(UpdateAction.Update);
                }
                else
                {
                    // Notify SDK that user selected postpone (for 1 day)
                    // Note that this method call is ignored by the SDK if the update is mandatory
                    Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                }
            });

            // Return true if you are using your own dialog, false otherwise
            return true;
        }

    }
}
