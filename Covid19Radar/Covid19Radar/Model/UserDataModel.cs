/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Covid19Radar.Model
{
    // Changed the way this information is held. Please do not use it in the future.
    public class UserDataModel
    {
        public DateTime StartDateTime { get; set; }
        public bool IsOptined { get; set; } = false;
        public bool IsPolicyAccepted { get; set; } = false;
        public Dictionary<string, long> LastProcessTekTimestamp { get; set; } = new Dictionary<string, long>();
        public ObservableCollection<UserExposureInfo> ExposureInformation { get; set; } = new ObservableCollection<UserExposureInfo>();
        public UserExposureSummary ExposureSummary { get; set; }
    }
}
