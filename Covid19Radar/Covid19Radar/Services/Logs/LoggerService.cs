using System;
using System.Runtime.CompilerServices;

namespace Covid19Radar.Services.Logs
{
	public enum LogLevel
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

        private readonly ILogWriter _writer;

        #endregion

        #region Constructors

        public LoggerService(ILogWriter logWriter)
        {
            _writer = logWriter ?? throw new ArgumentNullException(nameof(logWriter));
        }

        #endregion

        #region ILoggerService Methods

        public void StartMethod(
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _writer.Write("Start", method, filePath, lineNumber, LogLevel.Info);
        }

        public void EndMethod(
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _writer.Write("End", method, filePath, lineNumber, LogLevel.Info);
        }

        public void Verbose(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _writer.Write(message, method, filePath, lineNumber, LogLevel.Verbose);
        }

        public void Debug(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _writer.Write(message, method, filePath, lineNumber, LogLevel.Debug);
        }

        public void Info(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _writer.Write(message, method, filePath, lineNumber, LogLevel.Info);
        }

        public void Warning(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _writer.Write(message, method, filePath, lineNumber, LogLevel.Warning);
        }

        public void Error(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _writer.Write(message, method, filePath, lineNumber, LogLevel.Error);
        }

        public void Exception(
            string message,
            Exception ex,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            _writer.Write(message + ", Exception: " + ex.ToString(), method, filePath, lineNumber, LogLevel.Error);
        }

        #endregion
    }
}
