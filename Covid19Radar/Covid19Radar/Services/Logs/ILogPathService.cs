using System;

namespace Covid19Radar.Services.Logs
{
    public interface ILogPathService
    {
        // Logger
        string LogsDirPath { get; }
        string LogFileWildcardName { get; }
        string LogFilePath(DateTime jstDateTime);

        // Log upload
        string LogUploadingTmpPath { get; }
        string LogUploadingPublicPath { get; }
        string LogUploadingFileWildcardName { get; }
    }
}
