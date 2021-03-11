using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Xamarin.ExposureNotifications
{
    /// <summary>
    /// Xamarin.ExposureNotification 内で利用する LoggerService
    /// </summary>
    public class LoggerService
    {
        private static ILoggerServiceInner _target;
        public static ILoggerServiceInner Instance
        {
            get => _target;
            set => _target = value;
        }
        public static void StartMethod(
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        { _target?.StartMethod(method, filePath, lineNumber); }

        public static void EndMethod(
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        { _target?.StartMethod(method, filePath, lineNumber); }
        public static void Verbose(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        { _target?.StartMethod(method, filePath, lineNumber); }
        public static void Debug(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        { _target?.StartMethod(method, filePath, lineNumber); }
        public static void Info(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        { _target?.StartMethod(method, filePath, lineNumber); }
        public static void Warning(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        { _target?.StartMethod(method, filePath, lineNumber); }
        public static void Error(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        { _target?.StartMethod(method, filePath, lineNumber); }
        public static void Exception(
            string message,
            Exception ex,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        { _target?.StartMethod(method, filePath, lineNumber); }
    }

    public interface ILoggerServiceInner
    {
        void StartMethod(
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void EndMethod(
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Verbose(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Debug(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Info(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Warning(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Error(
            string message,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
        void Exception(
            string message,
            Exception ex,
            [CallerMemberName] string method = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0);
    }
}
