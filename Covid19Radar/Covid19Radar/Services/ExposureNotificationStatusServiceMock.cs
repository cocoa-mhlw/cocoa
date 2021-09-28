// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public class ExposureNotificationStatusServiceMock : IExposureNotificationStatusService
    {
        public ExposureNotificationStatusServiceMock()
        {
        }

        public ExposureNotificationStatus ExposureNotificationStatus => ExposureNotificationStatus.Active;
        public ExposureNotificationStoppedReason ExposureNotificationStoppedReason => ExposureNotificationStoppedReason.NotStopping;

        public Task UpdateStatuses() => Task.CompletedTask;

    }
}
