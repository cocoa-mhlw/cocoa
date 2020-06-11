using System.Collections.Generic;
using System.Windows.Input;
using Acr.UserDialogs;
using Covid19Radar.Model;
using Covid19Radar.Renderers;
using Covid19Radar.Resources;
using Covid19Radar.Services;
using Covid19Radar.Views;
using DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class PrivacyPolicyPage2ViewModel : ViewModelBase
    {
        private readonly UserDataService userDataService;
        private UserDataModel userData;


        private string _url;
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        public PrivacyPolicyPage2ViewModel(INavigationService navigationService, UserDataService userDataService) : base(navigationService, userDataService)
        {
            Title = AppResources.PrivacyPolicyPageTitle;
            Url = Resources.AppResources.UrlPrivacyPolicy;

            this.userDataService = userDataService;
            userData = this.userDataService.Get();
        }
    }
}
