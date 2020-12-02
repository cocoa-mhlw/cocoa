using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;
using Xunit.Sdk;

namespace Covid19Radar.UnitTests.Services.Logs
{
    public class LoggerServiceTests
    {
        #region Test Methods

        #region StartMethod()

        [Theory]
        [InlineData("Foo", "~/Documents/Foo.cs", 1, "\"Info\"", "\"Start\"", "\"Foo\"", "\"~/Documents/Foo.cs\"", "\"1\"")]
        [InlineData("", "", 0, "\"Info\"", "\"Start\"", "\"\"", "\"\"", "\"0\"")]
        [InlineData(null, null, 0, "\"Info\"", "\"Start\"", "\"\"", "\"\"", "\"0\"")]
        public void StartMethod_Success(
            string method,
            string filePath,
            int lineNumber,
            string expectedLogLevel,
            string expectedMessage,
            string expectedMethod,
            string expectedFilePath,
            string expectedLineNumber)
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            DeleteLogsDirIfExists();

            loggerService.StartMethod(method, filePath, lineNumber);

            using (var sr = new StreamReader("~/.cocoa/logs/cocoa_log_20201101.csv"))
            {
                for (var i = 0; i < 2; i++)
                {
                    var logColumns = ParseLogRow(sr.ReadLine());

                    var isHeader = (i == 0);
                    if (isHeader)
                    {
                        continue;
                    }

                    Assert.Equal(expectedLogLevel, logColumns[1]);
                    Assert.Equal(expectedMessage, logColumns[2]);
                    Assert.Equal(expectedMethod, logColumns[3]);
                    Assert.Equal(expectedFilePath, logColumns[4]);
                    Assert.Equal(expectedLineNumber, logColumns[5]);
                }
            }

            Mock.Get(mockIEssentialsService).Verify(s => s.Platform, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.PlatformVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.Model, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.DeviceType, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.AppVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.BuildNumber, Times.Once());
        }

        [Fact]
        public void StartMethod_Success_LogsDir_Not_Exists()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            DeleteLogsDirIfExists();

            Assert.False(Directory.Exists("~/.cocoa/logs"));
            Assert.False(File.Exists("~/.cocoa/logs/cocoa_log_20201101.csv"));

            loggerService.StartMethod();

            Assert.True(Directory.Exists("~/.cocoa/logs"));
            Assert.True(File.Exists("~/.cocoa/logs/cocoa_log_20201101.csv"));

