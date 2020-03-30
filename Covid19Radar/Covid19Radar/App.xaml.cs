using System;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Covid19Radar.ViewModels;
using Covid19Radar.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Shiny;
using Prism.Mvvm;
using Covid19Radar.Shiny.Infrastructure;
using DryIoc;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Covid19Radar
{
    public partial class App : Prism.DryIoc.PrismApplication
    {
        protected override IContainerExtension CreateContainerExtension() => PrismContainerExtension.Current;

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

            var result = await NavigationService.NavigateAsync("NavigationPage/MainPage");

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
            // var container = containerRegistry.GetContainer();


            // Dialog
  //          containerRegistry.RegisterDialog<DialogView, DialogViewModel>();

            // Viewmodel
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<DescriptionPage, DescriptionPageViewModel>();
            containerRegistry.RegisterForNavigation<SmsVerificationPage, SmsVerificationPageViewModel>();
            containerRegistry.RegisterForNavigation<InputSmsOTPPage, InputSmsOTPPageViewModel>();
            containerRegistry.RegisterForNavigation<ConsentByUserPage, ConsentByUserPageViewModel>();
            containerRegistry.RegisterForNavigation<InitSettingPage, InitSettingPageViewModel>();
            containerRegistry.RegisterForNavigation<HomePage, HomePageViewModel>();

        }

        protected override void OnStart()
        {
            /*
            AppCenter.Start(Constants.AppCenterTokensAndroid +
                  Constants.AppCenterTokensIOS +
                  typeof(Analytics), typeof(Crashes));
                  */
            base.OnStart();
        }
    }
}
