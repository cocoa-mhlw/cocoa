/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Api.Models
{
    /// <summary>
    /// Payload for http request message.
    /// </summary>
    public interface IPayload
    {
        /// <summary>
        /// Validation Results
        /// </summary>
        /// <returns>true if valid</returns>
        bool IsValid();
    }
}
