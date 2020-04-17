using System;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Mvvm;
using DryIoc;
using ImTools;
using Covid19Radar.Model;
using System.Threading.Tasks;
using Prism.Navigation;
using Covid19Radar.Services;
using Prism.Services;
using Covid19Radar.Common;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using System.Net.Http;
using Prism.Logging.AppCenter;
using Prism.Logging;
using System.Collections.Generic;
using System.Text;

/* 
 * Our mission...is 
 * Empower every person and every organization on the planet achieve more.
 * Put an end to Covid 19
 */

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Covid19Radar
{
    public partial class App : PrismApplication
    {
        private IBeaconService _beaconService;
        const string LogTag = "Covid19Radar";
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        static App()
        {
            /*
            Crashes.SendingErrorReport += SendingErrorReportHandler;
            Crashes.SentErrorReport += SentErrorReportHandler;
            Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;
            */
        }

        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer, setFormsDependencyResolver: true) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            INavigationResult result;

            // AppCenter
            /*
            AppCenter.LogLevel = LogLevel.Debug;
            Crashes.ShouldProcessErrorReport = ShouldProcess;
            Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;
            Crashes.GetErrorAttachments = GetErrorAttachments;
            Distribute.ReleaseAvailable = OnReleaseAvailable;
            AppCenter.Start($"android={AppConstants.AppCenterTokensAndroid};ios={AppConstants.AppCenterTokensIOS};", typeof(Analytics), typeof(Crashes), typeof(Distribute));
            await AppCenter.GetInstallIdAsync().ContinueWith(installId =>
            {
                AppCenterLog.Info(LogTag, "AppCenter.InstallId=" + installId.Result);
            });
            await Crashes.HasCrashedInLastSessionAsync().ContinueWith(hasCrashed =>
            {
                AppCenterLog.Info(LogTag, "Crashes.HasCrashedInLastSession=" + hasCrashed.Result);
            });
            await Crashes.GetLastSessionCrashReportAsync().ContinueWith(report =>
            {
                AppCenterLog.Info(LogTag, "Crashes.LastSessionCrashReport.Exception=" + report.Result?.StackTrace);
            });
            Container.Resolve<ILogger>().Log("Started App Center");
            */


            // Check user data and skip tutorial
            UserDataService userDataService = Xamarin.Forms.DependencyService.Resolve<UserDataService>();
            //UserDataModel userData =await userDataService.Register();

            var isUserExists = await userDataService.IsExistUserDataAsync();
            if (isUserExists)
            {
                UserDataModel _userData = userDataService.Get();
                _beaconService = Xamarin.Forms.DependencyService.Resolve<IBeaconService>();
                // Only Call InitializeService! Start automagically!
                AppUtils.CheckPermission();
                _beaconService.InitializeService();

                result = await NavigationService.NavigateAsync("NavigationPage/HomePage");
            }
            else
            {
                result = await NavigationService.NavigateAsync("NavigationPage/StartTutorialPage");
            }

            if (!result.Success)
            {
                MainPage = new ExceptionPage
                {
                    BindingContext = new ExceptionPageViewModel()
                    {
                        Message = result.Exception.Message
                    }
                };
                System.Diagnostics.Debugger.Break();
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // logger
            /*
            var logger = new AppCenterLogger();
            containerRegistry.RegisterInstance<ILogger>(logger);
            containerRegistry.RegisterInstance<ILoggerFacade>(logger);
            */

            // Viewmodel
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<StartTutorialPage, StartTutorialPageViewModel>();
            containerRegistry.RegisterForNavigation<DescriptionPage, DescriptionPageViewModel>();
            containerRegistry.RegisterForNavigation<ConsentByUserPage, ConsentByUserPageViewModel>();
            containerRegistry.RegisterForNavigation<InitSettingPage, InitSettingPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<SmsVerificationPage, SmsVerificationPageViewModel>();
            containerRegistry.RegisterForNavigation<UserSettingPage, UserSettingPageViewModel>();
            containerRegistry.RegisterForNavigation<InputSmsOTPPage, InputSmsOTPPageViewModel>();
            containerRegistry.RegisterForNavigation<ContributersPage, ContributersPageViewModel>();
            containerRegistry.RegisterForNavigation<UpdateInfoPage, UpdateInfoPageViewModel>();
            containerRegistry.RegisterSingleton<UserDataService, UserDataService>();
            containerRegistry.RegisterSingleton<HttpDataService, HttpDataService>();
            containerRegistry.RegisterForNavigation<SetupCompletedPage, SetupCompletedPageViewModel>();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }


        // LogUnobservedTaskExceptions
        private void LogUnobservedTaskExceptions()
        {
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Container.Resolve<ILogger>().Report(e.Exception);
            };
        }

        /*

        static void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sending error report");

            var args = e as SendingErrorReportEventArgs;
            ErrorReport report = args.Report;

            //test some values
            if (report.StackTrace != null)
            {
                AppCenterLog.Info(LogTag, report.StackTrace.ToString());
            }
            else if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }
        }

        static void SentErrorReportHandler(object sender, SentErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sent error report");

            var args = e as SentErrorReportEventArgs;
            ErrorReport report = args.Report;

            //test some values
            if (report.StackTrace != null)
            {
                AppCenterLog.Info(LogTag, report.StackTrace.ToString());
            }
            else
            {
                AppCenterLog.Info(LogTag, "No system exception was found");
            }

            if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }
        }

        static void FailedToSendErrorReportHandler(object sender, FailedToSendErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Failed to send error report");

            var args = e as FailedToSendErrorReportEventArgs;
            ErrorReport report = args.Report;

            //test some values
            if (report.StackTrace  != null)
            {
                AppCenterLog.Info(LogTag, report.StackTrace.ToString());
            }
            else if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }

            if (e.Exception != null)
            {
                AppCenterLog.Info(LogTag, "There is an exception associated with the failure");
            }
        }

        bool ShouldProcess(ErrorReport report)
        {
            AppCenterLog.Info(LogTag, "Determining whether to process error report");
            return true;
        }

        bool ConfirmationHandler()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                Current.MainPage.DisplayActionSheet("Crash detected. Send anonymous crash report?", null, null, "Send", "Always Send", "Don't Send").ContinueWith((arg) =>
                {
                    var answer = arg.Result;
                    UserConfirmation userConfirmationSelection;
                    if (answer == "Send")
                    {
                        userConfirmationSelection = UserConfirmation.Send;
                    }
                    else if (answer == "Always Send")
                    {
                        userConfirmationSelection = UserConfirmation.AlwaysSend;
                    }
                    else
                    {
                        userConfirmationSelection = UserConfirmation.DontSend;
                    }
                    AppCenterLog.Debug(LogTag, "User selected confirmation option: \"" + answer + "\"");
                    Crashes.NotifyUserConfirmation(userConfirmationSelection);
                });
            });

            return true;
        }

        IEnumerable<ErrorAttachmentLog> GetErrorAttachments(ErrorReport report)
        {
            return new ErrorAttachmentLog[]
            {
//                ErrorAttachmentLog.AttachmentWithText("Hello world!", "hello.txt"),
//                ErrorAttachmentLog.AttachmentWithBinary(Encoding.UTF8.GetBytes("Fake image"), "fake_image.jpeg", "image/jpeg")
            };
        }

        bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            AppCenterLog.Info(LogTag, "OnReleaseAvailable id=" + releaseDetails.Id
                                            + " version=" + releaseDetails.Version
                                            + " releaseNotesUrl=" + releaseDetails.ReleaseNotesUrl);
            var custom = releaseDetails.ReleaseNotes?.ToLowerInvariant().Contains("custom") ?? false;
            if (custom)
            {
                var title = "Version " + releaseDetails.ShortVersion + " available!";
                Task answer;
                if (releaseDetails.MandatoryUpdate)
                {
                    answer = Current.MainPage.DisplayAlert(title, releaseDetails.ReleaseNotes, "Update now!");
                }
                else
                {
                    answer = Current.MainPage.DisplayAlert(title, releaseDetails.ReleaseNotes, "Update now!", "Maybe tomorrow...");
                }
                answer.ContinueWith((task) =>
                {
                    if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
                    {
                        Distribute.NotifyUpdateAction(UpdateAction.Update);
                    }
                    else
                    {
                        Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                    }
                });
            }
            return custom;
        }
*/
    }
}
