// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface IDialogService
    {
        Task<bool> ShowExposureNotificationOffWarningAsync();
        Task<bool> ShowBluetoothOffWarningAsync();
        Task<bool> ShowLocationOffWarningAsync();
        Task ShowTemporarilyUnavailableWarningAsync();
        Task ShowHomePageUnknownErrorWaringAsync();
        Task<bool> ShowLocalNotificationOffWarningAsync();
        Task ShowUserProfileNotSupportAsync();
        Task ShowEventLogSaveCompletedAsync();
    }
}
