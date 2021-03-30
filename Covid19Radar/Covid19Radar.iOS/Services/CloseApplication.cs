/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System.Threading;
using Covid19Radar.iOS.Services;
using Covid19Radar.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(CloseApplication))]
namespace Covid19Radar.iOS.Services
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            Thread.CurrentThread.Abort();
        }
    }
}