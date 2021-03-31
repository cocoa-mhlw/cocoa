/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services.Logs
{
    public class LogPathServiceTests
    {
        #region Test Methods

        #region LogFilePath()

        [Theory]
        [InlineData(1, 1, 1, "cocoa_log_00010101.csv")]
        [InlineData(10, 1, 10, "cocoa_log_00100110.csv")]
        [InlineData(100, 10, 1, "cocoa_log_01001001.csv")]
        [InlineData(2020, 11, 1, "cocoa_log_20201101.csv")]
        [InlineData(9999, 12, 31, "cocoa_log_99991231.csv")]
        public void LogFilePath_Success(int year, int month, int day, string expected)
        {
            var mockILogPathDependencyService = CreateDefaultMockILogPathDependencyService();
            var logPathService = CreateDefaultLogPathService(mockILogPathDependencyService);
            var dateTime = new DateTime(year, month, day);
            var jstDateTime = TimeZoneInfo.ConvertTime(dateTime, JstTimeZoneInfo());

            var actual = logPathService.LogFilePath(jstDateTime);

            Assert.Equal(Path.Combine("~/.cocoa/logs", expected), actual);
            Mock.Get(mockILogPathDependencyService).Verify(s => s.LogsDirPath, Times.Once());
            Mock.Get(mockILogPathDependencyService).Verify(s => s.LogUploadingTmpPath, Times.Never());
            Mock.Get(mockILogPathDependencyService).Verify(s => s.LogUploadingPublicPath, Times.Never());
        }

        #endregion

        #endregion

        #region Other Private Methods

        private LogPathService CreateDefaultLogPathService(ILogPathDependencyService logPathDependencyService)
        {
            return new LogPathService(logPathDependencyService);
        }

        private ILogPathDependencyService CreateDefaultMockILogPathDependencyService()
        {
            var mock = Mock.Of<ILogPathDependencyService>(s =>
            s.LogsDirPath == "~/.cocoa/logs" &&
            s.LogUploadingTmpPath == "~/.cocoa/tmp" &&
            s.LogUploadingPublicPath == "~/.cocoa/public"
            );

            return mock;
        }

        private TimeZoneInfo JstTimeZoneInfo()
        {
            // iOS/Android/Unix
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
            }
            catch (TimeZoneNotFoundException)
            {
                // Not iOS/Android/Unix
            }

            // Windows
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                // Not Windows
            }

            // Emergency fallback
            return TimeZoneInfo.CreateCustomTimeZone("JST", new TimeSpan(9, 0, 0), "(GMT+09:00) JST", "JST");
        }

        #endregion
    }
}
