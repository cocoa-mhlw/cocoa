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

namespace Covid19Radar.Droid.Service
{
	public class RangeEventArgs : EventArgs
	{
		public Region Region { get; set; }
		public ICollection<Beacon> Beacons { get; set; }
	}

	public class RangeNotifier : Java.Lang.Object, IRangeNotifier
	{
		public event EventHandler<RangeEventArgs> DidRangeBeaconsInRegionComplete;

		public void DidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
		{
			OnDidRangeBeaconsInRegion(beacons, region);
		}

		private void OnDidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
		{
			DidRangeBeaconsInRegionComplete?.Invoke(this, new RangeEventArgs { Beacons = beacons, Region = region });
		}
	}
}