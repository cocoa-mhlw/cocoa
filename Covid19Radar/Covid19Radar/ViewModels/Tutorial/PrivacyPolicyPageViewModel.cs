using System;
using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class PrivacyPolicyPageViewModel : ViewModelBase
    {
        private readonly IUserDataService userDataService;
        private readonly ILoggerService loggerService;
        private readonly ITermsUpdateService termsUpdateService;

        private UserDataModel userData;


        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public PrivacyPolicyPageViewModel(INavigationService navigationService, IUserDataService userDataService, ILoggerService loggerService, ITermsUpdateService termsUpdateService) : base(navigationService)
        {
            Title = AppResources.PrivacyPolicyPageTitle;
            Url = Resources.AppResources.UrlPrivacyPolicy;

            this.userDataService = userDataService;
            this.loggerService = loggerService;
            userData = this.userDataService.Get();
            this.termsUpdateService = termsUpdateService;
        }

        public Command OnClickAgree => new Command(async () =>
        {
            loggerService.StartMethod();

            userData.IsPolicyAccepted = true;
            await userDataService.SetAsync(userData);
            loggerService.Info($"IsPolicyAccepted set to {userData.IsPolicyAccepted}");
            await termsUpdateService.SaveLastUpdateDateAsync(TermsType.PrivacyPolicy, DateTime.Now);
            await NavigationService.NavigateAsync(nameof(TutorialPage4));

            loggerService.EndMethod();
        });
    }
}
