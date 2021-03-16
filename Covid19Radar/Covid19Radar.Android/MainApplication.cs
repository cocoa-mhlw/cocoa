using System;
using Android.App;
using Android.Runtime;
using Covid19Radar.Common;
using Prism;
using Prism.Ioc;
using Covid19Radar.Services.Logs;
using Covid19Radar.Droid.Services.Logs;
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

            ContainerInitializer.Initialize(new AndroidInitializer());
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Services
            containerRegistry.RegisterSingleton<ILogPathDependencyService, LogPathServiceAndroid>();
            containerRegistry.RegisterSingleton<ILogPeriodicDeleteService, LogPeriodicDeleteServiceAndroid>();
            containerRegistry.RegisterSingleton<ILogFileDependencyService, LogFileServiceAndroid>();
            containerRegistry.RegisterSingleton<ISecureStorageDependencyService, SecureStorageServiceAndroid>();
            containerRegistry.RegisterSingleton<IPreferencesService, PreferencesService>();
            containerRegistry.Register<ICloseApplication, CloseApplication>();
            containerRegistry.Register<IDeviceVerifier, DeviceCheckService>();
        }
    }
}
