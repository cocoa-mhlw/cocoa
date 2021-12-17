/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Diagnostics;
using System.IO;

namespace Covid19Radar.Services.Logs
{
    public class LogPathService : ILogPathService
    {
        #region Instance Properties

        public string LogsDirPath => localPathService.LogsDirPath;
        public string LogFileWildcardName => logFilePrefix + "*." + logFileExtension;
        public string LogUploadingTmpPath => localPathService.LogUploadingTmpPath;
        public string LogUploadingPublicPath => localPathService.LogUploadingPublicPath;
        public string LogUploadingFileWildcardName => logUploadingFilePrefix + "*." + logUploadingFileExtension;

        #endregion

        #region Static Fields

        private static readonly string logFilePrefix = "cocoa_log_";
        private static readonly string logFileExtension = "csv";
        private static readonly string logUploadingFilePrefix = "cocoa_log_";
        private static readonly string logUploadingFileExtension = "zip";

        #endregion

        #region Instance Fields

        private readonly ILocalPathService localPathService;

        #endregion

        #region Constructors

        public LogPathService(ILocalPathService localPathService)
        {
            this.localPathService = localPathService;
        }

        #endregion

        #region ILogPathService Methods

        public string LogFilePath(DateTime jstDateTime)
        {
            try
            {
                var dateTimeString = jstDateTime.ToString("yyyyMMdd");
                return Path.Combine(LogsDirPath, LogFileName(dateTimeString));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }

        #endregion

        #region Other Private Methods

        private string LogFileName(string dateTimeString)
        {
            return logFilePrefix + dateTimeString + "." + logFileExtension;
        }

        #endregion
    }
}
