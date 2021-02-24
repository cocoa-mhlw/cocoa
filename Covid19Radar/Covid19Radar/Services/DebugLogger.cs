using FFImageLoading.Helpers;
using Prism.Logging;
using System;
using System.Collections.Generic;
using static System.Diagnostics.Debug;

namespace Covid19Radar.Services
{
    public class DebugLogger : ConsoleLoggingService , IMiniLogger
    {
        public void Debug(string message) =>
            WriteLine($"Debug: {message}");

        public void Error(string errorMessage) =>
            WriteLine($"Error: {errorMessage}");

        public void Error(string errorMessage, Exception ex) =>
            WriteLine($"Error: {errorMessage}\n{ex.GetType().Name}: {ex}");

        // Prism 8
        // BREAKING Removed ILoggerFacade and entire Prism.Logging namespace - recommended migration use Prism.Plugin.Logging or other 3rd party logging frameworks
        // Recoomend solutoin Prism.Plugin.Logging
        // https://github.com/dansiegel/Prism.Plugin.Logging/blob/d54217be59973f1ea7021c16014ca9aed265ae77/src/Prism.Plugin.Logging.Abstractions/ConsoleLoggingService.cs
        /*
        public void Log(string message, Category category, Priority priority) =>
            WriteLine($"{category} - {priority}: {message}");
        */
    }
}
