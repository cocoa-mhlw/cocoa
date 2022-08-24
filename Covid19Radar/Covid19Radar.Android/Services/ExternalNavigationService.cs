/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Android.Content;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Droid.Services
{
    public class ExternalNavigationService : IExternalNavigationService
    {
        private readonly ILoggerService _loggerService;
        private readonly IEssentialsService _essentialsService;
        private readonly IPlatformService _platformService;

        public ExternalNavigationService(ILoggerService loggerService, IEssentialsService essentialsService, IPlatformService platformService)
        {
            _loggerService = loggerService;
            _essentialsService = essentialsService;
            _platformService = platformService;
        }

        public void NavigateAppSettings()
        {
            _loggerService.StartMethod();
            try
            {
                var intent = new Intent();
                intent.SetAction(Android.Provider.Settings.ActionApplicationDetailsSettings);
                intent.SetData(Android.Net.Uri.Parse($"package:{_essentialsService.AppPackageName}"));
                _platformService.CurrentActivity.StartActivity(intent);
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

        public bool NavigateBluetoothSettings()
        {
            _loggerService.StartMethod();
            try
            {
                var intent = new Intent();
                intent.SetAction(Android.Provider.Settings.ActionBluetoothSettings);
                _platformService.CurrentActivity.StartActivity(intent);
            }
            catch (Exception ex)
            {
                _loggerService.Exception("Failed navigate to bluetooth settings", ex);
                return false;
            }
            finally
            {
                _loggerService.EndMethod();
            }
            return true;
        }

        public void NavigateLocationSettings()
        {
            _loggerService.StartMethod();
            try
            {
                var intent = new Intent();
                intent.SetAction(Android.Provider.Settings.ActionLocationSourceSettings);
                _platformService.CurrentActivity.StartActivity(intent);
            }
            catch (Exception ex)
            {
                _loggerService.Exception("Failed navigate to location settings", ex);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }
}
