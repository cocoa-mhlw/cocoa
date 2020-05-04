using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltBeaconOrg.BoundBeacon;
using AltBeaconOrg.BoundBeacon.Startup;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Covid19Radar.Common;
using Covid19Radar.Droid.Services;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Java.Util;
using Plugin.CurrentActivity;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(BeaconService))]
namespace Covid19Radar.Droid.Services
{
    public class BeaconService : IBeaconService, IDisposable
    {
        private HttpDataService _httpDataService;
        private MinutesTimer _uploadTimer;
        private UserDataModel _userData;
        private SQLiteConnection _connection;

        public BeaconService()
        {
            _httpDataService = DependencyService.Resolve<HttpDataService>();
            _connection = DependencyService.Get<ISQLiteConnectionProvider>().GetConnection();
            _connection.CreateTable<BeaconDataModel>();
        }

        protected MainActivity MainActivity
        {
            get { return (MainActivity)CrossCurrentActivity.Current.Activity; }
        }

        public List<BeaconDataModel> GetBeaconData()
        {
            return MainActivity.GetBeaconData();
        }

        public async void StartRagingBeacons(UserDataModel userData)
        {
            _userData = userData;
            BeaconUuidModel _beaconUuid = await _httpDataService.GetBeaconUuidAsync();

            MainActivity.StartRagingBeacons(_beaconUuid);
            /*
            _uploadTimer = new MinutesTimer(_userData.GetJumpHashTimeDifference());
            _uploadTimer.Start();
            _uploadTimer.TimeOutEvent += TimerUploadAsync;
            */
        }

        public void StopRagingBeacons()
        {
            MainActivity.StopRagingBeacons();
        }

        public async void StartAdvertisingBeacons(UserDataModel userData)
        {
            BeaconUuidModel _beaconUuid = await _httpDataService.GetBeaconUuidAsync();
            MainActivity.StartAdvertisingBeacons(userData, _beaconUuid);
        }

        public void StopAdvertisingBeacons()
        {
            MainActivity.StopAdvertisingBeacons();
        }

        private async void TimerUploadAsync(EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString());

            List<BeaconDataModel> beacons = _connection.Table<BeaconDataModel>().ToList();
            foreach (var beacon in beacons)
            {
                if (beacon.IsSentToServer) continue;
                if (!await _httpDataService.PostBeaconDataAsync(_userData, beacon)) continue;

                var key = beacon.Id;
                lock (MainActivity.dataLock)
                {
                    var b = _connection.Table<BeaconDataModel>().SingleOrDefault(x => x.Id == key);
                    b.IsSentToServer = true;
                    _connection.Update(b);
                }
            }
        }

        public void Dispose()
        {
            _uploadTimer.Stop();
        }
    }
}

