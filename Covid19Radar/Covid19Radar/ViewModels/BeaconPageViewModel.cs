using Covid19Radar.Model;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using System.Diagnostics;
using Prism.Ioc;
using Prism.DryIoc;
using ImTools;
using Covid19Radar.Common;

namespace Covid19Radar.ViewModels
{
    public class BeaconPageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private IDependencyService _dependencyService;

        public BeaconPageViewModel(INavigationService navigationService, IDependencyService dependencyService)
            : base(navigationService,dependencyService)

        {
            _navigationService = navigationService;
            _dependencyService = dependencyService;
            var beaconservice = dependencyService.Get<IIBeaconService>();
            iBeacon beacon = new iBeacon(Guid.NewGuid(), iBeacon.DEFAULT_MAJOR, iBeacon.DEFAULT_MINOR,iBeacon.DEFAULT_TXPOWER);
            beaconservice.StartTransmission(beacon);
            Title = "Beacon Test";

        }
    }
}
