/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Services
{
    public interface IEssentialsService
    {
        // DeviceInfo
        string Platform { get; }
        string PlatformVersion { get; }
        string Model { get; }
        string DeviceType { get; }

        // AppInfo
        string AppVersion { get; }
        string BuildNumber { get; }
    }
}
