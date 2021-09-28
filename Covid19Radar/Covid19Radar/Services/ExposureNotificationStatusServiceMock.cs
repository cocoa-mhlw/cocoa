// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading.Tasks;
using Covid19Radar.Common;

namespace Covid19Radar.Services
{
    public class ExposureNotificationStatusServiceMock : IExposureNotificationStatusService
    {
        private readonly IPreferencesService _preferencesService;
        public ExposureNotificationStatusServiceMock(IPreferencesService preferencesService)
        {
            _preferencesService = preferencesService;
        }

        public ExposureNotificationStatus ExposureNotificationStatus => ExposureNotificationStatus.Active;
        public ExposureNotificationStoppedReason ExposureNotificationStoppedReason => ExposureNotificationStoppedReason.NotStopping;
        public DateTime? LastConfirmedUtcDateTime => DateTime.UtcNow;

        public Task UpdateStatuses() => Task.CompletedTask;

        public void RemoveAllExposureNotificationStatus()
        {
            _preferencesService.RemoveValue(PreferenceKey.LastConfirmedUtcDateTime);
            _preferencesService.RemoveValue(PreferenceKey.CanConfirmExposure);
        }
    }
}
