using System.Windows.Input;
using Covid19Radar.Resx;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class InputSmsOTPPageViewModel : ViewModelBase
    {
        private string _otp;
        public string Otp
        {
            get => _otp;
            set => SetProperty(ref _otp, value);
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public ICommand ResendOtp { get; }
        public ICommand OnClickNext { get; }

        private readonly IPageDialogService _pageDialogService;

        public InputSmsOTPPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            Title = AppResources.OtpTitle;
            _pageDialogService = pageDialogService;

            ResendOtp = new Command(async () =>
            {
                await _pageDialogService.DisplayAlertAsync("Error", "This is not implemented yet", "OK");
            });
            OnClickNext = new Command(async () =>
             {
                 System.Diagnostics.Debug.WriteLine(Otp);
                 await NavigationService.NavigateAsync("UserSettingPage");
             });
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            PhoneNumber = (string)parameters["phone_number"];
        }

    }
}
