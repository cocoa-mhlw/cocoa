/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.IO.Compression;
using Covid19Radar.Common;
using Xamarin.Forms;

namespace Covid19Radar.Services.Logs
{
    public class LogFileService : ILogFileService
    {
        #region Static Fields

        private static readonly string logUploadFilePrefix = "cocoa_log_";
        private static readonly string logUploadFileExtension = "zip";

        #endregion

        #region Instance Fields

        private readonly ILoggerService loggerService;
        private readonly ILogPathService logPathService;

        #endregion

        #region Constructors

        public LogFileService(ILoggerService loggerService, ILogPathService logPathService)
        {
            this.loggerService = loggerService;
            this.logPathService = logPathService;
        }

        #endregion

        #region ILogFileService Methods

        public string CreateLogId() => Guid.NewGuid().ToString();

        public string LogUploadingFileName(string logId)
        {
            return logUploadFilePrefix + logId + "." + logUploadFileExtension;
        }

        public bool CreateLogUploadingFileToTmpPath(string logUploadingFileName)
        {
            loggerService.StartMethod();
            try
            {
                var logsDirPath = logPathService.LogsDirPath;
                var logFiles = Directory.GetFiles(logsDirPath, logPathService.LogFileWildcardName);
                if (logFiles.Length == 0)
                {
                    loggerService.EndMethod();
                    return false;
                }
                ZipFile.CreateFromDirectory(logsDirPath, Path.Combine(logPathService.LogUploadingTmpPath, logUploadingFileName));
                loggerService.EndMethod();
                return true;
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed to create uploading file", ex);
                loggerService.EndMethod();
                return false;
            }
        }

        public bool CopyLogUploadingFileToPublicPath(string logUploadingFileName)
        {
            loggerService.StartMethod();
            try
            {
                var tmpPath = logPathService.LogUploadingTmpPath;
                var publicPath = logPathService.LogUploadingPublicPath;
                if (string.IsNullOrEmpty(tmpPath) || string.IsNullOrEmpty(publicPath))
                {
                    loggerService.EndMethod();
                    return false;
                }
                File.Copy(Path.Combine(tmpPath, logUploadingFileName), Path.Combine(publicPath, logUploadingFileName), true);
                loggerService.EndMethod();
                return true;
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed to copy log file", ex);
                loggerService.EndMethod();
                return false;
            }
        }

        public bool DeleteAllLogUploadingFiles()
        {
            loggerService.StartMethod();
            try
            {
                var tmpPath = logPathService.LogUploadingTmpPath;
                if (string.IsNullOrEmpty(tmpPath))
                {
                    loggerService.EndMethod();
                    return false;
                }
                var uploadingFiles = Directory.GetFiles(tmpPath, logPathService.LogUploadingFileWildcardName);
                foreach (string fileName in uploadingFiles)
                {
                    File.Delete(fileName);
                }
                if (uploadingFiles.Length > 0)
                {
                    loggerService.Info($"Deleted {uploadingFiles.Length} file(s)");
                }
                loggerService.EndMethod();
                return true;
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed to delete uploading file", ex);
                loggerService.EndMethod();
                return false;
            }

        }

        public void AddSkipBackupAttribute() => DependencyService.Get<ILogFileDependencyService>().AddSkipBackupAttribute();

        public void Rotate()
        {
            loggerService.StartMethod();
            try
            {
                var dateTimes = Utils.JstDateTimes(14);
                var logsDirPath = logPathService.LogsDirPath;
                var logFiles = Directory.GetFiles(logsDirPath, logPathService.LogFileWildcardName);
                foreach (string fileName in logFiles)
                {
                    if (ShouldDeleteFile(dateTimes, fileName))
                    {
                        File.Delete(fileName);
                        loggerService.Info($"Deleted '{Path.GetFileName(fileName)}'");
                    }
                }
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed to log files rotate.", ex);
            }
            loggerService.EndMethod();
        }

        public bool DeleteLogsDir()
        {
            loggerService.StartMethod();
            try
            {
                var logsDirPath = logPathService.LogsDirPath;
                Directory.Delete(logsDirPath, true);
                loggerService.Info("Deleted all log files.");
                loggerService.EndMethod();
                return true;
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed to Delete all log files.", ex);
                loggerService.EndMethod();
                return false;
            }
        }

        #endregion

        #region Other Private Methods

        private bool ShouldDeleteFile(DateTime[] dateTimes, string fileName)
        {
            foreach (DateTime dateTime in dateTimes)
            {
                if (fileName.Contains(dateTime.ToString("yyyyMMdd")))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
