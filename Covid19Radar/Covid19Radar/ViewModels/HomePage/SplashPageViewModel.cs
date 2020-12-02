using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class SplashPageViewModel : ViewModelBase
    {
        private readonly ITermsUpdateService _termsUpdateService;
        private readonly ILoggerService _loggerService;

        public SplashPageViewModel(INavigationService navigationService, ILoggerService loggerService, ITermsUpdateService termsUpdateService) : base(navigationService)
        {
            _termsUpdateService = termsUpdateService;
            _loggerService = loggerService;
        }

        public override async void Initialize(INavigationParameters parameters)
        {
            _loggerService.StartMethod();

            base.Initialize(parameters);
            var termsUpdateInfo = await _termsUpdateService.GetTermsUpdateInfo();

            if (_termsUpdateService.IsReAgree(TermsType.TermsOfService, termsUpdateInfo))
            {
                var param = new NavigationParameters
                {
                    { "updateInfo", termsUpdateInfo }
                };
                _ = await NavigationService.NavigateAsync(nameof(ReAgreeTermsOfServicePage), param);
            }
            else if (_termsUpdateService.IsReAgree(TermsType.PrivacyPolicy, termsUpdateInfo))
            {
                var param = new NavigationParameters
                {
                    { "updatePrivacyPolicyInfo", termsUpdateInfo.PrivacyPolicy }
                };
                _ = await NavigationService.NavigateAsync(nameof(ReAgreePrivacyPolicyPage), param);
            }
            else
            {
                _ = await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
            }

            _loggerService.EndMethod();
        }
    }
}
