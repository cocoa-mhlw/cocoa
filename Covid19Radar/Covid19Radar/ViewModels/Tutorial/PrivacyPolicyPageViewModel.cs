using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class PrivacyPolicyPageViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private UserDataModel userData;


        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public PrivacyPolicyPageViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = AppResources.PrivacyPolicyPageTitle;
            Url = Resources.AppResources.UrlPrivacyPolicy;

            this.userDataService = userDataService;
            userData = this.userDataService.Get();

        }

        public Command OnClickAgree => new Command(async () =>
        {
            userData.IsPolicyAccepted = true;
            await userDataService.SetAsync(userData);
            await NavigationService.NavigateAsync(nameof(TutorialPage4));
        });
    }
}
