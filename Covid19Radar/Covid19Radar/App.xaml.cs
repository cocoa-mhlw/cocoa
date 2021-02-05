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
using System.IO.IsolatedStorage;

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

        public App(IPlatformInitializer initializer) : base(initializer, setFormsDependencyResolver: true) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            LoggerService = Container.Resolve<ILoggerService>();
            LoggerService.StartMethod();
            LogFileService = Container.Resolve<ILogFileService>();
            LogFileService.AddSkipBackupAttribute();

            RecoverLostPropertiesFile(LoggerService);

#if USE_MOCK
            // For debug mode, set the mock api provider to interact
            // with some fake data
            Xamarin.ExposureNotifications.ExposureNotification.OverrideNativeImplementation(new Services.TestNativeImplementation());
#endif
            Xamarin.ExposureNotifications.ExposureNotification.Init();

            // Local Notification tap event listener
            //NotificationCenter.Current.NotificationTapped += OnNotificationTapped;
            LogUnobservedTaskExceptions();

            INavigationResult result;
            // Check user data and skip tutorial
            IUserDataService userDataService = Container.Resolve<IUserDataService>();

            if (userDataService.IsExistUserData)
            {
                LoggerService.Info("User data exists");
                var userData = userDataService.Get();
                LoggerService.Info($"userData.IsOptined: {userData.IsOptined}");
                LoggerService.Info($"userData.IsPolicyAccepted: {userData.IsPolicyAccepted}");
                if (userData.IsOptined && userData.IsPolicyAccepted)
                {
                    LoggerService.Info($"Transition to SplashPage");
                    result = await NavigationService.NavigateAsync("/" + nameof(SplashPage));
                }
                else
                {
                    LoggerService.Info($"Transition to TutorialPage1");
                    result = await NavigationService.NavigateAsync("/" + nameof(TutorialPage1));
                }
            }
            else
            {
                LoggerService.Info("No user data exists");
                LoggerService.Info($"Transition to TutorialPage1");
                result = await NavigationService.NavigateAsync("/" + nameof(TutorialPage1));
            }

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
            containerRegistry.RegisterForNavigation<SendLogConfirmationPage>();
            containerRegistry.RegisterForNavigation<SendLogCompletePage>();

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
            containerRegistry.RegisterForNavigation<ReAgreePrivacyPolicyPage>();
            containerRegistry.RegisterForNavigation<ReAgreeTermsOfServicePage>();
            containerRegistry.RegisterForNavigation<SplashPage>();

            // Services
            containerRegistry.RegisterSingleton<ILoggerService, LoggerService>();
            containerRegistry.RegisterSingleton<ILogFileService, LogFileService>();
            containerRegistry.RegisterSingleton<ILogPathService, LogPathService>();
            containerRegistry.RegisterSingleton<ILogPeriodicDeleteService, LogPeriodicDeleteService>();
            containerRegistry.RegisterSingleton<ILogUploadService, LogUploadService>();
            containerRegistry.RegisterSingleton<IEssentialsService, EssentialsService>();
            containerRegistry.RegisterSingleton<IUserDataService, UserDataService>();
            containerRegistry.RegisterSingleton<ExposureNotificationService>();
            containerRegistry.RegisterSingleton<ITermsUpdateService, TermsUpdateService>();
            containerRegistry.RegisterSingleton<IApplicationPropertyService, ApplicationPropertyService>();
            containerRegistry.RegisterSingleton<IHttpClientService, HttpClientService>();
#if USE_MOCK
            containerRegistry.RegisterSingleton<IHttpDataService, HttpDataServiceMock>();
            containerRegistry.RegisterSingleton<IStorageService, StorageServiceMock>();
#else            
            containerRegistry.RegisterSingleton<IHttpDataService, HttpDataService>();
            containerRegistry.RegisterSingleton<IStorageService, StorageService>();
#endif
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

        private void LogUnobservedTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                // maybe think local only logger
            };
        }

        // Workaround for possible data loss of Application.Current.Properties .
        // See https://github.com/xamarin/Xamarin.Forms/issues/13676
        private void RecoverLostPropertiesFile(ILoggerService LoggerService)
        {
            const string PropertyStoreFile = "PropertyStore.forms";
            const string PropertyStoreTmpFile = PropertyStoreFile + ".tmp";
            var store = IsolatedStorageFile.GetUserStoreForApplication();
            if (store.FileExists(PropertyStoreTmpFile))
            {
                if (store.FileExists(PropertyStoreFile))
                {
                    // Empty file could be exist because current impl of Xamarin.Forms uses System.IO.FileMode.OpenOrCreate for reading file (!).
                    using (var stream = store.OpenFile(PropertyStoreFile, System.IO.FileMode.Open))
                    {
                        if (stream.Length > 0)
                        {
                            // tmp file exists, but the store file contains data.
                            return;
                        }
                    }
                    // delete empty file
                    store.DeleteFile(PropertyStoreFile);
                }
                // tmp file exists while primary file is deleted.
                // It means tmp file is fully written (before store.DeleteFile()).

                LoggerService.Warning($"{PropertyStoreFile} is not found or empty, but tmp file {PropertyStoreTmpFile} is found. Recovering from tmp file.");
                store.MoveFile(PropertyStoreTmpFile, PropertyStoreFile);
            }
        }
    }
}
