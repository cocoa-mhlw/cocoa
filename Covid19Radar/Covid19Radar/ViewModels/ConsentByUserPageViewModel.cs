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
        public ConsentByUserPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Consent by user page";
        }

        public Command OnClickNext => (new Command(() =>
        {
                       _navigationService.NavigateAsync("DemoPage");
            //            _navigationService.NavigateAsync("BeaconPage");

        }));

    }
}
