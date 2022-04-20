/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Linq;

namespace Covid19Radar.Api.Models
{
    public interface IAndroidDeviceVerification
    {
        public string ClearText { get; }

        public string JwsPayload { get; }

        public static string GetRegionString(string[] regions)
            => string.Join(",", regions.Select(r => r.ToUpperInvariant()).OrderBy(r => r));
    }
}
