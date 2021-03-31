/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Xamarin.Essentials;

namespace Covid19Radar.Services
{
    public class EssentialsService : IEssentialsService
    {
        // DeviceInfo
        public string Platform => DeviceInfo.Platform.ToString();
        public string PlatformVersion => DeviceInfo.VersionString;
        public string Model => DeviceInfo.Model;
        public string DeviceType => DeviceInfo.DeviceType.ToString();

        // AppInfo
        public string AppVersion => AppInfo.VersionString;
        public string BuildNumber => AppInfo.BuildString;
    }
}
