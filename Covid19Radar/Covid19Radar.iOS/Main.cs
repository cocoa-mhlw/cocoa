using Covid19Radar.iOS.Services;
using Covid19Radar.iOS.Services.Logs;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using DryIoc;
using UIKit;

namespace Covid19Radar.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            App.InitializeServiceLocator(RegisterPlatformTypes);

            App.UseMockExposureNotificationImplementationIfNeeded();

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }

        private static void RegisterPlatformTypes(Container container)
        {
            container.Register<ILogPathDependencyService, LogPathServiceIos>(Reuse.Singleton);
            container.Register<ISecureStorageDependencyService, SecureStorageServiceIos>(Reuse.Singleton);
            container.Register<IPreferencesService, PreferencesService>(Reuse.Singleton);
        }
    }
}
