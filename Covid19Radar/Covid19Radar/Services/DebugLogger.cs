/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Services.Logs;
using FFImageLoading.Helpers;
using Prism.Logging;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    public class DebugLogger : ILoggerFacade, IMiniLogger
    {
        private readonly ILoggerService _logger;

        public DebugLogger()
        {
            _logger = DependencyService.Resolve<ILoggerService>();
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string errorMessage)
        {
            _logger.Error(errorMessage);
        }

        public void Error(string errorMessage, Exception ex)
        {
            _logger.Exception(errorMessage, ex);
        }

        public void Log(string message, Category category, Priority priority)
        {
            message = $"{nameof(Priority)}.{priority} - {message}";

            switch (category) {
            case Category.Debug:
                _logger.Debug(message);
                break;
            case Category.Info:
                _logger.Info(message);
                break;
            case Category.Warn:
                _logger.Warning(message);
                break;
            case Category.Exception:
                _logger.Error(message);
                break;
            default:
                _logger.Verbose(message);
                break;
            }
        }
    }
}
