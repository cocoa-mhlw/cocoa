using Covid19Radar.Resources;
using Prism.Navigation;

namespace Covid19Radar.ViewModels
{
    public class PrivacyPolicyPage2ViewModel : ViewModelBase
    {
        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public PrivacyPolicyPage2ViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = AppResources.PrivacyPolicyPageTitle;
            Url = AppResources.UrlPrivacyPolicy;
        }
    }
}
