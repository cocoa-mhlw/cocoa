using System.Collections.Generic;
using System.Linq;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;
using System;
using System.Windows.Input;
using Prism.Navigation.Xaml;
using Acr.UserDialogs;
using Covid19Radar.Renderers;

namespace Covid19Radar.ViewModels
{
    public class NotifyOtherPageViewModel : ViewModelBase
    {
        public NotifyOtherPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resources.AppResources.TitileUserStatusSettings;
        }

        public Command SharePositiveDiagnosisCommand => (new Command(async () =>
       {
           await NavigationService.NavigateAsync("SharePositiveDiagnosisPage");
       }));

        public Command LearnMoreCommand => (new Command(async () =>
        {
            await Xamarin.Essentials.Browser.OpenAsync(Resources.AppResources.NotifyOthersLearnMoreUrl);
        }));
    }
}
