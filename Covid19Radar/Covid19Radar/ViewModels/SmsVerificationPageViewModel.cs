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

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public SmsVerificationPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Input Phone number";
        }

        public Command OnClickNext => (new Command(() =>
        {
            _navigationService.NavigateAsync($"InputSmsOTPPage?phone_number={PhoneNumber}");
        }));

    }
}
