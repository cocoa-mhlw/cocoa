using Covid19Radar.Model;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class PrivacyPolicyPage2ViewModel : ViewModelBase
    {
        private readonly IUserDataService userDataService;
        private UserDataModel userData;


        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public PrivacyPolicyPage2ViewModel(INavigationService navigationService, IUserDataService userDataService) : base(navigationService)
        {
            Title = AppResources.PrivacyPolicyPageTitle;
            Url = Resources.AppResources.UrlPrivacyPolicy;

            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }
    }
}
