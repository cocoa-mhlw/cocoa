/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using ExposureNotifications;

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
            var manager = await GetManager();
            var status = manager.ExposureNotificationStatus;
            _loggerService.Info($"status: {status}");
            return status == ENStatus.Active;
        }

        public async Task<bool> GetBluetoothEnabledAsync()
        {
            var manager = await GetManager();
            var status = manager.ExposureNotificationStatus;
            _loggerService.Info($"status: {status}");
            return status != ENStatus.BluetoothOff;
        }

        public Task<bool> GetGpsEnabledAsync()
        {
            // Not used on iOS.
            throw new PlatformNotSupportedException();
        }

        private ENManager _manager = null;
        private bool _managerActivated = false;

        private Task<ENManager> GetManager()
        {
            var taskCompletionSource = new TaskCompletionSource<ENManager>();
            lock (this)
            {
                if (_manager == null)
                {
                    _manager = new ENManager();
                }
                if (!_managerActivated)
                {
                    _manager.Activate((error) => {
                        if (error == null || error.Code == (long)ENErrorCode.Ok)
                        {
                            _loggerService.Info("activated");
                            _managerActivated = true;
                            taskCompletionSource.SetResult(_manager);
                        }
                        else
                        {
                            _loggerService.Error($"error: {error}");
                            _manager = null;
                            _managerActivated = false;
                            taskCompletionSource.SetException(new Exception("Failed activate of ENManager."));
                        }
                    });
                }
                else
                {
                    taskCompletionSource.SetResult(_manager);
                }
            }
            return taskCompletionSource.Task;
        }
    }
}
