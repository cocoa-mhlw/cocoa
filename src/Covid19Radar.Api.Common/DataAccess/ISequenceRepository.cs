/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;

namespace Covid19Radar.Api.DataAccess
{
    public interface ISequenceRepository
    {
        Task<ulong> GetNextAsync(string key, ulong startNo, int increment = 1);
        Task<ulong> GetNextAsync(Common.PartitionKeyRotation.KeyInformation key, ulong startNo, int increment = 1);
    }
}
