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
        private readonly IUserDataService _userDataService;

        public SplashPageViewModel(INavigationService navigationService, ILoggerService loggerService, ITermsUpdateService termsUpdateService, IUserDataService userDataService) : base(navigationService)
        {
            _termsUpdateService = termsUpdateService;
            _loggerService = loggerService;
            _userDataService = userDataService;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            _loggerService.StartMethod();

            base.OnNavigatedTo(parameters);

            await _userDataService.Migrate();

            if (_termsUpdateService.IsAllAgreed())
            {
                _loggerService.Info("User data exists");

                var termsUpdateInfo = await _termsUpdateService.GetTermsUpdateInfo();

                if (_termsUpdateService.IsReAgree(TermsType.TermsOfService, termsUpdateInfo))
                {
                    var param = new NavigationParameters
                {
                    { "updateInfo", termsUpdateInfo }
                };
                    _loggerService.Info($"Transition to ReAgreeTermsOfServicePage");
                    _ = await NavigationService.NavigateAsync(nameof(ReAgreeTermsOfServicePage), param);
                }
                else if (_termsUpdateService.IsReAgree(TermsType.PrivacyPolicy, termsUpdateInfo))
                {
                    var param = new NavigationParameters
                {
                    { "updatePrivacyPolicyInfo", termsUpdateInfo.PrivacyPolicy }
                };
                    _loggerService.Info($"Transition to ReAgreePrivacyPolicyPage");
                    _ = await NavigationService.NavigateAsync(nameof(ReAgreePrivacyPolicyPage), param);
                }
                else
                {
                    _loggerService.Info($"Transition to HomePage");
                    _ = await NavigationService.NavigateAsync("/" + nameof(MenuPage) + "/" + nameof(NavigationPage) + "/" + nameof(HomePage));
                }
            }
            else
            {
                _loggerService.Info("No user data exists");
                _loggerService.Info($"Transition to TutorialPage1");
                _ = await NavigationService.NavigateAsync("/" + nameof(TutorialPage1));
            }

            _loggerService.EndMethod();
        }
    }
}
