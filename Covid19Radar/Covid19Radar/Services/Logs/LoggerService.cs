using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Covid19Radar.Common;

namespace Covid19Radar.Services.Logs
{
    enum LogLevel
    {
        Verbose,
        Debug,
        Info,
        Warning,
        Error
    }

    public class LoggerService : ILoggerService
    {
        #region Instance Fields

        private readonly ILogPathService logPathService;
        private readonly IEssentialsService essentialsService;

        private readonly object lockObject = new object();

        #endregion

        #region Constructors

        public LoggerService(ILogPathService logPathService, IEssentialsService essentialsService)
        {
            this.logPathService = logPathService;
            this.essentialsService = essentialsService;

#if TEST_BACKTASK
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                // Android の場合は決め打ちにする
                var packagename = "net.moonmile.sample.opencacao.backtask";
                dir = $"/storage/emulated/0/Android/data/{packagename}/files";
            }
            var filename = Path.Combine(dir, $"trace-log-{DateTime.Now.ToString("yyyyMMdd-HHmm")}.txt");
            var tw = System.IO.File.OpenWrite(filename);
            var tr1 = new System.Diagnostics.TextWriterTraceListener(tw);
            Trace.Listeners.Add(tr1);
            Trace.AutoFlush = true;
            Trace.WriteLine("START: " + DateTime.Now.ToString());
#endif
        }

        #endregion

        #region ILoggerService Methods

        public void StartMethod(
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Output("Start", method, filePath, lineNumber, LogLevel.Info);
        }

        public void EndMethod(
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Output("End", method, filePath, lineNumber, LogLevel.Info);
        }

        public void Verbose(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Output(message, method, filePath, lineNumber, LogLevel.Verbose);
        }

        public void Debug(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Output(message, method, filePath, lineNumber, LogLevel.Debug);
        }

        public void Info(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Output(message, method, filePath, lineNumber, LogLevel.Info);
        }

        public void Warning(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Output(message, method, filePath, lineNumber, LogLevel.Warning);
        }

        public void Error(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Output(message, method, filePath, lineNumber, LogLevel.Error);
        }

        public void Exception(
            string message,
            Exception ex,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Output(message + ", Exception: " + ex.ToString(), method, filePath, lineNumber, LogLevel.Error);
        }

        #endregion

        #region Other Private Methods

        private void Output(string message, string method, string filePath, int lineNumber, LogLevel logLevel)
        {
#if TEST_BACKTASK
            var line = CreateLogContentRow(message, method, filePath, lineNumber, logLevel, Utils.JstNow());
            System.Diagnostics.Trace.WriteLine(line);
            return;
# endif

#if !DEBUG
            if (logLevel == LogLevel.Verbose || logLevel == LogLevel.Debug)
            {
                return;
            }
#endif
            try
            {
                lock (lockObject)
                {
                    CreateLogsDirIfNotExists();

                    var jstNow = Utils.JstNow();
                    var logFilePath = logPathService.LogFilePath(jstNow);

                    CreateLogFileIfNotExists(logFilePath);

                    var row = CreateLogContentRow(message, method, filePath, lineNumber, logLevel, jstNow);
                    System.Diagnostics.Debug.WriteLine(row);
                    using (var sw = new StreamWriter(logFilePath, true, Encoding.UTF8))
                    {
                        sw.WriteLine(row);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void CreateLogsDirIfNotExists()
        {
            if (Directory.Exists(logPathService.LogsDirPath))
            {
                return;
            }
            Directory.CreateDirectory(logPathService.LogsDirPath);
        }

        private void CreateLogFileIfNotExists(string logFilePath)
        {
            if (File.Exists(logFilePath))
            {
                return;
            }
            File.Create(logFilePath).Close();

            using (var sw = new StreamWriter(logFilePath, true, Encoding.UTF8))
            {
                sw.WriteLine(CreateLogHeaderRow());
            }
        }

        private string CreateLogHeaderRow()
        {
            var columnNames = new List<string>
            {
                "output_date",
                "log_level",
                "message",
                "method",
                "file_path",
                "line_number",
                "platform",
                "platform_version",
                "model",
                "device_type",
                "app_version",
                "build_number"
            };

            return CreateLogRow(columnNames);
        }

        private string CreateLogContentRow(string message, string method, string filePath, int lineNumber, LogLevel logLevel, DateTime jstDateTime)
        {
            var columns = new List<string>
            {
                jstDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                logLevel.ToString(),
                message,
                method,
                filePath,
                lineNumber.ToString(),
                essentialsService.Platform,
                essentialsService.PlatformVersion,
                essentialsService.Model,
                essentialsService.DeviceType,
                essentialsService.AppVersion,
                essentialsService.BuildNumber
            };

            return CreateLogRow(columns);
        }

        private string CreateLogRow(List<string> columns)
        {
            var convertedColumns = columns
                .Select(column => column ?? string.Empty)
                .Select(column => column.Replace("\r", "").Replace("\n", ""))
                .Select(column => column.Replace("\"", "\"\""))
                .Select(column => "\"" + column + "\"");

            return string.Join(",", convertedColumns);
        }

        #endregion
    }
}
