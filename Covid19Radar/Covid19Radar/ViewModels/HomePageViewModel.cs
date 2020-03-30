using Acr.UserDialogs.Forms;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Shiny.Beacons;
using Shiny.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Windows.Input;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;

namespace Covid19Radar.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private INavigationService _navigationService;
        private IBeaconManager _beaconManager;
        private IUserDialogs _dialogs;
        private IConnectivity _connectivityService;

        public ICommand Start { get; }
        public ICommand Stop { get; }
        public IList<Beacon> Beacons { get; private set; }
        IDisposable scanSub;

        public HomePageViewModel(INavigationService navigationService, IConnectivity connectivityService, IUserDialogs dialogs, IBeaconManager beaconManager)
            : base(navigationService,connectivityService,dialogs,beaconManager)
        {
            _navigationService = navigationService;
            _beaconManager = beaconManager;

            Title = "ホーム";
        }
    }
    public class BeaconViewModel : ReactiveObject
    {
        public BeaconViewModel(Beacon beacon)
        {
            this.Beacon = beacon;
            this.Proximity = beacon.Proximity;
        }


        public Beacon Beacon { get; }
        public ushort Major => this.Beacon.Major;
        public ushort Minor => this.Beacon.Minor;
        public string Identifier => $"Major: {this.Major} - Minor: {this.Minor}";
        [Reactive] public Proximity Proximity { get; set; }
    }
}
