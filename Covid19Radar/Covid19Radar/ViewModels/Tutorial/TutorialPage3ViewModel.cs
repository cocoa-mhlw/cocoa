using System;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class TutorialPage3ViewModel : ViewModelBase
    {
        private readonly ILoggerService loggerService;
        private readonly UserDataService userDataService;
        private UserDataModel userData;
        private ITermsUpdateService termsUpdateService;

        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public TutorialPage3ViewModel(INavigationService navigationService, ILoggerService loggerService, UserDataService userDataService, ITermsUpdateService termsUpdateService) : base(navigationService, userDataService)
        {
            this.loggerService = loggerService;
            this.userDataService = userDataService;
            userData = this.userDataService.Get();
            this.termsUpdateService = termsUpdateService;
        }
        public Command OnClickAgree => new Command(async () =>
        {
            loggerService.StartMethod();

            UserDialogs.Instance.ShowLoading(Resources.AppResources.LoadingTextRegistering);
            if (!userDataService.IsExistUserData)
            {
                loggerService.Info("No user data exists");
                userData = await userDataService.RegisterUserAsync();
                if (userData == null)
                {
                    loggerService.Info("userData is null");
                    UserDialogs.Instance.HideLoading();
                    await UserDialogs.Instance.AlertAsync(Resources.AppResources.DialogNetworkConnectionError, Resources.AppResources.DialogNetworkConnectionErrorTitle, Resources.AppResources.ButtonOk);
                    loggerService.EndMethod();
                    return;
                }
                loggerService.Info("userData is not null");
            }
            else
            {
                loggerService.Info("User data exists");
            }

            userData.IsOptined = true;
            await userDataService.SetAsync(userData);
            loggerService.Info($"IsOptined set to {userData.IsOptined}");
            await termsUpdateService.SaveLastUpdateDateAsync(TermsType.TermsOfService, DateTime.Now);
            UserDialogs.Instance.HideLoading();
            await NavigationService.NavigateAsync(nameof(PrivacyPolicyPage));
            loggerService.EndMethod();
        });
    }
}