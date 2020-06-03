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
using Plugin.LocalNotification;

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

            // Exposure Notification

#if DEBUG
            // For debug mode, set the mock api provider to interact
            // with some fake data
            Xamarin.ExposureNotifications.ExposureNotification.OverrideNativeImplementation(new Services.TestNativeImplementation());
#endif
            Xamarin.ExposureNotifications.ExposureNotification.Init();

            // Local Notification tap event listener
            NotificationCenter.Current.NotificationTapped += OnNotificationTapped;
            LogUnobservedTaskExceptions();

            Distribute.ReleaseAvailable = OnReleaseAvailable;
            Push.PushNotificationReceived += OnPushNotificationReceived;
            AppCenter.Start($"android={AppConstants.AppCenterTokensAndroid};ios={AppConstants.AppCenterTokensIOS};", typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
            Container.Resolve<ILogger>().Log("Started App Center");

            _ = InitializeBackgroundTasks();

            INavigationResult result;
            // Check user data and skip tutorial
            UserDataService userDataService = Container.Resolve<UserDataService>();

            if (userDataService.IsExistUserData)
            {
                var userData = userDataService.Get();
                if (userData.IsOptined)
                {
                    result = await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
                }
                else
                {
                    result = await NavigationService.NavigateAsync("/" + nameof(StartTutorialPage));
                }
            }
            else
            {
                result = await NavigationService.NavigateAsync("/" + nameof(StartTutorialPage));
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

        protected void OnNotificationTapped(NotificationTappedEventArgs e)
        {
            NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
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
            containerRegistry.RegisterForNavigation<MenuPage>();
            containerRegistry.RegisterForNavigation<StartTutorialPage>();
            containerRegistry.RegisterForNavigation<DescriptionPage1>();
            containerRegistry.RegisterForNavigation<DescriptionPage2>();
            containerRegistry.RegisterForNavigation<DescriptionPage3>();
            containerRegistry.RegisterForNavigation<DescriptionPage4>();
            containerRegistry.RegisterForNavigation<PrivacyPolicyPage>();
            containerRegistry.RegisterForNavigation<InitSettingPage>();
            containerRegistry.RegisterForNavigation<HomePage>();
            containerRegistry.RegisterForNavigation<ContributorsPage>();
            containerRegistry.RegisterForNavigation<SetupCompletedPage>();
            containerRegistry.RegisterForNavigation<LicenseAgreementPage>();
            containerRegistry.RegisterForNavigation<NotifyOtherPage>();
            containerRegistry.RegisterForNavigation<ExposuresPage>();
            containerRegistry.RegisterForNavigation<UpdateInformationPage>();
            containerRegistry.RegisterForNavigation<SettingsPage>();
            containerRegistry.RegisterForNavigation<DebugPage>();

            // Norij Work View
            containerRegistry.RegisterForNavigation<TutorialPage1>();
            containerRegistry.RegisterForNavigation<TutorialPage2>();
            containerRegistry.RegisterForNavigation<TutorialPage3>();
            containerRegistry.RegisterForNavigation<TutorialPage4>();
            containerRegistry.RegisterForNavigation<TutorialPage5>();
            containerRegistry.RegisterForNavigation<TutorialPage6>();

            containerRegistry.RegisterForNavigation<HelpMenuPage>();
            containerRegistry.RegisterForNavigation<HelpPage1>();
            containerRegistry.RegisterForNavigation<HelpPage2>();
            containerRegistry.RegisterForNavigation<HelpPage3>();
            containerRegistry.RegisterForNavigation<HelpPage4>();

            containerRegistry.RegisterForNavigation<InqueryPage>();
            containerRegistry.RegisterForNavigation<ChatbotPage>();
            containerRegistry.RegisterForNavigation<TermsofservicePage>();


            // Services
            containerRegistry.RegisterSingleton<UserDataService>();
            containerRegistry.RegisterSingleton<ExposureNotificationService>();
            containerRegistry.RegisterSingleton<HttpDataService>();
        }

        protected override void OnStart()
        {
            //ImageService.Instance.Config.Logger = Container.Resolve<IMiniLogger>();
        }

        protected override void OnResume()
        {
        }

        async Task InitializeBackgroundTasks()
        {
            if (await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
            await Xamarin.ExposureNotifications.ExposureNotification.ScheduleFetchAsync();
        }

        protected override void OnSleep()
        {
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
