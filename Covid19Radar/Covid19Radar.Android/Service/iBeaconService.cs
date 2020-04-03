using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltBeaconOrg.BoundBeacon;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Covid19Radar.Droid.Service;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Xamarin.Forms;
using System.ComponentModel;
using System.Threading.Tasks;

[assembly: Dependency(typeof(iBeaconService))]

namespace Covid19Radar.Droid.Service
{
    public class iBeaconService : IIBeaconService
    {
        private readonly MonitorNotifier _monitorNotifier;
        private readonly RangeNotifier _rangeNotifier;
        private BeaconManager _beaconManager;
        AltBeaconOrg.BoundBeacon.Region _tagRegion;
        AltBeaconOrg.BoundBeacon.Region _emptyRegion;
        private Xamarin.Forms.ListView _list;
        private readonly List<Beacon> _data;
        private BeaconTransmitter _beaconTransmitter;

        public iBeaconService()
        {
            _monitorNotifier = new MonitorNotifier();
            _rangeNotifier = new RangeNotifier();
            _data = new List<Beacon>();
        }

        public event EventHandler<ListChangedEventArgs> ListChanged;
        public event EventHandler DataClearing;

        /// <summary>
        /// Check Transmittion Supported
        /// </summary>
        /// <returns><c>true</c>, Can Transmission, <c>false</c> Can not Transmittion</returns>
        public bool TransmissionSupported()
        {
            Console.WriteLine("TransmissionSupported");
            int checkResult = BeaconTransmitter.CheckTransmissionSupported(Android.App.Application.Context);
            return (checkResult == BeaconTransmitter.Supported);
        }

        /// <summary>
        /// iBeacon Start Transmission
        /// </summary>
        /// <param name="ibeacon">iBeacon Model</param>
        public void StartTransmission(iBeacon ibeacon)
        {
            this.StartTransmission(ibeacon.Uuid, ibeacon.Major, ibeacon.Minor, ibeacon.TxPower);
        }

        /// <summary>
        /// iBeacon Start Transmission
        /// </summary>
        /// <param name="uuid">UUID</param>
        /// <param name="major">Major</param>
        /// <param name="minor">Minor</param>
        public void StartTransmission(Guid uuid, ushort major, ushort minor, sbyte txPower)
        {
            Console.WriteLine("StartTransmission");
            Beacon beacon = new Beacon.Builder()
                                .SetId1(uuid.ToString())
                                .SetId2(major.ToString())
                                .SetId3(minor.ToString())
                                .SetTxPower(txPower)
                                .SetManufacturer(Const.COMPANY_CODE_APPLE)
                                .Build();

            BeaconParser beaconParser = new BeaconParser().SetBeaconLayout(Const.IBEACON_FORMAT);

            _beaconTransmitter = new BeaconTransmitter(Android.App.Application.Context, beaconParser);
            _beaconTransmitter.StartAdvertising(beacon);
        }

        /// <summary>
        /// iBeacon Stop Transmission
        /// </summary>
        public void StopTransmission()
        {
            _beaconTransmitter.StopAdvertising();
        }

        public BeaconManager GetBeaconManager
        {
            get
            {
                if (_beaconManager == null)
                {
                    _beaconManager = InitializeBeaconManager();
                }
                return _beaconManager;
            }
        }

        public void InitializeService()
        {
            _beaconManager = InitializeBeaconManager();
        }

        private BeaconManager InitializeBeaconManager()
        {
            Console.WriteLine("InitializeBeaconManager");
            // Enable the BeaconManager 
            BeaconManager beaconManager = BeaconManager.GetInstanceForApplication(Android.App.Application.Context);
            BeaconParser beaconParser = new BeaconParser().SetBeaconLayout(Const.IBEACON_FORMAT);
            beaconManager.BeaconParsers.Add(beaconParser);

            /*
            _monitorNotifier.EnterRegionComplete += EnteredRegion;
            _monitorNotifier.ExitRegionComplete += ExitedRegion;
            _monitorNotifier.DetermineStateForRegionComplete += DeterminedStateForRegionComplete;
			*/

            _rangeNotifier.DidRangeBeaconsInRegionComplete += DidRangeBeaconsInRegionComplete;

            /*
            _tagRegion = new AltBeaconOrg.BoundBeacon.Region("myUniqueBeaconId", Identifier.Parse("E4C8A4FC-F68B-470D-959F-29382AF72CE7"), null, null);
            _tagRegion = new AltBeaconOrg.BoundBeacon.Region("myUniqueBeaconId", Identifier.Parse("B9407F30-F5F8-466E-AFF9-25556B57FE6D"), null, null);
            _emptyRegion = new AltBeaconOrg.BoundBeacon.Region("myEmptyBeaconId", null, null, null);
			*/

            beaconManager.SetBackgroundMode(false);
            beaconManager.Bind((IBeaconConsumer)Android.App.Application.Context);

            return beaconManager;
        }

