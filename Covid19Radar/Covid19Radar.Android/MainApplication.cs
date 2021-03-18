using System;
using Android.App;
using Android.Runtime;
using Covid19Radar.Services.Logs;
using Covid19Radar.Droid.Services.Logs;
using DryIoc;
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

            App.InitializeServiceLocator(RegisterPlatformTypes);

            App.UseMockExposureNotificationImplementationIfNeeded();
        }

        private void RegisterPlatformTypes(IContainer container)
        {
            container.Register<ILogPathDependencyService, LogPathServiceAndroid>(Reuse.Singleton);
            container.Register<ISecureStorageDependencyService, SecureStorageServiceAndroid>(Reuse.Singleton);
            container.Register<IPreferencesService, PreferencesService>(Reuse.Singleton);
        }
    }
}