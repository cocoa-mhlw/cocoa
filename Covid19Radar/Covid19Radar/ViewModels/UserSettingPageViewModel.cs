using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class UserSettingPageViewModel : ViewModelBase
    {
        private string _phoneNumber;

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                SetProperty(ref _phoneNumber, value);
                RaisePropertyChanged(nameof(IsPhoneNumberValid));
            }
        }

        public bool IsPhoneNumberValid => !string.IsNullOrWhiteSpace(PhoneNumber);

        public UserSettingPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = Resx.AppResources.TitleStatusSettings;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            PhoneNumber = string.Empty;
        }

        public Command OnClickNext => new Command(() => NavigationService.NavigateAsync("SmsVerificationPage"), () => IsPhoneNumberValid);
    }
}
