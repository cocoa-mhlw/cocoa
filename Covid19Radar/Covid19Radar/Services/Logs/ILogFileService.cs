/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

namespace Covid19Radar.Services.Logs
{
    public interface ILogFileService
    {
        // Log upload
        string CreateLogId();
        string CreateZipFileName(string logId);
        string CreateZipFile(string fileName);
        string CopyLogUploadingFileToPublicPath(string logPath);
        bool DeleteAllLogUploadingFiles();

        // Log rotate
        void SetSkipBackupAttributeToLogDir();
        void Rotate();
        bool DeleteLogsDir();
    }
}
