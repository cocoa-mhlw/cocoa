using System.Windows.Input;
using Covid19Radar.Resources;
using Prism.Commands;
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
            set
            {
                SetProperty(ref _otp, value);
                OnClickNext.RaiseCanExecuteChanged();
            }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public ICommand ResendOtp { get; }
        public DelegateCommand OnClickNext { get; }

        private readonly IPageDialogService _pageDialogService;

        public InputSmsOTPPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
            : base(navigationService)
        {
            Title = AppResources.TitleOtp;
            _pageDialogService = pageDialogService;

            ResendOtp = new Command(async () =>
            {
                await _pageDialogService.DisplayAlertAsync("Error", "This is not implemented yet", "OK");
            });
            OnClickNext = new DelegateCommand(async () =>
             {
                 await NavigationService.NavigateAsync("StatusUpdateCompletePage");
             }, CanVerifyOtp);
        }

        private bool CanVerifyOtp()
        {
            if (string.IsNullOrEmpty(Otp))
            {
                return false;
            }
            return Otp.Length == 6;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.TryGetValue<string>("phone_number", out var phoneNumber))
            {
                PhoneNumber = phoneNumber;
            }
        }

    }
}
