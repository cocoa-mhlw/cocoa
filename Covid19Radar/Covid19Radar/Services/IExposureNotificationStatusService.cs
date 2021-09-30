/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public enum ExposureNotificationStatus
    {
        Active,
        Stopped,
        Unconfirmed
    }

    public enum ExposureNotificationStoppedReason
    {
        NotStopping,
        ExposureNotificationOff,
        BluetoothOff,
        GpsOff
    }

    public interface IExposureNotificationStatusService
    {
        ExposureNotificationStatus ExposureNotificationStatus { get; }
        ExposureNotificationStoppedReason ExposureNotificationStoppedReason { get; }
        DateTime? LastConfirmedUtcDateTime { get; }

        Task UpdateStatuses();
        void RemoveAllExposureNotificationStatus();
    }

    public interface IExposureNotificationStatusPlatformService
    {
        Task<bool> GetExposureNotificationEnabledAsync();
        Task<bool> GetBluetoothEnabledAsync();
        Task<bool> GetGpsEnabledAsync();
    }
}
