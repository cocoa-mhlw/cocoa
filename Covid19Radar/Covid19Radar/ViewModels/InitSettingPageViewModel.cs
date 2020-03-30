using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InitSettingPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        public InitSettingPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "デバイスへのアクセス許可";
        }

        public Command OnClickNext => (new Command(() =>
        {
            _navigationService.NavigateAsync("HomePage");
        }));

    }
}
