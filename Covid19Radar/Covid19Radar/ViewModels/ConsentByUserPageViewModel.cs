using System.Collections.Generic;
using System.Windows.Input;
using Covid19Radar.Model;
using Covid19Radar.Resx;
using Covid19Radar.Services;
using DryIoc;
using Prism.Ioc;
using Prism.Navigation;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ConsentByUserPageViewModel : ViewModelBase
    {
        private UserDataService _userDataService;

        public ICommand OnClickNext { get; }

        /*
        private List<TermsOfServiceModel> _termsOfServices;
        public List<TermsOfServiceModel> TermsOfServices
        {
            get => _termsOfServices;
            set => SetProperty(ref _termsOfServices, value);
        }
        */
        private string _url;

        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public ConsentByUserPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = AppResources.TitleConsentByUserPage;
            Url = Resx.AppResources.UrlPrivacyPolicy;

            _userDataService = App.Current.Container.Resolve<UserDataService>();
            OnClickNext = new Command(async () =>
             {
                 // Regist user
                 if (!_userDataService.IsExistUserData)
                 {
                     IsBusy = true;
                     UserDataModel userData = await _userDataService.RegistUserAsync();
                     IsBusy = false;
                 }

                 await NavigationService.NavigateAsync("InitSettingPage");
             });

        }
/*
        private void SetData()
        {
            TermsOfServices = new List<TermsOfServiceModel>
            {
                new TermsOfServiceModel
                {
                    Title=AppResources.TermsOfServiceTitle1,
                    Description=AppResources.TermsOfServiceDescription1
                },
                new TermsOfServiceModel
                {
                    Title=AppResources.TermsOfServiceTitle2,
                    Description=AppResources.TermsOfServiceDescription2
                },
                new TermsOfServiceModel
                {
                    Title=AppResources.TermsOfServiceTitle3,
                    Description=AppResources.TermsOfServiceDescription3
                },
                new TermsOfServiceModel
                {
                    Title=AppResources.TermsOfServiceTitle4,
                    Description=AppResources.TermsOfServiceDescription4
                },
            };
        }
*/
    }
}
