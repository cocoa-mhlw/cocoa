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

        UserData userData;
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            INavigationResult result;

            // Check user data and skip tutorial
            UserData userData = Container.Resolve<UserData>();
            if (Application.Current.Properties.ContainsKey("UserData"))
            {
                result = await NavigationService.NavigateAsync("NavigationPage/BeaconPage");
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
            // Viewmodel
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<StartTutorialPage, StartTutorialPageViewModel>();
            containerRegistry.RegisterForNavigation<DescriptionPage, DescriptionPageViewModel>();
            containerRegistry.RegisterForNavigation<SmsVerificationPage, SmsVerificationPageViewModel>();
            containerRegistry.RegisterForNavigation<InputSmsOTPPage, InputSmsOTPPageViewModel>();
            containerRegistry.RegisterForNavigation<ConsentByUserPage, ConsentByUserPageViewModel>();
            containerRegistry.RegisterForNavigation<InitSettingPage, InitSettingPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();
            containerRegistry.RegisterForNavigation<BeaconPage, BeaconPageViewModel>();
            containerRegistry.RegisterForNavigation<DemoPage, DemoPageViewModel>();

            // CheckUser
            UserData userData = new UserData();

            if (Application.Current.Properties.ContainsKey("UserData"))
            {
                userData = Application.Current.Properties["UserData"] as UserData;
            }
            containerRegistry.RegisterInstance<UserData>(userData);

        }

        protected override void OnStart()
        {
            AppCenter.Start($"android={AppConstants.AppCenterTokensAndroid};ios={AppConstants.AppCenterTokensIOS};",
                  typeof(Analytics), typeof(Crashes), typeof(Distribute));
            base.OnStart();
        }
    }
}
