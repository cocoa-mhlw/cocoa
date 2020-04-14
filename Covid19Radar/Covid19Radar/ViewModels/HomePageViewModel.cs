using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Commands;
using Prism.DryIoc;
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

namespace Covid19Radar.ViewModels
{
    public class HomePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private INavigationService _navigationService;
        private readonly IBeaconService _beaconService;
        private UserDataService _userDataService;
        private UserDataModel _userData;

        private ObservableCollection<BeaconDataModel> beacons;
        public ObservableCollection<BeaconDataModel> Beacons
        {
            get { return beacons; }
            set
            {

                beacons = value;
            }
        }
        public HomePageViewModel(INavigationService navigationService, UserDataService userdataservice)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _userDataService = userdataservice;
            _beaconService = Xamarin.Forms.DependencyService.Resolve<IBeaconService>();

            _userData = _userDataService.Get();
            Title = "Home Page";

            var list = _beaconService.GetBeaconData();
            Beacons = new ObservableCollection<BeaconDataModel>(list);
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

        public Command OnClickUpdateList => (new Command(() =>
        {
        }));

        
    }
}
