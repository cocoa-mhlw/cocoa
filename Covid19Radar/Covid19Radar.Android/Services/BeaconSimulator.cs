using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltBeaconOrg.BoundBeacon;
using AltBeaconOrg.BoundBeacon.Simulator;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Covid19Radar.Common;

namespace Covid19Radar.Droid.Services
{
	public class BeaconSimulator : Java.Lang.Object, IBeaconSimulator
	{
		public bool UseSimulatedBeacons = true;

		private readonly List<Beacon> _beacons;

		public BeaconSimulator()
		{
			_beacons = new List<Beacon>();
		}

		public IList<Beacon> Beacons
		{
			get
			{
				return _beacons;
			}
		}

		public void CreateBasicSimulatedBeacons()
		{
			if (!UseSimulatedBeacons) return;

			var beacon1 = new AltBeacon.Builder().SetId1(AppConstants.AppUUID)
				.SetId2("111").SetId3("111").SetRssi(-55).SetTxPower(-55).Build();

			var beacon2 = new AltBeacon.Builder().SetId1(AppConstants.AppUUID)
				.SetId2("2").SetId3("32768").SetRssi(-55).SetTxPower(-55).Build();

			var beacon3 = new AltBeacon.Builder().SetId1(AppConstants.AppUUID)
				.SetId2("32768").SetId3("1").SetRssi(-55).SetTxPower(-55).Build();

			var beacon4 = new AltBeacon.Builder().SetId1(AppConstants.AppUUID)
				.SetId2("65535").SetId3("65535").SetRssi(-55).SetTxPower(-55).Build();

			_beacons.AddRange(new[] { beacon1, beacon2, beacon3, beacon4 });
		}
	}
}
