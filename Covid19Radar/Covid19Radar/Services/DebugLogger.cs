using FFImageLoading.Helpers;
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Diagnostics.Debug;

namespace Covid19Radar.Services
{
    public class DebugLogger : ILoggerFacade, IMiniLogger
    {
        public void Debug(string message) =>
            WriteLine($"Debug: {message}");

        public void Error(string errorMessage) =>
            WriteLine($"Error: {errorMessage}");

        public void Error(string errorMessage, Exception ex) =>
            WriteLine($"Error: {errorMessage}\n{ex.GetType().Name}: {ex}");

        public void Log(string message, Category category, Priority priority) =>
            WriteLine($"{category} - {priority}: {message}");
    }
}
