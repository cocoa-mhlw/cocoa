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

        }
    }

    public class LoggerMock<T> : LoggerMock, ILogger<T>
    {

    }

}
