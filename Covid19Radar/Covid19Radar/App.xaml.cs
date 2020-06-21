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
using System.Net.Http;
using Prism.Logging;
using System.Collections.Generic;
using System.Text;
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

#if USE_MOCK
            // For debug mode, set the mock api provider to interact
            // with some fake data
            Xamarin.ExposureNotifications.ExposureNotification.OverrideNativeImplementation(new Services.TestNativeImplementation());
#endif
            Xamarin.ExposureNotifications.ExposureNotification.Init();

            // Local Notification tap event listener
            NotificationCenter.Current.NotificationTapped += OnNotificationTapped;
            LogUnobservedTaskExceptions();

            // Migrate userData
            await UserDataMigrationService.Migrate();

            INavigationResult result;
            // Check user data and skip tutorial
            UserDataService userDataService = Container.Resolve<UserDataService>();

            if (userDataService.IsExistUserData)
            {
                var userData = userDataService.Get();
                if (userData.IsOptined && userData.IsPolicyAccepted)
                {
                    result = await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
                }
                else
                {
                    result = await NavigationService.NavigateAsync("/" + nameof(TutorialPage1));
                }
            }
            else
            {
                result = await NavigationService.NavigateAsync("/" + nameof(TutorialPage1));
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
            // Base and Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MenuPage>();
            containerRegistry.RegisterForNavigation<HomePage>();


            // Settings
            containerRegistry.RegisterForNavigation<SettingsPage>();
            containerRegistry.RegisterForNavigation<LicenseAgreementPage>();
            containerRegistry.RegisterForNavigation<DebugPage>();

            // tutorial
            containerRegistry.RegisterForNavigation<TutorialPage1>();
            containerRegistry.RegisterForNavigation<TutorialPage2>();
            containerRegistry.RegisterForNavigation<TutorialPage3>();
            containerRegistry.RegisterForNavigation<PrivacyPolicyPage>();
            containerRegistry.RegisterForNavigation<TutorialPage4>();
            containerRegistry.RegisterForNavigation<TutorialPage5>();
            containerRegistry.RegisterForNavigation<TutorialPage6>();
            // Help
            containerRegistry.RegisterForNavigation<HelpMenuPage>();
            containerRegistry.RegisterForNavigation<HelpPage1>();
            containerRegistry.RegisterForNavigation<HelpPage2>();
            containerRegistry.RegisterForNavigation<HelpPage3>();
            containerRegistry.RegisterForNavigation<HelpPage4>();

            containerRegistry.RegisterForNavigation<PrivacyPolicyPage2>();
            containerRegistry.RegisterForNavigation<InqueryPage>();
            containerRegistry.RegisterForNavigation<ChatbotPage>();
            containerRegistry.RegisterForNavigation<TermsofservicePage>();
            containerRegistry.RegisterForNavigation<ThankYouNotifyOtherPage>();
            containerRegistry.RegisterForNavigation<NotifyOtherPage>();
            containerRegistry.RegisterForNavigation<NotContactPage>();
            containerRegistry.RegisterForNavigation<ContactedNotifyPage>();
            containerRegistry.RegisterForNavigation<SubmitConsentPage>();
            containerRegistry.RegisterForNavigation<ExposuresPage>();

            // Services
            containerRegistry.RegisterSingleton<UserDataService>();
            containerRegistry.RegisterSingleton<ExposureNotificationService>();
#if USE_MOCK
            containerRegistry.RegisterSingleton<IHttpDataService, HttpDataServiceMock>();
#else            
            containerRegistry.RegisterSingleton<IHttpDataService, HttpDataService>();
#endif
        }

        protected override void OnStart()
        {
        }

        protected override void OnResume()
        {
        }

        /*
         public async Task InitializeBackgroundTasks()
        {
            if (await Xamarin.ExposureNotifications.ExposureNotification.IsEnabledAsync())
                await Xamarin.ExposureNotifications.ExposureNotification.ScheduleFetchAsync();
        }
        */
        protected override void OnSleep()
        {
        }

        private void LogUnobservedTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                // maybe think local only logger
            };
        }
    }
}
