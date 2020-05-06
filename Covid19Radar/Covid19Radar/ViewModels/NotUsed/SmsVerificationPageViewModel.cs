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
        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public SmsVerificationPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Input Phone number";
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.TryGetValue<string>("phone_number", out var phoneNumber))
            {
                PhoneNumber = phoneNumber;
            }
        }

        public Command OnClickNext => (new Command(() =>
        {
            NavigationService.NavigateAsync($"InputSmsOTPPage?phone_number={PhoneNumber}");
        }));

    }
}
