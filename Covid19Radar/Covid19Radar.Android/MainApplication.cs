using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Covid19Radar.Common;
using Covid19Radar.Services.Logs;
using Covid19Radar.Droid.Services.Logs;
using DryIoc;
using CommonServiceLocator;
using Covid19Radar.Services;
using Covid19Radar.Droid.Services;

namespace Covid19Radar.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class MainApplication : Application
    {

        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            InitializeServiceLocator();
        }

        /**
         * Initialize IOC container what used by background worker.
         */
        private void InitializeServiceLocator()
        {
            var container = new Container();

            RegisterPlatformTypes(container);
            RegisterTypes(container);

            var serviceLocator = new ContainerServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        private void RegisterPlatformTypes(Container container)
        {
            container.Register<ILogPathDependencyService, LogPathServiceAndroid>(Reuse.Singleton);
            container.Register<ISecureStorageDependencyService, SecureStorageServiceAndroid>(Reuse.Singleton);
            container.Register<IPreferencesService, PreferencesService>(Reuse.Singleton);
        }

        private void RegisterTypes(Container container)
        {
            container.Register<ILoggerService, LoggerService>(Reuse.Singleton);
            container.Register<ILogFileService, LogFileService>(Reuse.Singleton);
            container.Register<ILogPathService, LogPathService>(Reuse.Singleton);
            container.Register<ILogPeriodicDeleteService, LogPeriodicDeleteService>(Reuse.Singleton);
            container.Register<ILogUploadService, LogUploadService>(Reuse.Singleton);
            container.Register<IEssentialsService, EssentialsService>(Reuse.Singleton);
            container.Register<IUserDataService, UserDataService>(Reuse.Singleton);
            container.Register<IExposureNotificationService, ExposureNotificationService>(Reuse.Singleton);
            container.Register<ITermsUpdateService, TermsUpdateService>(Reuse.Singleton);
            container.Register<IApplicationPropertyService, ApplicationPropertyService>(Reuse.Singleton);
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
    }

    public class ContainerServiceLocator : ServiceLocatorImplBase
    {
        private readonly Container _container;

        public ContainerServiceLocator(Container container)
        {
            _container = container;
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (_container == null) throw new ObjectDisposedException("container");
            return _container.Resolve(serviceType, key);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (_container == null) throw new ObjectDisposedException("container");
            return _container.ResolveMany(serviceType);
        }
    }
}