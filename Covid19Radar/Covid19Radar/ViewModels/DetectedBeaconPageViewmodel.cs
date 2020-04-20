using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DetectedBeaconPageViewmodel : ViewModelBase
    { 
        private INavigationService _navigationService;
        private IBeaconService _beaconService;


        public ObservableCollection<BeaconDataModel> BeaconDataList { get; set; }


        public DetectedBeaconPageViewmodel(INavigationService navigationService, IBeaconService beaconService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _beaconService = beaconService;

            Title = "Detected Beacon List";
            BeaconDataList = new ObservableCollection<BeaconDataModel>();
        }

        public override async void OnNavigatedFrom(INavigationParameters parameters)
        {
            base.OnNavigatedFrom(parameters);

            if (parameters.GetNavigationMode() == NavigationMode.Forward)
            {
                var beaconList = _beaconService.GetBeaconData();
                BeaconDataList = new ObservableCollection<BeaconDataModel>(beaconList);
            }
        }

        public Command OnClickPrev => (new Command(async () =>
        {
            await _navigationService.GoBackAsync();
        }));
    }
}
