/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Api.Models
{
    public interface IAppleDeviceVerification
    {
		public string Platform { get; set; }

		public string AppPackageName { get; set; }

		public string[] Regions { get; }

		public string KeysText { get; }

		public string DeviceVerificationPayload { get; }

		public string VerificationPayload { get; set; }
	}
}
