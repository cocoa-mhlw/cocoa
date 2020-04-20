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
    public class DetectedBeaconPageViewmodel : ViewModelBase
    {
        private INavigationService _navigationService;
        public DetectedBeaconPageViewmodel(INavigationService navigationService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Detected Beacon List";
        }

        public Command OnClickPrev => (new Command(() =>
        {
            _navigationService.NavigateAsync("HomePage");
        }));


    }
}
