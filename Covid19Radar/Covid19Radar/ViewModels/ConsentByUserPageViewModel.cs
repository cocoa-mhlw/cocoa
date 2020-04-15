using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using DryIoc;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class ConsentByUserPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private UserDataService _userDataService;
        public string TextUserAgreement { get; set; }
        public string TextContainsDescriptionOfConsent1 { get; set; }
        public string TextContainsDescriptionOfConsent2 { get; set; }
        public string ButtonAgreeAndProceed { get; set; }

        public ConsentByUserPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = Resx.AppResources.TitleConsentByUserPage;

            _userDataService = App.Current.Container.Resolve<UserDataService>();
            TextUserAgreement = Resx.AppResources.TextUserAgreement;
            TextContainsDescriptionOfConsent1 = Resx.AppResources.TextContainsDescriptionOfConsent1;
            TextContainsDescriptionOfConsent2 = Resx.AppResources.TextContainsDescriptionOfConsent2;
            ButtonAgreeAndProceed = Resx.AppResources.ButtonAgreeAndProceed;
        }

        public Command OnClickNext => (new Command(async () =>
        {
            // Regist user
            if (!_userDataService.IsExistUserData())
            {
                UserDataModel userData = await _userDataService.RegistUserAsync();
            }

            await _navigationService.NavigateAsync("InitSettingPage");
        }));

    }
}
