using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DryIoc;
using System.Threading.Tasks;
using Prism.Navigation;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Common;

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
        private ILoggerService LoggerService;
        private ILogFileService LogFileService;

        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor.
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            LoggerService = Container.Resolve<ILoggerService>();
            LoggerService.StartMethod();
            LogFileService = Container.Resolve<ILogFileService>();
            LogFileService.AddSkipBackupAttribute();

            Xamarin.ExposureNotifications.ExposureNotification.Init();

            // Local Notification tap event listener
            //NotificationCenter.Current.NotificationTapped += OnNotificationTapped;
            LogUnobservedTaskExceptions();

            INavigationResult result = await NavigationService.NavigateAsync("/" + nameof(SplashPage));

            if (!result.Success)
            {
                LoggerService.Info($"Failed transition.");

                MainPage = new ExceptionPage
                {
                    BindingContext = new ExceptionPageViewModel()
                    {
                        Message = result.Exception.Message
                    }
                };
                System.Diagnostics.Debugger.Break();
            }

            LoggerService.EndMethod();
        }

        //protected void OnNotificationTapped(NotificationTappedEventArgs e)
        //{
        //    NavigationService.NavigateAsync(nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
        //}

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Base and Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MenuPage>();
            containerRegistry.RegisterForNavigation<HomePage>();


            // Settings
            containerRegistry.RegisterForNavigation<SettingsPage>();
            containerRegistry.RegisterForNavigation<LicenseAgreementPage>();

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
            containerRegistry.RegisterForNavigation<SendLogConfirmationPage>();
            containerRegistry.RegisterForNavigation<SendLogCompletePage>();

            containerRegistry.RegisterForNavigation<PrivacyPolicyPage2>();
            containerRegistry.RegisterForNavigation<InqueryPage>();
            containerRegistry.RegisterForNavigation<TermsofservicePage>();
            containerRegistry.RegisterForNavigation<ThankYouNotifyOtherPage>();
            containerRegistry.RegisterForNavigation<NotifyOtherPage>();
            containerRegistry.RegisterForNavigation<NotContactPage>();
            containerRegistry.RegisterForNavigation<ContactedNotifyPage>();
            containerRegistry.RegisterForNavigation<SubmitConsentPage>();
            containerRegistry.RegisterForNavigation<ExposuresPage>();
            containerRegistry.RegisterForNavigation<ReAgreePrivacyPolicyPage>();
            containerRegistry.RegisterForNavigation<ReAgreeTermsOfServicePage>();
            containerRegistry.RegisterForNavigation<SplashPage>();
        }

        protected override void OnStart()
        {
            // Initialize periodic log delete service
            var logPeriodicDeleteService = Container.Resolve<ILogPeriodicDeleteService>();
            logPeriodicDeleteService.Init();

            LogFileService.Rotate();
        }

        protected override void OnResume()
        {
            LogFileService.Rotate();
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

        protected override IContainerExtension CreateContainerExtension()
        {
            return new DryIocContainerExtension(ContainerLocator.Container.GetContainer());
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
