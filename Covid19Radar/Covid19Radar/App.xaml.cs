/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
using System;
using CommonServiceLocator;
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

        public App(IPlatformInitializer initializer) : base(initializer, setFormsDependencyResolver: true) { }

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

        public static void UseMockExposureNotificationImplementationIfNeeded()
        {
#if USE_MOCK
            // For debug mode, set the mock api provider to interact
            // with some fake data
            Xamarin.ExposureNotifications.ExposureNotification.OverrideNativeImplementation(new Services.TestNativeImplementation());
#endif
        }

        // Initialize IOC container
        public static void InitializeServiceLocator(Action<IContainer> registerPlatformTypes)
        {
            var container = new Container(GetContainerRules());

            registerPlatformTypes(container);
            RegisterCommonTypes(container);

            var serviceLocator = new ContainerServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        private static Rules GetContainerRules()
        {
            return Rules.Default.WithAutoConcreteTypeResolution()
                    .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                    .WithoutFastExpressionCompiler()
                    .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Throw);
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            var container = (ServiceLocator.Current as ContainerServiceLocator).CopyContainerWithRegistrations();
            return new DryIocContainerExtension(container);
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

        private static void RegisterCommonTypes(IContainer container)
        {
            // Services
            container.Register<ILoggerService, LoggerService>(Reuse.Singleton);
            container.Register<ILogFileService, LogFileService>(Reuse.Singleton);
            container.Register<ILogPathService, LogPathService>(Reuse.Singleton);
            container.Register<ILogPeriodicDeleteService, LogPeriodicDeleteService>(Reuse.Singleton);
            container.Register<ILogUploadService, LogUploadService>(Reuse.Singleton);
            container.Register<ILogWriter, LogWriter>(Reuse.Singleton);
            container.Register<IEssentialsService, EssentialsService>(Reuse.Singleton);
            container.Register<IUserDataService, UserDataService>(Reuse.Singleton);
            container.Register<IExposureNotificationService, ExposureNotificationService>(Reuse.Singleton);
            container.Register<ITermsUpdateService, TermsUpdateService>(Reuse.Singleton);
            container.Register<IHttpClientService, HttpClientService>(Reuse.Singleton);
#if USE_MOCK
            container.Register<IHttpDataService, HttpDataServiceMock>(Reuse.Singleton);
            container.Register<IStorageService, StorageServiceMock>(Reuse.Singleton);
#else
            container.Register<IHttpDataService, HttpDataService>(Reuse.Singleton);
            container.Register<IStorageService, StorageService>(Reuse.Singleton);
#endif
            container.Register<ISecureStorageService, SecureStorageService>(Reuse.Singleton);
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
            base.OnResume();
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
            base.OnSleep();
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
