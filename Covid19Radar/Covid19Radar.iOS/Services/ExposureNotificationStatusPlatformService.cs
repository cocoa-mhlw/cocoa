/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Xamarin.ExposureNotifications;

namespace Covid19Radar.iOS.Services
{
    public class ExposureNotificationStatusPlatformService : IExposureNotificationStatusPlatformService
    {
        private readonly ILoggerService _loggerService;

        public ExposureNotificationStatusPlatformService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public async Task<bool> GetExposureNotificationEnabledAsync()
        {
            var status = await ExposureNotification.GetStatusAsync();
            _loggerService.Info($"status: {status}");
            return status == Status.Active;
        }

        public async Task<bool> GetBluetoothEnabledAsync()
        {
            var status = await ExposureNotification.GetStatusAsync();
            _loggerService.Info($"status: {status}");
            return status != Status.BluetoothOff;
        }

        public Task<bool> GetGpsEnabledAsync()
        {
            // Not used on iOS.
            throw new PlatformNotSupportedException();
        }
    }
}
