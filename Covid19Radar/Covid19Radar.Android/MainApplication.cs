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

        private void RegisterPlatformTypes(Container container)
        {
            container.Register<ILogPathDependencyService, LogPathServiceAndroid>(Reuse.Singleton);
            container.Register<ISecureStorageDependencyService, SecureStorageServiceAndroid>(Reuse.Singleton);
            container.Register<IPreferencesService, PreferencesService>(Reuse.Singleton);
        }
    }
}