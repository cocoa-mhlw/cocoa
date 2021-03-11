using System;
using System.Runtime.CompilerServices;

namespace Covid19Radar.Services.Logs
{
#if TEST_BACKTASK
    // インターフェースを切り替えてしまう
    // これは ILoggerService を別アセンブリにしたほうが良い
    public interface ILoggerService : Xamarin.ExposureNotifications.ILoggerServiceInner { }
#else
    public interface ILoggerService
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
#endif

}
