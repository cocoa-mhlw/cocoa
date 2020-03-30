//#define STARTUP_ATTRIBUTES
//#define STARTUP_AUTO

using System;
using Shiny;
using Shiny.Logging;
using Microsoft.Extensions.DependencyInjection;
using Covid19Radar;
using Shiny.Infrastructure;
using Acr.UserDialogs.Forms;
using Shiny.Notifications;
using Shiny.Prism;
using Prism.DryIoc;
using Shiny.IO;
using System.IO;
using Covid19Radar.Shiny.Settings;
using Covid19Radar.Shiny.Delegates;
using Covid19Radar.Shiny.Infrastructure;
using Covid19Radar.Shiny.Models;
using Acr.UserDialogs;
using SQLite;


#if STARTUP_ATTRIBUTES
//[assembly: ShinySqliteIntegration(true, true, true, true, true)]
//[assembly: ShinyJob(typeof(SampleJob), "MyIdentifier", BatteryNotLow = true, DeviceCharging = false, RequiredInternetAccess = Shiny.Jobs.InternetAccess.Any)]
[assembly: ShinyAppCenterIntegration(Constants.AppCenterTokens, true, true)]
[assembly: ShinyService(typeof(ShinySqliteStateConnection))]
[assembly: ShinyService(typeof(GlobalExceptionHandler))]
[assembly: ShinyService(typeof(CoreDelegateServices))]
[assembly: ShinyService(typeof(JobLoggerTask))]
[assembly: ShinyService(typeof(IUserDialogs), typeof(UserDialogs))]
[assembly: ShinyService(typeof(IFullService), typeof(FullService))]
[assembly: ShinyService(typeof(IAppSettings), typeof(AppSettings))]

#if !STARTUP_AUTO
[assembly: ShinyNotifications(typeof(NotificationDelegate), true)]
[assembly: ShinyBeacons(typeof(BeaconDelegate))]
[assembly: ShinyBleCentral(typeof(BleCentralDelegate))]
[assembly: ShinyGps(typeof(LocationDelegates))]
[assembly: ShinyGeofences(typeof(LocationDelegates))]
[assembly: ShinyMotionActivity]
[assembly: ShinySensors]
[assembly: ShinyHttpTransfers(typeof(HttpTransferDelegate))]
[assembly: ShinySpeechRecognition]
[assembly: ShinyPush(typeof(PushDelegate))]
[assembly: ShinyNfc]
#endif
#endif

namespace Covid19Radar.Shiny
{
    public class ShinyAppStartup : PrismStartup
    {
        public ShinyAppStartup() : base(PrismContainerExtension.Current)
        {

        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            Log.UseConsole();
            Log.UseDebug();
            services.UseMemoryCache();
//            services.UseAppCenterLogging(Constants.AppCenterTokensAndroid+Constants.AppCenterTokensIOS, true, true);
//            services.UseSqliteLogging(true, true);
//            services.UseSqliteCache();
//            services.UseSqliteSettings();
//            services.UseSqliteStorage();

#if STARTUP_ATTRIBUTES
            services.RegisterModule(new AssemblyServiceModule());
#if STARTUP_AUTO
            services.RegisterModule(new AutoRegisterModule());
#endif
#else
            UseAllServices(services);
#endif
        }


        static void UseAllServices(IServiceCollection services)
        {
            // your infrastructure

            services.AddSingleton<ShinySqliteStateConnection>();
            services.AddSingleton<CoreDelegateServices>();
//            services.AddSingleton<IUserDialogs, UserDialogs>();
           services.AddSingleton<IAppSettings, AppSettings>();

            // startup tasks
// TODO 下のハンドラは後程チェック(Exceptionがでる
            //services.AddSingleton<GlobalExceptionHandler>();
            //services.AddSingleton<IFullService, FullService>();
            //services.AddSingleton<JobLoggerTask>();
            //services.AddAppState<AppStateDelegate>();

            // register all of the shiny stuff you want to use
            //services.UseJobForegroundService(TimeSpan.FromSeconds(30));
            //services.UseHttpTransfers<HttpTransferDelegate>();
            services.UseBeacons<BeaconDelegate>();
            //services.UseBleCentral<BleCentralDelegate>();
            //services.UseBlePeripherals();
            //services.UseMotionActivity();
            //services.UseSpeechRecognition();
            //services.UseAllSensors();
            //services.UseNfc();

            //services.UseGeofencing<LocationDelegates>();
            //services.UseGpsDirectGeofencing<LocationDelegates>();
            //services.UseGps<LocationDelegates>();

            //services.UseNotifications(true);
            services.UseNotifications<NotificationDelegate>(
                true,
                new NotificationCategory(
                    "Test",
                    new NotificationAction("Reply", "Reply", NotificationActionType.TextReply),
                    new NotificationAction("Yes", "Yes", NotificationActionType.None),
                    new NotificationAction("No", "No", NotificationActionType.Destructive)
                )
            );

            //services.UsePushNotifications<PushDelegate>();
            //services.UseFirebaseMessaging<PushDelegate>();
            //services.UsePushAzureNotificationHubs<PushDelegate>(
            //    "Endpoint=sb://shinysamples.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=jI6ss5WOD//xPNuHFJmS7sWWzqndYQyz7wAVOMTZoLE=",
            //    "shinysamples"
            //);
        }

    }
}
