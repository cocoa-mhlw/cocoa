/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Covid19Radar.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using Prism.Navigation;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using System;
using Covid19Radar.Common;
using Covid19Radar.Services.Migration;
using Covid19Radar.Repository;
using DryIoc;
using Covid19Radar.Views.EndOfService;
using Covid19Radar.Model;

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

        // Workaround for fixing DryIoc.ContainerException.
        // https://github.com/PrismLibrary/Prism/issues/2529
        private static bool FirstLoad = true;

        private ILoggerService LoggerService;
        private ILogFileService LogFileService;
        private IBackupAttributeService BackupAttributeService;

        private IEventLogRepository EventLogRepository { get; set; }
        private IEventLogService EventLogService { get; set; }

        /*
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor.
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer, setFormsDependencyResolver: false) { }

        protected override void OnInitialized()
        {
            InitializeComponent();

            LoggerService = Container.Resolve<ILoggerService>();
            LoggerService.StartMethod();
            LogFileService = Container.Resolve<ILogFileService>();
            LogFileService.SetSkipBackupAttributeToLogDir();
            BackupAttributeService = Container.Resolve<IBackupAttributeService>();
            BackupAttributeService.SetSkipBackupAttributeToEventLogDir();

            EventLogRepository = Container.Resolve<IEventLogRepository>();
            EventLogService = Container.Resolve<IEventLogService>();

            LogUnobservedTaskExceptions();

            FirstLoad = false;

            LoggerService.EndMethod();
        }

        public async Task<INavigationResult> NavigateToSplashAsync(Destination destination, INavigationParameters navigationParameters)
        {
            LoggerService.Info($"Destination: {destination}");

            var navigationParams = SplashPage.BuildNavigationParams(destination, navigationParameters);
            return await NavigationService.NavigateAsync(Destination.SplashPage.ToPath(), navigationParams);
        }

        public async Task<INavigationResult> NavigateToAsync(Destination destination, INavigationParameters navigationParameters)
        {
            LoggerService.StartMethod();

            try
            {
                return await NavigationService.NavigateAsync(destination.ToPath(), navigationParameters);
            }
            finally
            {
                LoggerService.EndMethod();
            }
        }

        // Initialize IOC container
        public static void InitializeServiceLocator(Action<IContainer> registerPlatformTypes)
        {
            if (!FirstLoad)
            {
                return;
            }

            var container = new Container(GetContainerRules());

            registerPlatformTypes(container);
            RegisterCommonTypes(container);

            PrismContainerExtension.Init(container);
            ContainerLocator.SetContainerExtension(() => PrismContainerExtension.Current);
        }

        private static Rules GetContainerRules()
        {
            return Rules.Default.WithAutoConcreteTypeResolution()
                    .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                    .WithoutFastExpressionCompiler()
                    .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Throw);
        }

        // Workaround for fixing DryIoc.ContainerException.
        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            if (!FirstLoad)
            {
                return;
            }

            base.RegisterRequiredTypes(containerRegistry);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Workaround for fixing DryIoc.ContainerException.
            if (!FirstLoad)
            {
                return;
            }

            // Base and Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MenuPage>();
            containerRegistry.RegisterForNavigation<HomePage>();
#if DEBUG
            containerRegistry.RegisterForNavigation<DebugPage>();
            containerRegistry.RegisterForNavigation<EditServerConfigurationPage>();
            containerRegistry.RegisterForNavigation<ManageExposureDataPage>();
            containerRegistry.RegisterForNavigation<ManageUserDataPage>();
#endif

            // Settings
            containerRegistry.RegisterForNavigation<SettingsPage>();
            containerRegistry.RegisterForNavigation<LicenseAgreementPage>();
            containerRegistry.RegisterForNavigation<EventLogSettingPage>();

            // tutorial
            containerRegistry.RegisterForNavigation<TutorialPage1>();
            containerRegistry.RegisterForNavigation<TutorialPage2>();
            containerRegistry.RegisterForNavigation<TutorialPage3>();
            containerRegistry.RegisterForNavigation<PrivacyPolicyPage>();
            containerRegistry.RegisterForNavigation<TutorialPage4>();
            containerRegistry.RegisterForNavigation<TutorialPage6>();
            containerRegistry.RegisterForNavigation<EventLogCooperationPage>();

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
            containerRegistry.RegisterForNavigation<NotifyOtherPage>();
            containerRegistry.RegisterForNavigation<ExposureCheckPage>();
            containerRegistry.RegisterForNavigation<ContactedNotifyPage>();
            containerRegistry.RegisterForNavigation<SubmitConsentPage>();
            containerRegistry.RegisterForNavigation<SubmitDiagnosisKeysCompletePage>();
            containerRegistry.RegisterForNavigation<ExposuresPage>();
            containerRegistry.RegisterForNavigation<ReAgreePrivacyPolicyPage>();
            containerRegistry.RegisterForNavigation<ReAgreeTermsOfServicePage>();
            containerRegistry.RegisterForNavigation<SplashPage>();
            containerRegistry.RegisterForNavigation<HowToEnableExposureNotificationsPage>();
            containerRegistry.RegisterForNavigation<HowToReceiveProcessingNumberPage>();
            containerRegistry.RegisterForNavigation<WebAccessibilityPolicyPage>();
            containerRegistry.RegisterForNavigation<TroubleshootingPage>();

            // End of service
            containerRegistry.RegisterForNavigation<EndOfServiceNoticePage>();
            containerRegistry.RegisterForNavigation<EndOfServicePage>();
            containerRegistry.RegisterForNavigation<TerminationOfUsePage>();
            containerRegistry.RegisterForNavigation<TerminationOfUseCompletePage>();
        }

        private static void RegisterCommonTypes(IContainer container)
        {
            // Services
            container.Register<IUserDataRepository, UserDataRepository>(Reuse.Singleton);
            container.Register<ILoggerService, LoggerService>(Reuse.Singleton);
            container.Register<ILogFileService, LogFileService>(Reuse.Singleton);
            container.Register<ILogPathService, LogPathService>(Reuse.Singleton);
            container.Register<ILogUploadService, LogUploadService>(Reuse.Singleton);
            container.Register<IEssentialsService, EssentialsService>(Reuse.Singleton);
            container.Register<IUserDataService, UserDataService>(Reuse.Singleton);
            container.Register<IHttpClientService, HttpClientService>(Reuse.Singleton);
            container.Register<IMigrationService, MigrationService>(Reuse.Singleton);
            container.Register<IExposureDataExportService, ExposureDataExportService>(Reuse.Singleton);

#if USE_MOCK
            container.Register<IHttpDataService, HttpDataServiceMock>(Reuse.Singleton);
            container.Register<IStorageService, StorageServiceMock>(Reuse.Singleton);
            container.Register<ITermsUpdateService, TermsUpdateServiceMock>(Reuse.Singleton);
#else
            container.Register<IHttpDataService, HttpDataService>(Reuse.Singleton);
            container.Register<IStorageService, StorageService>(Reuse.Singleton);
            container.Register<ITermsUpdateService, TermsUpdateService>(Reuse.Singleton);
#endif

#if DEBUG
            container.Register<IServerConfigurationRepository, DebugServerConfigurationRepository>(Reuse.Singleton);
            container.Register<IDebugExposureDataCollectServer, DebugExposureDataCollectServer>(Reuse.Singleton);
#else
            container.Register<IServerConfigurationRepository, ReleaseServerConfigurationRepository>(Reuse.Singleton);
            container.Register<IDebugExposureDataCollectServer, DebugExposureDataCollectServerNop>(Reuse.Singleton);
#endif

            container.Register<IExposureDataRepository, ExposureDataRepository>(Reuse.Singleton);
            container.Register<IDiagnosisKeyRegisterServer, DiagnosisKeyRegisterServer>(Reuse.Singleton);
            container.Register<IDialogService, DialogService>(Reuse.Singleton);
            container.Register<ISecureStorageService, SecureStorageService>(Reuse.Singleton);
            container.Register<IExposureDetectionService, ExposureDetectionService>(Reuse.Singleton);
            container.Register<IExposureRiskCalculationService, ExposureRiskCalculationService>(Reuse.Singleton);
            container.Register<IDiagnosisKeyRepository, DiagnosisKeyRepository>(Reuse.Singleton);
            container.Register<IExposureConfigurationRepository, ExposureConfigurationRepository>(Reuse.Singleton);
            container.Register<IExposureRiskCalculationConfigurationRepository, ExposureRiskCalculationConfigurationRepository>(Reuse.Singleton);
            container.Register<ICheckVersionService, CheckVersionService>(Reuse.Singleton);

            container.Register<ISendEventLogStateRepository, SendEventLogStateRepository>(Reuse.Singleton);
            container.Register<IEventLogRepository, EventLogRepository>(Reuse.Singleton);
            container.Register<IEventLogService, EventLogService>(Reuse.Singleton);

            container.Register<ISplashNavigationService, SplashNavigationService>(Reuse.Singleton);

            // Utilities
            container.Register<IDateTimeUtility, DateTimeUtility>(Reuse.Singleton);
            container.Register<IDeviceInfoUtility, DeviceInfoUtility>(Reuse.Singleton);
        }

        protected override void OnStart()
        {
            base.OnStart();
            ExecuteBackgroundTask();
        }

        protected override void OnResume()
        {
            base.OnResume();
            ExecuteBackgroundTask();
        }

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

        private void ExecuteBackgroundTask()
        {
            LoggerService.StartMethod();

            try
            {
                LogFileService.Rotate();

            }
            catch (Exception ex)
            {
                LoggerService.Exception("Failed to manually execute background task", ex);
            }
            finally
            {
                LoggerService.EndMethod();
            }
        }
    }
}
