using Covid19Radar.Model;
using Covid19Radar.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Covid19Radar.ViewModels
{
    public class DetectedBeaconPageViewmodel : ViewModelBase
    {
        public List<BeaconDataModel> Beacons { get; set; }
        public DetectedBeaconPageViewmodel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Resx.AppResources.TitleDetectedBeaconPage;
            SetData();
        }

        private void SetData()
        {
            Beacons = Xamarin.Forms.DependencyService.Resolve<IBeaconService>().GetBeaconData();
        }
    }
}
