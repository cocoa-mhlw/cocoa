/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿namespace Covid19Radar.Services.Logs
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