        public void StartMonitoring()
        {
			Console.WriteLine("StartMonitoring");
			GetBeaconManager.SetForegroundBetweenScanPeriod(5000); // 5000 milliseconds

            GetBeaconManager.AddMonitorNotifier(_monitorNotifier);
            _beaconManager.StartMonitoringBeaconsInRegion(_tagRegion);
            _beaconManager.StartMonitoringBeaconsInRegion(_emptyRegion);
        }

        public void StartRanging()
        {
            Console.WriteLine("StartRanging");
            GetBeaconManager.SetForegroundBetweenScanPeriod(5000); // 5000 milliseconds

            GetBeaconManager.AddRangeNotifier(_rangeNotifier);
//            _beaconManager.StartRangingBeaconsInRegion(_tagRegion);
            _beaconManager.StartRangingBeaconsInRegion(_emptyRegion);
        }

private void DeterminedStateForRegionComplete(object sender, MonitorEventArgs e)
{
	Console.WriteLine("DeterminedStateForRegionComplete");
}


private void ExitedRegion(object sender, MonitorEventArgs e)
{
	Console.WriteLine("ExitedRegion");
}

private void EnteredRegion(object sender, MonitorEventArgs e)
{
	Console.WriteLine("EnteredRegion");
}
        async void DidRangeBeaconsInRegionComplete(object sender, RangeEventArgs e)
        {
            Console.WriteLine("DidRangeBeaconsInRegionComplete");
            ClearData();

            var allBeacons = new List<Beacon>();
            if (e.Beacons.Count > 0)
            {
                foreach (var b in e.Beacons)
                {
                    allBeacons.Add(b);
                }

                var orderedBeacons = allBeacons.OrderBy(b => b.Distance).ToList();
                await UpdateData(orderedBeacons);
            }
            else
            {
                // unknown
                ClearData();
            }
        }

        private async Task UpdateData(List<Beacon> beacons)
        {
            Console.WriteLine("UpdateData");
            await Task.Run(() =>
            {
                var newBeacons = new List<Beacon>();
                foreach (var beacon in beacons)
                {
                    if (_data.All(b => b.Id1.ToString() == beacon.Id1.ToString()))
                    {
                        newBeacons.Add(beacon);
                    }
                }

                ((Activity)Android.App.Application.Context).RunOnUiThread(() =>
                {
                    foreach (var beacon in newBeacons)
                    {
                        _data.Add(beacon);
                    }

                    if (newBeacons.Count > 0)
                    {
                        _data.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                        UpdateList();
                    }
                });
            });
        }

        private void ClearData()
        {
            Console.WriteLine("ClearData");
            ((Activity)Android.App.Application.Context).RunOnUiThread(() =>
            {
                _data.Clear();
                OnDataClearing();
            });
        }

        private void OnDataClearing()
        {
            Console.WriteLine("OnDataClearing");
            var handler = DataClearing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void UpdateList()
        {
            Console.WriteLine("UpdateList");
            ((Activity)Android.App.Application.Context).RunOnUiThread(() =>
            {
                OnListChanged();
            });
        }

        private void OnListChanged()
        {
            Console.WriteLine("OnListChanged");
			var handler = ListChanged;
			if (handler != null)
			{
				var data = new List<SharedBeacon>();
				_data.ForEach(b =>
				{
					data.Add(new SharedBeacon { Id = b.Id1.ToString(), Distance = string.Format("{0:N2}m", b.Distance) });
				});
				//handler(this, new ListChangedEventArgs(data));
			}
        }
    }
}
