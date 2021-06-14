/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#nullable enable

using System;
using System.Runtime.CompilerServices;
using Covid19Radar.Services.Logs;
using FFImageLoading.Helpers;
using Prism.Logging;
using Xamarin.Forms;

namespace Covid19Radar.Services
{
    public class DebugLogger : ILoggerFacade, IMiniLogger
    {
        private ILoggerService? _logger;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ILoggerService? GetLogger()
        {
            if (_logger is null) {
                _logger = DependencyService.Resolve<ILoggerService>();
            }
            return _logger;
        }

        public void Debug(string message)
        {
            this.GetLogger()?.Debug(message);
        }

        public void Error(string errorMessage)
        {
            this.GetLogger()?.Error(errorMessage);
        }

        public void Error(string errorMessage, Exception ex)
        {
            this.GetLogger()?.Exception(errorMessage, ex);
        }

        public void Log(string message, Category category, Priority priority)
        {
            var logger = this.GetLogger();
            if (!(logger is null)) {
                message = $"{nameof(Priority)}.{priority} - {message}";

                switch (category) {
                case Category.Debug:
                    logger.Debug(message);
                    break;
                case Category.Info:
                    logger.Info(message);
                    break;
                case Category.Warn:
                    logger.Warning(message);
                    break;
                case Category.Exception:
                    logger.Error(message);
                    break;
                default:
                    logger.Verbose(message);
                    break;
                }
            }
        }
    }
}
