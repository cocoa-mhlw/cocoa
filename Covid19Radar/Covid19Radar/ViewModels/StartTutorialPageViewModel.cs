using Covid19Radar.Model;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class StartTutorialPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        public string TextStopTheSpreadOfCOVID19 { get; set; }
        public string TextWeWillHelpAgenciesContact { get; set; }
        public string TextToProtectThoseAroundYou { get; set; }
        public string ButtonIWantToHelp { get; set; }

        public StartTutorialPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            TextStopTheSpreadOfCOVID19 = Resx.AppResources.TextStopTheSpreadOfCOVID19;
            TextWeWillHelpAgenciesContact = Resx.AppResources.TextWeWillHelpAgenciesContact;
            TextToProtectThoseAroundYou = Resx.AppResources.TextToProtectThoseAroundYou;
            ButtonIWantToHelp = Resx.AppResources.ButtonIWantToHelp;
        }

        public Command OnClickNext => (new Command(() =>
        {
            _navigationService.NavigateAsync("DescriptionPage");
        }));

    }
}
