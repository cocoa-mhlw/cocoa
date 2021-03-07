using Covid19Radar.Services.Logs;

namespace Covid19Radar.Droid.Services.Logs
{
    public class LogFileServiceAndroid : ILogFileDependencyService
    {
        public void AddSkipBackupAttribute()
        {
            // Skip backup in `AndroidManifest.xml`
        }
    }
}
