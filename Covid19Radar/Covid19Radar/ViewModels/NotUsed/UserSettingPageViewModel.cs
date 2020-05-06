using System.Linq;
using Covid19Radar.Services;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class UserSettingPageViewModel : ViewModelBase
    {
        private readonly OTPService _otpService;
        private readonly UserDataService _userDataService;
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

        public UserSettingPageViewModel() : base()
        {
            Title = Resources.AppResources.TitleUserSettings;
            _otpService = App.Current.Container.Resolve<OTPService>();
            _userDataService = App.Current.Container.Resolve<UserDataService>();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            PhoneNumber = string.Empty;
        }

        public Command OnClickNext => new Command(async () =>
        {
            var user = _userDataService.Get();
            await _otpService.SendOTPAsync(user, PhoneNumberWithoutMask());
            await NavigationService.NavigateAsync($"InputSmsOTPPage?phone_number={PhoneNumber}");
        }, () => IsPhoneNumberValid);

        private string PhoneNumberWithoutMask()
        {
            return new string(("+81").Concat(PhoneNumber.Where(char.IsDigit).Skip(1)).ToArray());
        }
    }
}