            Mock.Get(mockILogPathService).Verify(s => s.LogsDirPath, Times.Exactly(2));
            Mock.Get(mockILogPathService).Verify(s => s.LogFilePath(It.IsAny<DateTime>()), Times.Once());
        }

        [Fact]
        public void StartMethod_Success_LogsDir_Exists_LogFile_Not_Exists()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            RecreateLogsDir();

            Assert.True(Directory.Exists("~/.cocoa/logs"));
            Assert.False(File.Exists("~/.cocoa/logs/cocoa_log_20201101.csv"));

            loggerService.StartMethod();

            Assert.True(Directory.Exists("~/.cocoa/logs"));
            Assert.True(File.Exists("~/.cocoa/logs/cocoa_log_20201101.csv"));
            Mock.Get(mockILogPathService).Verify(s => s.LogsDirPath, Times.Once());
            Mock.Get(mockILogPathService).Verify(s => s.LogFilePath(It.IsAny<DateTime>()), Times.Once());
        }

        [Fact]
        public void StartMethod_Success_LogsDir_Exists_LogFile_Exists()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            RecreateLogsFile();

            Assert.True(Directory.Exists("~/.cocoa/logs"));
            Assert.True(File.Exists("~/.cocoa/logs/cocoa_log_20201101.csv"));

            loggerService.StartMethod();

            Assert.True(Directory.Exists("~/.cocoa/logs"));
            Assert.True(File.Exists("~/.cocoa/logs/cocoa_log_20201101.csv"));
            Mock.Get(mockILogPathService).Verify(s => s.LogsDirPath, Times.Once());
            Mock.Get(mockILogPathService).Verify(s => s.LogFilePath(It.IsAny<DateTime>()), Times.Once());
        }

        #endregion

        #region EndMethod()

        [Fact]
        public void EndMethod_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            DeleteLogsDirIfExists();

            loggerService.EndMethod("Foo", "~/Documents/Foo.cs", 1);

            using (var sr = new StreamReader("~/.cocoa/logs/cocoa_log_20201101.csv"))
            {
                for (var i = 0; i < 2; i++)
                {
                    var logColumns = ParseLogRow(sr.ReadLine());

                    var isHeader = (i == 0);
                    if (isHeader)
                    {
                        continue;
                    }

                    Assert.Equal("\"Info\"", logColumns[1]);
                    Assert.Equal("\"End\"", logColumns[2]);
                    Assert.Equal("\"Foo\"", logColumns[3]);
                    Assert.Equal("\"~/Documents/Foo.cs\"", logColumns[4]);
                    Assert.Equal("\"1\"", logColumns[5]);
                }
            }

            Mock.Get(mockIEssentialsService).Verify(s => s.Platform, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.PlatformVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.Model, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.DeviceType, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.AppVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.BuildNumber, Times.Once());
        }

        #endregion

        #region Verbose()

        [Fact]
        public void Verbose_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            DeleteLogsDirIfExists();

            loggerService.Verbose("Message", "Foo", "~/Documents/Foo.cs", 1);

            using (var sr = new StreamReader("~/.cocoa/logs/cocoa_log_20201101.csv"))
            {
                for (var i = 0; i < 2; i++)
                {
                    var logColumns = ParseLogRow(sr.ReadLine());

                    var isHeader = (i == 0);
                    if (isHeader)
                    {
                        continue;
                    }

                    Assert.Equal("\"Verbose\"", logColumns[1]);
                    Assert.Equal("\"Message\"", logColumns[2]);
                    Assert.Equal("\"Foo\"", logColumns[3]);
                    Assert.Equal("\"~/Documents/Foo.cs\"", logColumns[4]);
                    Assert.Equal("\"1\"", logColumns[5]);
                }
            }

            Mock.Get(mockIEssentialsService).Verify(s => s.Platform, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.PlatformVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.Model, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.DeviceType, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.AppVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.BuildNumber, Times.Once());
        }

        #endregion

        #region Debug()

        [Fact]
        public void Debug_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            DeleteLogsDirIfExists();

            loggerService.Debug("Message", "Foo", "~/Documents/Foo.cs", 1);

            using (var sr = new StreamReader("~/.cocoa/logs/cocoa_log_20201101.csv"))
            {
                for (var i = 0; i < 2; i++)
                {
                    var logColumns = ParseLogRow(sr.ReadLine());

                    var isHeader = (i == 0);
                    if (isHeader)
                    {
                        continue;
                    }

                    Assert.Equal("\"Debug\"", logColumns[1]);
                    Assert.Equal("\"Message\"", logColumns[2]);
                    Assert.Equal("\"Foo\"", logColumns[3]);
                    Assert.Equal("\"~/Documents/Foo.cs\"", logColumns[4]);
                    Assert.Equal("\"1\"", logColumns[5]);
                }
            }

            Mock.Get(mockIEssentialsService).Verify(s => s.Platform, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.PlatformVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.Model, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.DeviceType, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.AppVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.BuildNumber, Times.Once());
        }

        #endregion

        #region Info()

        [Theory]
        [InlineData("Hello", "\"Hello\"")]
        [InlineData("Hello, world!!", "\"Hello, world!!\"")]
        [InlineData("Hello\r", "\"Hello\"")]
        [InlineData("Hello\n", "\"Hello\"")]
        [InlineData("Hello\r\n", "\"Hello\"")]
        [InlineData("\"Hello\"", "\"\"\"Hello\"\"\"")]
        [InlineData("\"\"Hello\"\"", "\"\"\"\"\"Hello\"\"\"\"\"")]
        [InlineData("\"Hello\": foo, \"world\": bar\n!!", "\"\"\"Hello\"\": foo, \"\"world\"\": bar!!\"")]
        [InlineData(null, "\"\"")]
        [InlineData("", "\"\"")]
        public void Info_Success(string message, string expectedMessage)
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            DeleteLogsDirIfExists();

            loggerService.Info(message, "Foo", "~/Documents/Foo.cs", 1);

            using (var sr = new StreamReader("~/.cocoa/logs/cocoa_log_20201101.csv"))
            {
                for (var i = 0; i < 2; i++)
                {
                    var logColumns = ParseLogRow(sr.ReadLine());

                    var isHeader = (i == 0);
                    if (isHeader)
                    {
                        continue;
                    }

                    Assert.Equal("\"Info\"", logColumns[1]);
                    Assert.Equal(expectedMessage, logColumns[2]);
                    Assert.Equal("\"Foo\"", logColumns[3]);
                    Assert.Equal("\"~/Documents/Foo.cs\"", logColumns[4]);
                    Assert.Equal("\"1\"", logColumns[5]);
                }
            }

            Mock.Get(mockIEssentialsService).Verify(s => s.Platform, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.PlatformVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.Model, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.DeviceType, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.AppVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.BuildNumber, Times.Once());
        }

        [Fact]
        public void Info_Success_Sync()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            DeleteLogsDirIfExists();

            loggerService.Info("Message1", "Foo1", "~/Documents/Foo1.cs", 1);
            loggerService.Info("Message2", "Foo2", "~/Documents/Foo2.cs", 2);
            loggerService.Info("Message3", "Foo3", "~/Documents/Foo3.cs", 3);
            loggerService.Info("Message4", "Foo4", "~/Documents/Foo4.cs", 4);
            loggerService.Info("Message5", "Foo5", "~/Documents/Foo5.cs", 5);
            loggerService.Info("Message6", "Foo6", "~/Documents/Foo6.cs", 6);
            loggerService.Info("Message7", "Foo7", "~/Documents/Foo7.cs", 7);
            loggerService.Info("Message8", "Foo8", "~/Documents/Foo8.cs", 8);
            loggerService.Info("Message9", "Foo9", "~/Documents/Foo9.cs", 9);
            loggerService.Info("Message10", "Foo10", "~/Documents/Foo10.cs", 10);

            var lineNumber = 0;
            string logRow;
            using (var sr = new StreamReader("~/.cocoa/logs/cocoa_log_20201101.csv"))
            {
                while ((logRow = sr.ReadLine()) != null)
                {
                    lineNumber++;

                    var isHeader = (lineNumber == 1);
                    if (isHeader)
                    {
                        continue;
                    }

                    var logColumns = ParseLogRow(logRow);

                    switch (lineNumber)
                    {
                        case 2:
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message1\"", logColumns[2]);
                            Assert.Equal("\"Foo1\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo1.cs\"", logColumns[4]);
                            Assert.Equal("\"1\"", logColumns[5]);
                            break;
                        case 3:
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message2\"", logColumns[2]);
                            Assert.Equal("\"Foo2\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo2.cs\"", logColumns[4]);
                            Assert.Equal("\"2\"", logColumns[5]);
                            break;
                        case 4:
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message3\"", logColumns[2]);
                            Assert.Equal("\"Foo3\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo3.cs\"", logColumns[4]);
                            Assert.Equal("\"3\"", logColumns[5]);
                            break;
                        case 5:
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message4\"", logColumns[2]);
                            Assert.Equal("\"Foo4\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo4.cs\"", logColumns[4]);
                            Assert.Equal("\"4\"", logColumns[5]);
                            break;
                        case 6:
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message5\"", logColumns[2]);
                            Assert.Equal("\"Foo5\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo5.cs\"", logColumns[4]);
                            Assert.Equal("\"5\"", logColumns[5]);
                            break;
                        case 7:
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message6\"", logColumns[2]);
                            Assert.Equal("\"Foo6\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo6.cs\"", logColumns[4]);
                            Assert.Equal("\"6\"", logColumns[5]);
                            break;
                        case 8:
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message7\"", logColumns[2]);
                            Assert.Equal("\"Foo7\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo7.cs\"", logColumns[4]);
                            Assert.Equal("\"7\"", logColumns[5]);
                            break;
                        case 9:
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message8\"", logColumns[2]);
                            Assert.Equal("\"Foo8\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo8.cs\"", logColumns[4]);
                            Assert.Equal("\"8\"", logColumns[5]);
                            break;
                        case 10:
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message9\"", logColumns[2]);
                            Assert.Equal("\"Foo9\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo9.cs\"", logColumns[4]);
                            Assert.Equal("\"9\"", logColumns[5]);
                            break;
                        case 11:
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message10\"", logColumns[2]);
                            Assert.Equal("\"Foo10\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo10.cs\"", logColumns[4]);
                            Assert.Equal("\"10\"", logColumns[5]);
                            break;
                        default:
                            throw new XunitException($"Unexpected message. log_level: {logColumns[1]}, message: {logColumns[2]}, method: {logColumns[3]}, file_path: {logColumns[4]}, line_number: {logColumns[5]}");
                    }
                }
            }

            Assert.Equal(11, lineNumber);
            Mock.Get(mockIEssentialsService).Verify(s => s.Platform, Times.Exactly(10));
            Mock.Get(mockIEssentialsService).Verify(s => s.PlatformVersion, Times.Exactly(10));
            Mock.Get(mockIEssentialsService).Verify(s => s.Model, Times.Exactly(10));
            Mock.Get(mockIEssentialsService).Verify(s => s.DeviceType, Times.Exactly(10));
            Mock.Get(mockIEssentialsService).Verify(s => s.AppVersion, Times.Exactly(10));
            Mock.Get(mockIEssentialsService).Verify(s => s.BuildNumber, Times.Exactly(10));
        }

        [Fact]
        public async void Info_Success_Async()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            DeleteLogsDirIfExists();

            await Task.WhenAll(
                Task.Run(() => loggerService.Info("Message1", "Foo1", "~/Documents/Foo1.cs", 1)),
                Task.Run(() => loggerService.Info("Message2", "Foo2", "~/Documents/Foo2.cs", 2)),
                Task.Run(() => loggerService.Info("Message3", "Foo3", "~/Documents/Foo3.cs", 3)),
                Task.Run(() => loggerService.Info("Message4", "Foo4", "~/Documents/Foo4.cs", 4)),
                Task.Run(() => loggerService.Info("Message5", "Foo5", "~/Documents/Foo5.cs", 5)),
                Task.Run(() => loggerService.Info("Message6", "Foo6", "~/Documents/Foo6.cs", 6)),
                Task.Run(() => loggerService.Info("Message7", "Foo7", "~/Documents/Foo7.cs", 7)),
                Task.Run(() => loggerService.Info("Message8", "Foo8", "~/Documents/Foo8.cs", 8)),
                Task.Run(() => loggerService.Info("Message9", "Foo9", "~/Documents/Foo9.cs", 9)),
                Task.Run(() => loggerService.Info("Message10", "Foo10", "~/Documents/Foo10.cs", 10))
                );

            var lineNumber = 0;
            string logRow;
            using (var sr = new StreamReader("~/.cocoa/logs/cocoa_log_20201101.csv"))
            {
                while ((logRow = sr.ReadLine()) != null)
                {
                    lineNumber++;

                    var isHeader = (lineNumber == 1);
                    if (isHeader)
                    {
                        continue;
                    }

                    var logColumns = ParseLogRow(logRow);
                    Assert.Equal(12, logColumns.Length);

                    var message = logColumns[2].Trim('\"');

                    switch (message)
                    {
                        case "Message1":
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message1\"", logColumns[2]);
                            Assert.Equal("\"Foo1\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo1.cs\"", logColumns[4]);
                            Assert.Equal("\"1\"", logColumns[5]);
                            break;
                        case "Message2":
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message2\"", logColumns[2]);
                            Assert.Equal("\"Foo2\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo2.cs\"", logColumns[4]);
                            Assert.Equal("\"2\"", logColumns[5]);
                            break;
                        case "Message3":
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message3\"", logColumns[2]);
                            Assert.Equal("\"Foo3\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo3.cs\"", logColumns[4]);
                            Assert.Equal("\"3\"", logColumns[5]);
                            break;
                        case "Message4":
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message4\"", logColumns[2]);
                            Assert.Equal("\"Foo4\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo4.cs\"", logColumns[4]);
                            Assert.Equal("\"4\"", logColumns[5]);
                            break;
                        case "Message5":
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message5\"", logColumns[2]);
                            Assert.Equal("\"Foo5\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo5.cs\"", logColumns[4]);
                            Assert.Equal("\"5\"", logColumns[5]);
                            break;
                        case "Message6":
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message6\"", logColumns[2]);
                            Assert.Equal("\"Foo6\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo6.cs\"", logColumns[4]);
                            Assert.Equal("\"6\"", logColumns[5]);
                            break;
                        case "Message7":
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message7\"", logColumns[2]);
                            Assert.Equal("\"Foo7\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo7.cs\"", logColumns[4]);
                            Assert.Equal("\"7\"", logColumns[5]);
                            break;
                        case "Message8":
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message8\"", logColumns[2]);
                            Assert.Equal("\"Foo8\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo8.cs\"", logColumns[4]);
                            Assert.Equal("\"8\"", logColumns[5]);
                            break;
                        case "Message9":
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message9\"", logColumns[2]);
                            Assert.Equal("\"Foo9\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo9.cs\"", logColumns[4]);
                            Assert.Equal("\"9\"", logColumns[5]);
                            break;
                        case "Message10":
                            Assert.Equal("\"Info\"", logColumns[1]);
                            Assert.Equal("\"Message10\"", logColumns[2]);
                            Assert.Equal("\"Foo10\"", logColumns[3]);
                            Assert.Equal("\"~/Documents/Foo10.cs\"", logColumns[4]);
                            Assert.Equal("\"10\"", logColumns[5]);
                            break;
                        case "message":
                            throw new XunitException($"The header is output on a line below the first line. LogRowCount: {lineNumber}");
                        default:
                            throw new XunitException($"Unexpected message. log_level: {logColumns[1]}, message: {logColumns[2]}, method: {logColumns[3]}, file_path: {logColumns[4]}, line_number: {logColumns[5]}, LogRowCount: {lineNumber}");
                    }
                }
            }

            Assert.Equal(11, lineNumber);
            Mock.Get(mockIEssentialsService).Verify(s => s.Platform, Times.Exactly(10));
            Mock.Get(mockIEssentialsService).Verify(s => s.PlatformVersion, Times.Exactly(10));
            Mock.Get(mockIEssentialsService).Verify(s => s.Model, Times.Exactly(10));
            Mock.Get(mockIEssentialsService).Verify(s => s.DeviceType, Times.Exactly(10));
            Mock.Get(mockIEssentialsService).Verify(s => s.AppVersion, Times.Exactly(10));
            Mock.Get(mockIEssentialsService).Verify(s => s.BuildNumber, Times.Exactly(10));
        }

        #endregion

        #region Warning()

        [Fact]
        public void Warning_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            DeleteLogsDirIfExists();

            loggerService.Warning("Message", "Foo", "~/Documents/Foo.cs", 1);

            using (var sr = new StreamReader("~/.cocoa/logs/cocoa_log_20201101.csv"))
            {
                for (var i = 0; i < 2; i++)
                {
                    var logColumns = ParseLogRow(sr.ReadLine());

                    var isHeader = (i == 0);
                    if (isHeader)
                    {
                        continue;
                    }

                    Assert.Equal("\"Warning\"", logColumns[1]);
                    Assert.Equal("\"Message\"", logColumns[2]);
                    Assert.Equal("\"Foo\"", logColumns[3]);
                    Assert.Equal("\"~/Documents/Foo.cs\"", logColumns[4]);
                    Assert.Equal("\"1\"", logColumns[5]);
                }
            }

            Mock.Get(mockIEssentialsService).Verify(s => s.Platform, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.PlatformVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.Model, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.DeviceType, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.AppVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.BuildNumber, Times.Once());
        }

        #endregion

        #region Error()

        [Fact]
        public void Error_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            DeleteLogsDirIfExists();

            loggerService.Error("Message", "Foo", "~/Documents/Foo.cs", 1);

            using (var sr = new StreamReader("~/.cocoa/logs/cocoa_log_20201101.csv"))
            {
                for (var i = 0; i < 2; i++)
                {
                    var logColumns = ParseLogRow(sr.ReadLine());

                    var isHeader = (i == 0);
                    if (isHeader)
                    {
                        continue;
                    }

                    Assert.Equal("\"Error\"", logColumns[1]);
                    Assert.Equal("\"Message\"", logColumns[2]);
                    Assert.Equal("\"Foo\"", logColumns[3]);
                    Assert.Equal("\"~/Documents/Foo.cs\"", logColumns[4]);
                    Assert.Equal("\"1\"", logColumns[5]);
                }
            }

            Mock.Get(mockIEssentialsService).Verify(s => s.Platform, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.PlatformVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.Model, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.DeviceType, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.AppVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.BuildNumber, Times.Once());
        }

        #endregion

        #region Exception()

        [Fact]
        public void Exception_Success()
        {
            var mockILogPathService = CreateDefaultMockILogPathService();
            var mockIEssentialsService = CreateDefaultMockIEssentialsService();
            var loggerService = CreateDefaultLoggerService(mockILogPathService, mockIEssentialsService);
            DeleteLogsDirIfExists();

            loggerService.Exception("Message", new NullReferenceException(), "Foo", "~/Documents/Foo.cs", 1);

            using (var sr = new StreamReader("~/.cocoa/logs/cocoa_log_20201101.csv"))
            {
                for (var i = 0; i < 2; i++)
                {
                    var logColumns = ParseLogRow(sr.ReadLine());

                    var isHeader = (i == 0);
                    if (isHeader)
                    {
                        continue;
                    }

                    Assert.Equal("\"Error\"", logColumns[1]);
                    Assert.Equal("\"Message, Exception: System.NullReferenceException: Object reference not set to an instance of an object.\"", logColumns[2]);
                    Assert.Equal("\"Foo\"", logColumns[3]);
                    Assert.Equal("\"~/Documents/Foo.cs\"", logColumns[4]);
                    Assert.Equal("\"1\"", logColumns[5]);
                }
            }

            Mock.Get(mockIEssentialsService).Verify(s => s.Platform, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.PlatformVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.Model, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.DeviceType, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.AppVersion, Times.Once());
            Mock.Get(mockIEssentialsService).Verify(s => s.BuildNumber, Times.Once());
        }

        #endregion

        #endregion

        #region Other Private Methods

        private LoggerService CreateDefaultLoggerService(ILogPathService logPathService, IEssentialsService essentialsService)
        {
            return new LoggerService(logPathService, essentialsService);
        }

        private ILogPathService CreateDefaultMockILogPathService()
        {
            var mock = Mock.Of<ILogPathService>(s =>
            s.LogsDirPath == "~/.cocoa/logs" &&
            s.LogFileWildcardName == "cocoa_log_*.csv" &&
            s.LogFilePath(It.IsAny<DateTime>()) == "~/.cocoa/logs/cocoa_log_20201101.csv" &&
            s.LogUploadingTmpPath == "~/.cocoa/tmp" &&
            s.LogUploadingPublicPath == "~/.cocoa/public" &&
            s.LogUploadingFileWildcardName == "cocoa_log_*.zip"
            );

            return mock;
        }

        private IEssentialsService CreateDefaultMockIEssentialsService()
        {
            var mock = Mock.Of<IEssentialsService>(s =>
            s.Platform == "iOS" &&
            s.PlatformVersion == "14.0" &&
            s.Model == "x86_64" &&
            s.DeviceType == "Virtual" &&
            s.AppVersion == "1.0.0" &&
            s.BuildNumber == "1.0"
            );

            return mock;
        }

        private void RecreateLogsDir()
        {
            DeleteLogsDirIfExists();
            Directory.CreateDirectory("~/.cocoa/logs");
        }

        private void DeleteLogsDirIfExists()
        {
            var logsDirPath = "~/.cocoa/logs";
            if (Directory.Exists(logsDirPath))
            {
                Directory.Delete(logsDirPath, true);
            }
        }

        private void RecreateLogsFile()
        {
            var logFilePath = "~/.cocoa/logs/cocoa_log_20201101.csv";
            if (File.Exists(logFilePath))
            {
                File.Delete(logFilePath);
            }
            File.Create(logFilePath).Close();
        }

        private string[] ParseLogRow(string logRow)
        {
            var pattern = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            return pattern.Split(logRow);
        }

        #endregion
    }
}
