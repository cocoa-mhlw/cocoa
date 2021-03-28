/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System;
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

namespace Covid19Radar.Droid.Services
{
    public class RangeNotifier : Java.Lang.Object, IRangeNotifier
    {
        public event EventHandler<ICollection<Beacon>> DidRangeBeaconsInRegionComplete;

        public void DidRangeBeaconsInRegion(ICollection<Beacon> beacons, AltBeaconOrg.BoundBeacon.Region region)
        {
            DidRangeBeaconsInRegionComplete?.Invoke(this, beacons);
        }
    }
}