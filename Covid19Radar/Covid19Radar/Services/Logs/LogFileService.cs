/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.IO.Compression;
using Covid19Radar.Common;

namespace Covid19Radar.Services.Logs
{
    public class LogFileService : ILogFileService
    {
        #region Static Fields

        private static readonly string logFilePrefix = "cocoa_log_";
        private static readonly string logFileExtension = "zip";

        #endregion

        #region Instance Fields

        private readonly ILoggerService loggerService;
        private readonly ILogPathService logPathService;
        private readonly IBackupAttributeService backupAttributeService;

        #endregion

        #region Constructors

        public LogFileService(
            ILoggerService loggerService,
            ILogPathService logPathService,
            IBackupAttributeService backupAttributeService
            )
        {
            this.loggerService = loggerService;
            this.logPathService = logPathService;
            this.backupAttributeService = backupAttributeService;
        }

        #endregion

        #region ILogFileService Methods

        public string CreateLogId() => Guid.NewGuid().ToString();

        public string CreateZipFileName(string logId)
        {
            return logFilePrefix + logId + "." + logFileExtension;
        }

        public string CreateZipFile(string fileName)
        {
            loggerService.StartMethod();
            try
            {
                var logsDirPath = logPathService.LogsDirPath;
                var logFiles = Directory.GetFiles(logsDirPath, logPathService.LogFileWildcardName);
                if (logFiles.Length == 0)
                {
                    return null;
                }

                var zipFilePath = Path.Combine(logPathService.LogUploadingTmpPath, fileName);
                ZipFile.CreateFromDirectory(logsDirPath, zipFilePath);

                return zipFilePath;
            }
            catch (Exception exception)
            {
                loggerService.Exception("Failed to create uploading file", exception);
                return null;
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        public string CopyLogUploadingFileToPublicPath(string logPath)
        {
            loggerService.StartMethod();
            try
            {
                var logFileName = Path.GetFileName(logPath);
                var tmpPath = logPathService.LogUploadingTmpPath;
                var publicPath = logPathService.LogUploadingPublicPath;
                if (string.IsNullOrEmpty(tmpPath) || string.IsNullOrEmpty(publicPath))
                {
                    return null;
                }
                var destPath = Path.Combine(publicPath, logFileName);
                File.Copy(logPath, destPath, true);

                return destPath;
            }
            catch (Exception exception)
            {
                loggerService.Exception("Failed to copy log file", exception);
                return null;
            }
            finally
            {
                loggerService.EndMethod();
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
            catch (Exception)
            {
                loggerService.Error("Failed to delete uploading file");
                loggerService.EndMethod();
                return false;
            }

        }

        public void SetSkipBackupAttributeToLogDir() => backupAttributeService.SetSkipBackupAttributeToLogDir();

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
            try
            {
                var logsDirPath = logPathService.LogsDirPath;
                Directory.Delete(logsDirPath, true);
                return true;
            }
            catch
            {
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
