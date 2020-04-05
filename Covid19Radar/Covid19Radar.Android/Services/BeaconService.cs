using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(IBeaconService))]
namespace Covid19Radar.Droid.Services
{
    public class BeaconService : IBeaconService
    {
        #region IBeaconService implementation
        public Dictionary<string,BeaconDataModel> GetBeaconData()
        {
            MainActivity activity = (MainActivity)(Android.App.Application.Context);
            return activity.GetBeaconData();
        }

        public void StartAdvertising()
        {
            MainActivity activity = (MainActivity)(Android.App.Application.Context);
            activity.StartAdvertising();
        }

        public void StartBeacon()
        {
            MainActivity activity = (MainActivity)(Android.App.Application.Context);
            activity.StartBeacon();
        }

        public void StopAdvertising()
        {
            MainActivity activity = (MainActivity)(Android.App.Application.Context);
            activity.StopAdvertising();
        }

        public void StopBeacon()
        {
            MainActivity activity = (MainActivity)(Android.App.Application.Context);
            activity.StopBeacon();
        }

        #endregion
    }
}