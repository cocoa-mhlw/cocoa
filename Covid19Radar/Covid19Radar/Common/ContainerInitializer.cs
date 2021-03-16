using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using DryIoc;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;

namespace Covid19Radar.Common
{
    public static class ContainerInitializer
    {
        public static void Initialize(IPlatformInitializer platformInitializer)
        {
            InitializeContainerLocator(platformInitializer);
        }

        private static void InitializeContainerLocator(IPlatformInitializer platformInitializer)
        {
            ContainerLocator.SetContainerExtension(CreateContainerExtension);
            platformInitializer.RegisterTypes(ContainerLocator.Current);
            RegisterCommonTypes(ContainerLocator.Current);
        }

        private static void RegisterCommonTypes(IContainerRegistry containerRegistry)
        {
            // Services
            containerRegistry.RegisterSingleton<ILoggerService, LoggerService>();
            containerRegistry.RegisterSingleton<ILogFileService, LogFileService>();
            containerRegistry.RegisterSingleton<ILogPathService, LogPathService>();
            containerRegistry.RegisterSingleton<ILogUploadService, LogUploadService>();
            containerRegistry.RegisterSingleton<IEssentialsService, EssentialsService>();
            containerRegistry.RegisterSingleton<IUserDataService, UserDataService>();
            containerRegistry.RegisterSingleton<IExposureNotificationService, ExposureNotificationService>();
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
            containerRegistry.RegisterSingleton<ISecureStorageService, SecureStorageService>();
        }

        /// <summary>
        /// Creates the <see cref="IContainerExtension"/> for DryIoc
        /// </summary>
        /// <remarks>
        /// copy from Prism.DryIoc.Forms/PrismApplication.cs
        /// </remarks>
        /// <returns></returns>
        private static IContainerExtension CreateContainerExtension()
        {
            return new DryIocContainerExtension(new Container(CreateContainerRules()));
        }

        /// <summary>
        /// Create <see cref="Rules" /> to alter behavior of <see cref="IContainer" />
        /// </summary>
        /// <remarks>
        /// backport from Prism.DryIoc.Forms/PrismApplication.cs of Prism8.0
        /// </remarks>
        /// <returns>An instance of <see cref="Rules" /></returns>
        private static Rules CreateContainerRules() => DefaultRules;

        /// <summary>
        /// Gets the Default DryIoc Container Rules used by Prism
        /// </summary>
        /// <remarks>
        /// backport from DryIocContainerExtension.cs of Prism8.0
        /// </remarks>

        public static Rules DefaultRules => Rules.Default.WithAutoConcreteTypeResolution()
                                                                       .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
#if HAS_WINUI || __IOS__
                                                                       .WithoutFastExpressionCompiler()
#endif
#if HAS_WINUI
                                                                       .WithTrackingDisposableTransients()
#endif
                                                                       .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace);
    }
}
