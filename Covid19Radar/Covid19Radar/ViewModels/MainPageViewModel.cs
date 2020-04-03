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
    public class MainPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private IDependencyService _dependencyService;
        private string _message;

        public DelegateCommand ShowDialogCommand { get; set; }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public MainPageViewModel(INavigationService navigationService, IDependencyService dependencyService)
    : base(navigationService, dependencyService)

        {
            _navigationService = navigationService;
            _dependencyService = dependencyService;
            var beaconservice = dependencyService.Get<IIBeaconService>();
            iBeacon beacon = new iBeacon(Guid.NewGuid(), iBeacon.DEFAULT_MAJOR, iBeacon.DEFAULT_MINOR, iBeacon.DEFAULT_TXPOWER);
            beaconservice.StartTransmission(beacon);
            beaconservice.InitializeService();
            beaconservice.StartRanging();
            Title = "Start";

        }

        public Command OnClickNext => (new Command(() =>
        {
            _navigationService.NavigateAsync("DescriptionPage");
        }));

    }
}
