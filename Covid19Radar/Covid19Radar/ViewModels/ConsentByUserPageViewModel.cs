using Prism.Commands;
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
        public string TextUserAgreement { get; set; }
        public string TextContainsDescriptionOfConsent1 { get; set; }
        public string TextContainsDescriptionOfConsent2 { get; set; }
        public string ButtonAgreeAndProceed { get; set; }

        public ConsentByUserPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = Resx.AppResources.TitleConsentByUserPage;
            TextUserAgreement = Resx.AppResources.TextUserAgreement;
            TextContainsDescriptionOfConsent1 = Resx.AppResources.TextContainsDescriptionOfConsent1;
            TextContainsDescriptionOfConsent2 = Resx.AppResources.TextContainsDescriptionOfConsent2;
            ButtonAgreeAndProceed = Resx.AppResources.ButtonAgreeAndProceed;
        }

        public Command OnClickNext => (new Command(() =>
        {
                       _navigationService.NavigateAsync("DemoPage");
            //            _navigationService.NavigateAsync("BeaconPage");

        }));

    }
}
