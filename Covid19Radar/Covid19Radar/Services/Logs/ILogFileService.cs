namespace Covid19Radar.Services.Logs
{
    public interface ILogFileService
    {
        // Log upload
        string CreateLogId();
        string LogUploadingFileName(string logId);
        bool CreateLogUploadingFileToTmpPath(string logUploadingFileName);
        bool CopyLogUploadingFileToPublicPath(string logUploadingFileName);
        bool DeleteAllLogUploadingFiles();

        // Log rotate
        void AddSkipBackupAttribute();
        void Rotate();
        bool DeleteLogsDir();
    }
}
