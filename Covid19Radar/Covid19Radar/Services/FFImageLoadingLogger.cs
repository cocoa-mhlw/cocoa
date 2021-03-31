/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using FFImageLoading.Helpers;
using Prism.Logging;
using System;
using System.Collections.Generic;

namespace Covid19Radar.Services
{
    public class FFImageLoadingLogger : IMiniLogger
    {
        private ILogger _logger { get; }

        public FFImageLoadingLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string errorMessage)
        {
            _logger.Warn(errorMessage);
        }

        public void Error(string errorMessage, Exception ex)
        {
            _logger.Report(ex, new Dictionary<string, string>{
                { "message", errorMessage }
            });
        }
    }
}
