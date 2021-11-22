/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public class ExposureNotificationStatusService : IExposureNotificationStatusService
    {
        #region IExposureNotificationStatusService Instance Properties

        // !!!: Must be referenced after the call to `UpdateStatuses()` .
        public ExposureNotificationStatus ExposureNotificationStatus
        {
            get
            {
                if (IsStopped)
                {
                    return ExposureNotificationStatus.Stopped;
                }
                if (!CanConfirmExposure)
                {
                    return ExposureNotificationStatus.Unconfirmed;
                }
                return ExposureNotificationStatus.Active;
            }
        }

        // !!!: Must be referenced after the call to `UpdateStatuses()` .
        public ExposureNotificationStoppedReason ExposureNotificationStoppedReason
        {
            get
            {
                if (!_gpsEnabled)
                {
                    return ExposureNotificationStoppedReason.GpsOff;
                }
                if (!_bluetoothEnabled)
                {
                    return ExposureNotificationStoppedReason.BluetoothOff;
                }
                if (!_exposureNotificationEnabled)
                {
                    return ExposureNotificationStoppedReason.ExposureNotificationOff;
                }
                return ExposureNotificationStoppedReason.NotStopping;
            }
        }

        #endregion

        #region Instance Fields

        private readonly ILoggerService _loggerService;
        private readonly IEssentialsService _essentialsService;
        private readonly IExposureNotificationStatusPlatformService _exposureNotificationStatusPlatformService;
        private readonly IUserDataRepository _userDataRepository;

        private bool _exposureNotificationEnabled = true;
        private bool _bluetoothEnabled = true;
        private bool _gpsEnabled = true;

        #endregion

        #region Instance Properties

        private bool IsStopped => ExposureNotificationStoppedReason != ExposureNotificationStoppedReason.NotStopping;
        private bool CanConfirmExposure => _userDataRepository.IsCanConfirmExposure();

        #endregion

        #region Constructors

        public ExposureNotificationStatusService(
            ILoggerService loggerService,
            IEssentialsService essentialsService,
            IExposureNotificationStatusPlatformService exposureNotificationPlatformService,
            IUserDataRepository userDataRepository
            )
        {
            _loggerService = loggerService;
            _essentialsService = essentialsService;
            _exposureNotificationStatusPlatformService = exposureNotificationPlatformService;
            _userDataRepository = userDataRepository;
        }

        #endregion

        #region IExposureNotificationStatusService Methods

        public async Task UpdateStatuses()
        {
            _loggerService.StartMethod();
            try
            {
                if (_essentialsService.IsAndroid)
                {
                    if (_essentialsService.DeviceVersion.CompareTo(new Version("11.0")) < 0)
                    {
                        _gpsEnabled = await _exposureNotificationStatusPlatformService.GetGpsEnabledAsync();
                    }
                    else
                    {
                        _gpsEnabled = true;
                    }

                    _bluetoothEnabled = await _exposureNotificationStatusPlatformService.GetBluetoothEnabledAsync();
                    _exposureNotificationEnabled = await _exposureNotificationStatusPlatformService.GetExposureNotificationEnabledAsync();

                    _loggerService.Info($"_gpsEnabled={_gpsEnabled}");
                }
                else if (_essentialsService.IsIos)
                {
                    _gpsEnabled = true;
                    _bluetoothEnabled = await _exposureNotificationStatusPlatformService.GetBluetoothEnabledAsync();
                    _exposureNotificationEnabled = await _exposureNotificationStatusPlatformService.GetExposureNotificationEnabledAsync();
                }

                _loggerService.Info($"_bluetoothEnabled={_bluetoothEnabled}");
                _loggerService.Info($"_exposureNotificationEnabled={_exposureNotificationEnabled}");
            }
            catch (Exception ex)
            {
                _loggerService.Exception("Failed to update exposure notification statuses", ex);
            }
            finally
            {
                _loggerService.Info($"ExposureNotificationStatus: {ExposureNotificationStatus}");
                _loggerService.Info($"ExposureNotificationStoppedReason: {ExposureNotificationStoppedReason}");
                _loggerService.EndMethod();
            }
        }

        #endregion
    }
}
