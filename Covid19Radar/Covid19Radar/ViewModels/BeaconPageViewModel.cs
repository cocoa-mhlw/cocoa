using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Views.cell;
using Prism.Commands;
using Prism.DryIoc;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class BeaconPageViewModel : ViewModelBase
    {
        public ReactiveCollection<BeaconViewCell> beaconList = new ReactiveCollection<BeaconViewCell>();

        private INavigationService _navigationService;
        private readonly IBeaconService _beaconService;
        private UserDataService _userDataService;
        private UserDataModel _userData;
        public BeaconPageViewModel(INavigationService navigationService, UserDataService userdataservice)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _userDataService = userdataservice;
            _beaconService = Xamarin.Forms.DependencyService.Get<IBeaconService>();

            _userData = _userDataService.Get();
            Title = "Beacon Page";

            // Polling Call update List
            var list = _beaconService.GetBeaconData();
        }

    }
}
