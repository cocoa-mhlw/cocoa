using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreLocation;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Foundation;
using UIKit;

namespace Covid19Radar.iOS.Services
{
    public class BeaconService : IBeaconService
    {
        public Dictionary<string, BeaconDataModel> GetBeaconData()
        {
            throw new NotImplementedException();
        }

        public void StartAdvertising()
        {
            throw new NotImplementedException();
        }

        public void StartBeacon()
        {
            AppDelegate.Instance.StartBeacon();
        }

        public void StopAdvertising()
        {
            throw new NotImplementedException();
        }

        public void StopBeacon()
        {
            AppDelegate.Instance.StopBeacon();
        }
    }
}