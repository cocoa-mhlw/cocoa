﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;

namespace Covid19Radar.Api.Models
{
    public class AuthorizedAppInformation
    {
        public string id { get => Platform; }

        public string PackageName { get; set; }

        public string Platform { get; set; }

        public string[] AllowedRegions { get; set; }
        public bool DeviceValidationEnabled { get; set; }

        // SafetyNet configuration

        public string[] SafetyNetApkDigestSha256 { get; set; }

        public bool SafetyNetBasicIntegrity { get; set; }

        public bool SafetyNetCtsProfileMatch { get; set; }

        public int SafetyNetPastTimeSeconds { get; set; }

        public int SafetyNetFutureTimeSeconds { get; set; }

        // DeviceCheck configuration

        public string DeviceCheckKeyId { get; set; }

        public string DeviceCheckTeamId { get; set; }

        public string DeviceCheckPrivateKey { get; set; }


        // Utils
        public static DevicePlatform ParsePlatform(string platform) =>
            platform.Equals("android", StringComparison.OrdinalIgnoreCase)
                ? DevicePlatform.Android
                : DevicePlatform.iOS;

        public enum DevicePlatform
        {
            iOS,
            Android
        }
    }
}
