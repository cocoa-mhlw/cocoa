/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Azure.Cosmos;

namespace Covid19Radar.Api.DataStore
{
    public interface ICosmos
    {
        Container User { get; }
        Container Notification { get; }
        Container TemporaryExposureKey { get; }
        Container Diagnosis { get; }
        Container TemporaryExposureKeyExport { get; }
        Container Sequence { get; }
        Container AuthorizedApp { get; }
        Container CustomVerificationStatus { get; }
    }
}
