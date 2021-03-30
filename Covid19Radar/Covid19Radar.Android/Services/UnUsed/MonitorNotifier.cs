/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using AltBeaconOrg.BoundBeacon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Covid19Radar.Droid.Services
{
    public class MonitorNotifier : Java.Lang.Object, IMonitorNotifier
    {
        public event EventHandler<MonitorEventArgs> DetermineStateForRegionComplete;
        public event EventHandler<MonitorEventArgs> EnterRegionComplete;
        public event EventHandler<MonitorEventArgs> ExitRegionComplete;

        public void DidDetermineStateForRegion(int state, Region region)
        {
            OnDetermineStateForRegionComplete(state, region);
        }

        public void DidEnterRegion(Region region)
        {
            OnEnterRegionComplete(region);
        }

        public void DidExitRegion(Region region)
        {
            OnExitRegionComplete(region);
        }

        private void OnDetermineStateForRegionComplete(int state, Region region)
        {
            DetermineStateForRegionComplete?.Invoke(this, new MonitorEventArgs { State = state, Region = region });
        }

        private void OnEnterRegionComplete(Region region)
        {
            EnterRegionComplete?.Invoke(this, new MonitorEventArgs { Region = region });
        }

        private void OnExitRegionComplete(Region region)
        {
            ExitRegionComplete?.Invoke(this, new MonitorEventArgs { Region = region });
        }

    }

}