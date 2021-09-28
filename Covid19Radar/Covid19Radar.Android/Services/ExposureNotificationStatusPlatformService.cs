/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Extensions;
using Android.Gms.Nearby;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using ExposureNotificationStatus = Android.Gms.Nearby.ExposureNotification.ExposureNotificationStatus;

namespace Covid19Radar.Droid.Services
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
            var statuses = await GetExposureNotificationStatuses();
            var activated = statuses.Contains(ExposureNotificationStatus.Activated);
            _loggerService.Info($"activated: {activated}");
            return activated;
        }

        public async Task<bool> GetBluetoothEnabledAsync()
        {
            var statuses = await GetExposureNotificationStatuses();
            var enabled = !statuses.Contains(ExposureNotificationStatus.BluetoothDisabled);
            _loggerService.Info($"bluetooth enabled: {enabled}");
            return enabled;
        }

        public async Task<bool> GetGpsEnabledAsync()
        {
            var statuses = await GetExposureNotificationStatuses();
            var enabled = !statuses.Contains(ExposureNotificationStatus.LocationDisabled);
            _loggerService.Info($"location enabled: {enabled}");
            return enabled;
        }

        private async Task<List<ExposureNotificationStatus>> GetExposureNotificationStatuses()
        {
            var statusList = new List<ExposureNotificationStatus>();

            var exposureNotificationClient = NearbyClass.GetExposureNotificationClient(Application.Context);
            var statusTask = exposureNotificationClient.NativeGetStatus();
            var statusCollection = await TasksExtensions.AsAsync<Java.Lang.IIterable>(statusTask);
            _loggerService.Info($"statuses: {statusCollection}");

            for (var i = statusCollection.Iterator(); i.HasNext;)
            {
                statusList.Add((ExposureNotificationStatus)i.Next());
            }

            return statusList;
        }
    }
}
