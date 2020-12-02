using System;
using System.Diagnostics;
using System.IO;

namespace Covid19Radar.Services.Logs
{
    public class LogPathService : ILogPathService
    {
        #region Instance Properties

        public string LogsDirPath => logPathDependencyService.LogsDirPath;
        public string LogFileWildcardName => logFilePrefix + "*." + logFileExtension;
        public string LogUploadingTmpPath => logPathDependencyService.LogUploadingTmpPath;
        public string LogUploadingPublicPath => logPathDependencyService.LogUploadingPublicPath;
        public string LogUploadingFileWildcardName => logUploadingFilePrefix + "*." + logUploadingFileExtension;

        #endregion

        #region Static Fields

        private static readonly string logFilePrefix = "cocoa_log_";
        private static readonly string logFileExtension = "csv";
        private static readonly string logUploadingFilePrefix = "cocoa_log_";
        private static readonly string logUploadingFileExtension = "zip";

        #endregion

        #region Instance Fields

        private readonly ILogPathDependencyService logPathDependencyService;

        #endregion

        #region Constructors

        public LogPathService(ILogPathDependencyService logPathDependencyService)
        {
            this.logPathDependencyService = logPathDependencyService;
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
