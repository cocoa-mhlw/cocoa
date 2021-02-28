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

#if USE_MOCK
            // For debug mode, set the mock api provider to interact
            // with some fake data
            Xamarin.ExposureNotifications.ExposureNotification.OverrideNativeImplementation(new TestNativeImplementation());
#endif
            Xamarin.ExposureNotifications.ExposureNotification.Init();
        }

        /**
         * Initialize IOC container what used by background worker.
         */
        private void InitializeServiceLocator()
        {
            var container = new Container(CreateContainerRules());

            RegisterPlatformTypes(container);
            App.RegisterCommonTypes(container);

            var serviceLocator = new ContainerServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }

        private Rules CreateContainerRules()
        {
            return Rules.Default.WithAutoConcreteTypeResolution()
                    .With(Made.Of(FactoryMethod.ConstructorWithResolvableArguments))
                    .WithoutFastExpressionCompiler()
                    .WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.Replace);
        }

        private void RegisterPlatformTypes(Container container)
        {
            container.Register<ILogPathDependencyService, LogPathServiceAndroid>(Reuse.Singleton);
            container.Register<ISecureStorageDependencyService, SecureStorageServiceAndroid>(Reuse.Singleton);
            container.Register<IPreferencesService, PreferencesService>(Reuse.Singleton);
        }
    }
}