// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System;
using Xamarin.Essentials;

namespace Covid19Radar.Common
{
    public interface IDeviceInfoUtility
    {
        public string Model { get; }
    }

    public class DeviceInfoUtility : IDeviceInfoUtility
    {
        public string Model => DeviceInfo.Model;
    }
}
