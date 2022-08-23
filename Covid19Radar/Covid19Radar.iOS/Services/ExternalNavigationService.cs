/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Foundation;
using UIKit;

namespace Covid19Radar.iOS.Services
{
    public class ExternalNavigationService : IExternalNavigationService
    {
        private readonly ILoggerService _loggerService;

        public ExternalNavigationService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public void NavigateAppSettings()
        {
            _loggerService.StartMethod();
            try
            {
                var url = new NSUrl(UIApplication.OpenSettingsUrlString);
                if (!UIApplication.SharedApplication.CanOpenUrl(url))
                {
                    _loggerService.Error($"Can not open");
                }
                UIApplication.SharedApplication.OpenUrl(url);
            }
            catch (Exception ex)
            {
                _loggerService.Exception("Failed navigate to application settings", ex);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public bool NavigateBluetoothSettings() => throw new PlatformNotSupportedException();
        public void NavigateLocationSettings() => throw new PlatformNotSupportedException();
    }
}
