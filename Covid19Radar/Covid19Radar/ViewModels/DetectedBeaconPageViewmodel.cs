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
                // var beaconList = _beaconService.GetBeaconData();

                var beaconList = new List<BeaconDataModel>()
                {
                    new BeaconDataModel()
                    {
                        Id = "d7cd5957-53d3-4dbb-a169-a1283f3bce30",
                        Major = "0",
                        Minor = "129",
                        Distance = 0.89,
                        ElaspedTime = TimeSpan.FromMinutes(10),
                        LastDetectTime = DateTime.Now,
                        Count = 1321
                    },
                    new BeaconDataModel()
                    {
                        Id = "1815da38-97bd-4d8a-9f30-0c18b862f696",
                        Major = "0",
                        Minor = "232",
                        Distance = 10.23,
                        ElaspedTime = TimeSpan.FromMinutes(2),
                        LastDetectTime = DateTime.Now.AddDays(-2),
                        Count = 1321
                    },
                    new BeaconDataModel()
                    {
                        Id = "c7506b33-d722-4923-9756-d9c391321ed4",
                        Major = "0",
                        Minor = "312",
                        Distance = 10.23,
                        ElaspedTime = TimeSpan.FromMinutes(2),
                        LastDetectTime = DateTime.Now.AddDays(-10),
                        Count = 1321
                    },
                    new BeaconDataModel()
                    {
                        Id = "427acfb3-d756-4d83-8d9d-533ccf7a12e8",
                        Major = "0",
                        Minor = "232",
                        Distance = 20.23,
                        ElaspedTime = TimeSpan.FromMinutes(2),
                        LastDetectTime = DateTime.Now.AddDays(-40),
                        Count = 5623
                    }
                };

                BeaconDataList = new ObservableCollection<BeaconDataModel>(beaconList);
            }
        }
    }
}
