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
    public class SmsVerificationPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        public SmsVerificationPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "電話番号入力";
        }

        public Command OnClickNext => (new Command(() =>
        {
            _navigationService.NavigateAsync("InputSmsOTPPage");
        }));

    }
}
