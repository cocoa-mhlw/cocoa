/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using AltBeaconOrg.BoundBeacon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Covid19Radar.Droid.Services
{
    public class RangeEventArgs : EventArgs
    {
        public Region Region { get; set; }
        public ICollection<Beacon> Beacons { get; set; }
    }
}