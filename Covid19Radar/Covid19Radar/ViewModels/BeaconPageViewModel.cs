using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Views.cell;
using Prism.Commands;
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

        public BeaconPageViewModel(INavigationService navigationService,IBeaconService beaconService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _beaconService = beaconService;
            Title = "Beacon Page";
            AppUtils.CheckPermission();

            _beaconService.InitializeService();
            _beaconService.StartBeacon();

            UserData userData = new UserData();
            userData.Uuid = AppConstants.AppUUID;
            userData.Major = "23";
            userData.Minor = "45";
            _beaconService.StartAdvertising(userData);
            /*
            try
            {
                beaconService =DependencyService.Get<IBeaconService>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            */

            //beaconService.StartBeacon();
            //beaconService.StopAdvertising();
            var beaconList = _beaconService.GetBeaconData();
            //var list = beaconService.GetBeaconData();

            var dummyCell = new BeaconViewCell();
            dummyCell.UUID.Value = "AAAAAAAAAA";

            //beaconList.Add(dummyCell);
            //beaconList.Add(dummyCell);

            // Create User
            // TODO Check Register User (UUID.Major.Minor) or New
            // POST New User and Store local properities
            /*
            if (!Application.Current.Properties.ContainsKey("UserData"))
            {
                // Access REST API and new id case
                UserData userData = new UserData();
                userData.Uuid = AppConstants.AppUUID;
                userData.Major = "23";
                userData.Minor = "45";
                Application.Current.Properties["UserData"] = userData;
            }
            */
        }

    }
}
