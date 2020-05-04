using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Tests.Mock
{
    /// <summary>
    /// Mock Logger
    /// </summary>
    public class LoggerMock : ILogger
    {
        public class ScopeMock : IDisposable
        {
            public void Dispose()
            {
            }
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return new ScopeMock();
        }

        public Func<LogLevel, bool> IsEnabledFunc = _ =>  true;
        public bool IsEnabled(LogLevel logLevel)
        {
            return IsEnabledFunc(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var exceptionMessage = (exception != null ? (formatter(state, exception) ?? exception.ToString()) : ""); 
            var message = $"Event:{eventId.Id}-{eventId.Name} State:{state} {exceptionMessage}"; 
            switch (logLevel)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    System.Diagnostics.Trace.TraceError(message);
                    break;
                case LogLevel.Warning:
                    System.Diagnostics.Trace.TraceWarning(message);
                    break;
                case LogLevel.Information:
                case LogLevel.Trace:
                    System.Diagnostics.Trace.TraceInformation(message);
                    break;
                case LogLevel.Debug:
                case LogLevel.None:
                    System.Diagnostics.Trace.WriteLine(message);
                    break;
            }
        }
    }

    public class LoggerMock<T> : LoggerMock, ILogger<T>
    {

    }

}
