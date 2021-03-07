using Covid19Radar.Services.Logs;
using Foundation;

namespace Covid19Radar.iOS.Services.Logs
{
    public class LogFileServiceIos : ILogFileDependencyService
    {
        private readonly ILogPathService logPathService;

        public LogFileServiceIos(ILogPathService logPathService)
        {
            this.logPathService = logPathService;
        }

        public void AddSkipBackupAttribute()
        {
            var logsDirPath = logPathService.LogsDirPath;
            var url = NSUrl.FromFilename(logsDirPath);
            _ = url.SetResource(NSUrl.IsExcludedFromBackupKey, NSNumber.FromBoolean(true));
        }
    }
}
