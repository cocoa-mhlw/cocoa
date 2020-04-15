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
    public class InputSmsOTPPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        public InputSmsOTPPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Input OTP";
        }

        public Command OnClickNext => (new Command(() =>
        {
            _navigationService.NavigateAsync("UserSettingPage");
        }));

    }
}
