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
        private ObservableCollection<BeaconDataModel> _beaconDataList;


        public ObservableCollection<BeaconDataModel> BeaconDataList
        {
            get => _beaconDataList;
            set => SetProperty(ref _beaconDataList, value);
        }


        public DetectedBeaconPageViewmodel(INavigationService navigationService, IBeaconService beaconService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _beaconService = beaconService;

            Title = "Detected Beacon List";
            BeaconDataList = new ObservableCollection<BeaconDataModel>();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            var navMode = parameters.GetNavigationMode();

            if (navMode == NavigationMode.New)
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
