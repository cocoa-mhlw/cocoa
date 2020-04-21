using System;
using Xamarin.UITest;
using Xamariners.EndToEnd.Xamarin.Models;

namespace Covid19Radar.UITest
{
    public class AppInitializer
    {

        public static IApp StartApp(Platform platform, RunnerConfiguration runnerConfiguration)
        {
            try
            {
                if (platform == Platform.Android)
                {
                    return ConfigureApp
                        .Android
                        .ApkFile(runnerConfiguration.AndroidConfiguration.AppPath)
                        .EnableLocalScreenshots()
                        .StartApp();
                }

                if (runnerConfiguration.IosConfiguration.Simulator)
                {
                    return ConfigureApp
                        .iOS
                        .DeviceIdentifier(runnerConfiguration.IosConfiguration.SimulatorIdentifier)
                        .AppBundle(runnerConfiguration.IosConfiguration.AppPath)
                        .EnableLocalScreenshots()
                        .StartApp();
                }

                var iosDeviceIdentifier = runnerConfiguration.IosConfiguration.DeviceIdentifier;
                var iosBundleName = runnerConfiguration.IosConfiguration.BundleName;

                return ConfigureApp
                    .iOS
                    .DeviceIdentifier(iosDeviceIdentifier)
                    .InstalledApp(iosBundleName)
                    .PreferIdeSettings()
                    .EnableLocalScreenshots()
                    .StartApp();

            }
            catch (Exception ex)
            {
                var asd = 3;
                throw;
            }
        }
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android.StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }

    }
}