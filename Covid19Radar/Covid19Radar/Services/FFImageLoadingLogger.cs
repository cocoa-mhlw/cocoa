/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Covid19Radar.Services.Logs;
using FFImageLoading.Helpers;
using Prism.Logging;

namespace Covid19Radar.Services
{
    public class FFImageLoadingLogger : IMiniLogger
    {
        private readonly ILogger        _prism_logger;
        private readonly ILoggerService _cocoa_logger;

        public FFImageLoadingLogger(ILogger prismLogger, ILoggerService cocoaLogger)
        {
            _prism_logger = prismLogger;
            _cocoa_logger = cocoaLogger;
        }

        public void Debug(string message)
        {
            _prism_logger.Debug(message);
            _cocoa_logger.Debug(message);
        }

        public void Error(string errorMessage)
        {
            _prism_logger.Warn(errorMessage);
            _cocoa_logger.Error(errorMessage);
        }

        public void Error(string errorMessage, Exception ex)
        {
            _prism_logger.Report(ex, new Dictionary<string, string>{
                { "message", errorMessage }
            });
            _cocoa_logger.Exception(errorMessage, ex);
        }
    }
}
