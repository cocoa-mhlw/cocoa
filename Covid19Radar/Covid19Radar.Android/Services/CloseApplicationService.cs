﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Services;

namespace Covid19Radar.Droid.Services
{
    public class CloseApplicationService : ICloseApplicationService
    {
        public void CloseApplication()
        {
            var activity = Xamarin.Essentials.Platform.CurrentActivity;
            activity.FinishAffinity();
        }
    }
}