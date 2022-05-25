using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace CovidRadar.UITestV2
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp
                    .Android
                    //.ApkFile(@"C:\Users\masakazu.yamamoto.63.INTRA\source\repos\Covid19Radar\precompiledApps\jp.go.mhlw.covid19radar_debug_dv_v1.4.0_1636696713.apk")
                    //.ApkFile(@"C:\Users\masakazu.yamamoto.63.INTRA\source\repos\Covid19Radar\Covid19Radar\Covid19Radar.Android\bin\Release\APP_PACKAGE_NAME.APP_PACKAGE_NAME-Signed.apk")
                    .ApkFile(@"C:\Users\masakazu.yamamoto.63.INTRA\source\repos\Covid19Radar\precompiledApps\APP_PACKAGE_NAME.APP_PACKAGE_NAME.apk")
                    .StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}
