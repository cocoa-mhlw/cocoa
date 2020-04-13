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
    public class HomePageViewModel : ViewModelBase
    {
        public ReactiveCollection<BeaconViewCell> beaconList = new ReactiveCollection<BeaconViewCell>();

        private INavigationService _navigationService;
        private readonly IBeaconService _beaconService;
        private UserDataService _userDataService;
        private UserDataModel _userData;
        public HomePageViewModel(INavigationService navigationService, UserDataService userdataservice)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _userDataService = userdataservice;
            _beaconService = Xamarin.Forms.DependencyService.Resolve<IBeaconService>();

            _userData = _userDataService.Get();
            Title = "Home Page";

            // Polling Call update or List using maybe RX
            var list = _beaconService.GetBeaconData();

            // Debug Polling update list
            Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                var list = _beaconService.GetBeaconData();
                foreach (BeaconDataModel beacon in list)
                {
                    System.Diagnostics.Debug.WriteLine(Utils.SerializeToJson(beacon));
                }
                return true;
            });

        }
        public Command OnClickUserSetting => (new Command(() =>
        {
            _navigationService.NavigateAsync("UserSettingPage");
        }));
        public Command OnClickAcknowledgments => (new Command(() =>
        {
            _navigationService.NavigateAsync("ContributersPage");
        }));

        public Command OnClickUpateInfo => (new Command(() =>
        {
            _navigationService.NavigateAsync("UpdateInfoPage");
        }));
    }
}
