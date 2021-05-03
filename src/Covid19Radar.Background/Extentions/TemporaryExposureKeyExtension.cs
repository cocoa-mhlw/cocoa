/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Models;
using Covid19Radar.Background.Protobuf;
using Google.Protobuf;

namespace Covid19Radar.Background.Extentions
{
    public static class TemporaryExposureKeyModelExtension
    {
        public static TemporaryExposureKey ToKey(this TemporaryExposureKeyModel tek)
        {
            return new TemporaryExposureKey()
            {
                KeyData = ByteString.CopyFrom(tek.KeyData),
                RollingStartIntervalNumber = tek.RollingStartIntervalNumber,
                RollingPeriod = tek.RollingPeriod,
                TransmissionRiskLevel = tek.TransmissionRiskLevel
            };
        }
    }
}
