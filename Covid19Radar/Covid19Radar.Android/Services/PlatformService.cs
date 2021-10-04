/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Android.App;
using Xamarin.Essentials;

namespace Covid19Radar.Droid.Services
{
    public class PlatformService : IPlatformService
    {
        public Activity CurrentActivity => Platform.CurrentActivity;
        public string GooglePlayServiceIdentifier => "com.google.android.gms";
    }
}
