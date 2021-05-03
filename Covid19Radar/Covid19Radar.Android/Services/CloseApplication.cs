﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

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
using Covid19Radar.Droid.Services;
using Covid19Radar.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApplication))]
namespace Covid19Radar.Droid.Services
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            var activity = Xamarin.Essentials.Platform.CurrentActivity;
            activity.FinishAffinity();
        }
    }
}