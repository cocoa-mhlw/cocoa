/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using Covid19Radar.Common;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services.Logs
{
    public class LogFileServiceTests
    {
        private static readonly string logsDirPath = "~/.cocoa/logs/";
        private static readonly string tmpDirPath = "~/.cocoa/tmp_test/";
        private static readonly string publicDirPath = "~/.cocoa/public/";

        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IBackupAttributeService> mockBackupAttributeService;

        public LogFileServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockBackupAttributeService = mockRepository.Create<IBackupAttributeService>();
        }

        [Fact]
        public void LogUploadingFileName_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            var logId = logFileService.CreateLogId();
            var fileName = logFileService.CreateZipFileName(logId);
            Assert.Equal(fileName, "cocoa_log_" + logId + ".zip");
        }

        [Fact]
        public void CreateLogUploadingFileToTmpPath_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            RecreateDir(logsDirPath);
            RecreateLogsFile();
            RecreateDir(tmpDirPath);

            var logId = logFileService.CreateLogId();
            var fileName = logFileService.CreateZipFileName(logId);
            var result = logFileService.CreateZipFile(fileName);
            Assert.Equal(Path.Combine(tmpDirPath, fileName), result);

            var uploadingFilePath = tmpDirPath + fileName;
            Assert.True(File.Exists(uploadingFilePath));
        }

        [Fact]
        public void CreateLogUploadingFileToTmpPath_Log_Not_Exists()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            RecreateDir(logsDirPath);
            RecreateDir(tmpDirPath);

            var logId = logFileService.CreateLogId();
            var fileName = logFileService.CreateZipFileName(logId);
            var result = logFileService.CreateZipFile(fileName);
            Assert.Null(result);

            var uploadingFilePath = tmpDirPath + fileName;
            Assert.False(File.Exists(uploadingFilePath));
        }

        [Fact]
        public void CreateLogUploadingFileToTmpPath_LogDir_Not_Exists()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            DeleteDirIfExists(logsDirPath);
            RecreateDir(tmpDirPath);

            var logId = logFileService.CreateLogId();
            var fileName = logFileService.CreateZipFileName(logId);
            var result = logFileService.CreateZipFile(fileName);
            Assert.Null(result);

            var uploadingFilePath = tmpDirPath + fileName;
            Assert.False(File.Exists(uploadingFilePath));
        }

        [Fact]
        public void CopyLogUploadingFileToPublicPath_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            RecreateDir(logsDirPath);
            RecreateLogsFile();
            RecreateDir(publicDirPath);
            RecreateDir(tmpDirPath);

            var logId = logFileService.CreateLogId();
            var fileName = logFileService.CreateZipFileName(logId);
            var result = logFileService.CreateZipFile(fileName);
            Assert.Equal(Path.Combine(tmpDirPath, fileName), result);

            result = logFileService.CopyLogUploadingFileToPublicPath(Path.Combine(tmpDirPath, fileName));
            Assert.Equal(Path.Combine(publicDirPath, fileName), result);

            var uploadingFilePath = publicDirPath + fileName;
            Assert.True(File.Exists(uploadingFilePath));
        }

        [Fact]
        public void CopyLogUploadingFileToPublicPath_PublicDir_Not_Exists()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            RecreateDir(logsDirPath);
            RecreateLogsFile();
            DeleteDirIfExists(publicDirPath);
            RecreateDir(tmpDirPath);

            var logId = logFileService.CreateLogId();
            var fileName = logFileService.CreateZipFileName(logId);
            var result = logFileService.CreateZipFile(fileName);
            Assert.Equal(result, Path.Combine(tmpDirPath, fileName));

            result = logFileService.CopyLogUploadingFileToPublicPath(Path.Combine(tmpDirPath, fileName));
            Assert.Null(result);

            var uploadingFilePath = publicDirPath + fileName;
            Assert.False(File.Exists(uploadingFilePath));
        }

        [Fact]
        public void CopyLogUploadingFileToPublicPath_TmpPath_Empty()
        {
            var mockILogPathService = CreateTmpPathEmptyMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            var logId = logFileService.CreateLogId();
            var fileName = logFileService.CreateZipFileName(logId);
            var result = logFileService.CopyLogUploadingFileToPublicPath(fileName);
            Assert.Null(result);

            var uploadingFilePath = publicDirPath + fileName;
            Assert.False(File.Exists(uploadingFilePath));
        }

        [Fact]
        public void CopyLogUploadingFileToPublicPath_PublicPath_Empty()
        {
            var mockILogPathService = CreatePublicPathEmptyMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            var logId = logFileService.CreateLogId();
            var fileName = logFileService.CreateZipFileName(logId);
            var result = logFileService.CopyLogUploadingFileToPublicPath(fileName);
            Assert.Null(result);

            var uploadingFilePath = publicDirPath + fileName;
            Assert.False(File.Exists(uploadingFilePath));
        }

        [Fact]
        public void DeleteAllLogUploadingFiles_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            RecreateDir(logsDirPath);
            RecreateLogsFile();
            RecreateDir(tmpDirPath);

            var logId = logFileService.CreateLogId();
            var fileName = logFileService.CreateZipFileName(logId);
            var result = logFileService.CreateZipFile(fileName);
            Assert.Equal(Path.Combine(tmpDirPath, fileName), result);

            bool deleteResult = logFileService.DeleteAllLogUploadingFiles();
            Assert.True(deleteResult);

            var uploadingFilePath = tmpDirPath + fileName;
            Assert.False(File.Exists(uploadingFilePath));
        }

        [Fact]
        public void DeleteAllLogUploadingFiles_TmpPath_Empty()
        {
            var mockILogPathService = CreateTmpPathEmptyMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            var result = logFileService.DeleteAllLogUploadingFiles();
            Assert.False(result);
        }

        [Fact]
        public void DeleteAllLogUploadingFiles_TmpDir_Not_Exists()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            RecreateDir(logsDirPath);
            RecreateLogsFile();
            RecreateDir(tmpDirPath);

            var logId = logFileService.CreateLogId();
            var fileName = logFileService.CreateZipFileName(logId);
            var result = logFileService.CreateZipFile(fileName);
            Assert.Equal(Path.Combine(tmpDirPath, fileName), result);

            DeleteDirIfExists(tmpDirPath);

            bool deleteResult = logFileService.DeleteAllLogUploadingFiles();
            Assert.False(deleteResult);
        }

        [Fact]
        public void Rotate_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            RecreateDir(logsDirPath);

            var dateTimes = Utils.JstDateTimes(21);
            foreach (DateTime dateTime in dateTimes)
            {
                var logFilePath = logsDirPath + "cocoa_log_" + dateTime.ToString("yyyyMMdd") + ".csv";
                File.Create(logFilePath).Close();
            }

            var logFiles = Directory.GetFiles(logsDirPath);
            Assert.Equal(21, logFiles.Length);

            logFileService.Rotate();

            logFiles = Directory.GetFiles(logsDirPath);
            Assert.Equal(14, logFiles.Length);
        }

        [Fact]
        public void Rotate_Exception()
        {
            var mockILogPathService = CreateLogPathEmptyMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            RecreateDir(logsDirPath);

            var dateTimes = Utils.JstDateTimes(21);
            foreach (DateTime dateTime in dateTimes)
            {
                var logFilePath = logsDirPath + "cocoa_log_" + dateTime.ToString("yyyyMMdd") + ".csv";
                File.Create(logFilePath).Close();

            }

            var logFiles = Directory.GetFiles(logsDirPath);
            Assert.Equal(21, logFiles.Length);

            logFileService.Rotate();

            logFiles = Directory.GetFiles(logsDirPath);
            Assert.Equal(21, logFiles.Length);
        }

        [Fact]
        public void DeleteLogsDir_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            RecreateDir(logsDirPath);

            var result = logFileService.DeleteLogsDir();
            Assert.True(result);
            Assert.False(Directory.Exists(logsDirPath));
        }

        [Fact]
        public void DeleteLogsDir_LogDir_Not_Exists()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var logFileService = CreateDefaultLogFileService(mockILogPathService);

            DeleteDirIfExists(logsDirPath);

            var result = logFileService.DeleteLogsDir();
            Assert.False(result);
        }

        private LogFileService CreateDefaultLogFileService(ILogPathService logPathService)
        {
            return new LogFileService(
                mockLoggerService.Object,
                logPathService,
                mockBackupAttributeService.Object
                );
        }

        private ILogPathService CreateDefaultMockILogPathService()
        {
            var mock = Mock.Of<ILogPathService>(s =>
            s.LogsDirPath == logsDirPath &&
            s.LogFileWildcardName == "cocoa_log_*.csv" &&
            s.LogFilePath(It.IsAny<DateTime>()) == logsDirPath + "cocoa_log_20201101.csv" &&
            s.LogUploadingTmpPath == tmpDirPath &&
            s.LogUploadingPublicPath == publicDirPath &&
            s.LogUploadingFileWildcardName == "cocoa_log_*.zip"
            );

            return mock;
        }

        private ILogPathService CreateTmpPathEmptyMockILogPathService()
        {
            var mock = Mock.Of<ILogPathService>(s =>
            s.LogsDirPath == logsDirPath &&
            s.LogFileWildcardName == "cocoa_log_*.csv" &&
            s.LogFilePath(It.IsAny<DateTime>()) == logsDirPath + "cocoa_log_20201101.csv" &&
            s.LogUploadingTmpPath == "" &&
            s.LogUploadingPublicPath == publicDirPath &&
            s.LogUploadingFileWildcardName == "cocoa_log_*.zip"
            );

            return mock;
        }

        private ILogPathService CreatePublicPathEmptyMockILogPathService()
        {
            var mock = Mock.Of<ILogPathService>(s =>
            s.LogsDirPath == logsDirPath &&
            s.LogFileWildcardName == "cocoa_log_*.csv" &&
            s.LogFilePath(It.IsAny<DateTime>()) == logsDirPath + "cocoa_log_20201101.csv" &&
            s.LogUploadingTmpPath == tmpDirPath &&
            s.LogUploadingPublicPath == "" &&
            s.LogUploadingFileWildcardName == "cocoa_log_*.zip"
            );

            return mock;
        }

        private ILogPathService CreateLogPathEmptyMockILogPathService()
        {
            var mock = Mock.Of<ILogPathService>(s =>
            s.LogsDirPath == "" &&
            s.LogFileWildcardName == "cocoa_log_*.csv" &&
            s.LogFilePath(It.IsAny<DateTime>()) == logsDirPath + "cocoa_log_20201101.csv" &&
            s.LogUploadingTmpPath == tmpDirPath &&
            s.LogUploadingPublicPath == publicDirPath &&
            s.LogUploadingFileWildcardName == "cocoa_log_*.zip"
            );

            return mock;
        }

        private void RecreateLogsFile()
        {
            var logFilePath = logsDirPath + "cocoa_log_20201101.csv";
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }
            File.Create(logFilePath).Close();
        }

        private void RecreateDir(string path)
        {
            DeleteDirIfExists(path);
            Directory.CreateDirectory(path);
        }

        private void DeleteDirIfExists(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }

}
