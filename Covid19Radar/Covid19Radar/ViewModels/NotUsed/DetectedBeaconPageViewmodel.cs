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
        public List<BeaconDataModel> Beacons { get; set; }
        private ObservableCollection<BeaconDataModel> _beaconDataList;


        public ObservableCollection<BeaconDataModel> BeaconDataList
        {
            get => _beaconDataList;
            set => SetProperty(ref _beaconDataList, value);
        }


        public DetectedBeaconPageViewmodel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Resources.AppResources.TitleDetectedBeaconPage;
            //_beaconService = beaconService;
            //SetData();

            BeaconDataList = new ObservableCollection<BeaconDataModel>();
        }
        
        
        private void SetData()
        {
            //Beacons = Xamarin.Forms.DependencyService.Resolve<IBeaconService>().GetBeaconData();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var navMode = parameters.GetNavigationMode();

            if (navMode == NavigationMode.New)
            {
                /*
                var beaconList = _beaconService.GetBeaconData();
                foreach (var beacon in beaconList)
                {
                    if (beacon.LastDetectTime.Kind != DateTimeKind.Local)
                    {
                        beacon.LastDetectTime = TimeZoneInfo.ConvertTimeFromUtc(beacon.LastDetectTime, TimeZoneInfo.Local);
                    }
                }

                BeaconDataList = new ObservableCollection<BeaconDataModel>(beaconList);
                */
            }
        }
    }
}
