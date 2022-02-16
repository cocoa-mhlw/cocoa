/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
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
        public Version DeviceVersion => DeviceInfo.Version;
        public bool IsAndroid => DeviceInfo.Platform == DevicePlatform.Android;
        public bool IsIos => DeviceInfo.Platform == DevicePlatform.iOS;

        // AppInfo
        public string AppVersion => AppInfo.VersionString;
        public string BuildNumber => AppInfo.BuildString;
        public string AppPackageName => AppInfo.PackageName;

        // PhoneDialer
        public void PhoneDialerOpen(string number) => PhoneDialer.Open(number);

        // Store URL
        public string StoreUrl
        {
            get {
                if (IsAndroid)
                {
                    return AppSettings.Instance.GooglePlayUrl;
                }
                return AppSettings.Instance.AppStoreUrl;
            }
        }
    }
}
