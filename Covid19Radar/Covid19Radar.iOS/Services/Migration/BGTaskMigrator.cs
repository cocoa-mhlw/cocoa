// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System.Threading.Tasks;
using BackgroundTasks;
using Covid19Radar.Services.Logs;
using Xamarin.Essentials;

namespace Covid19Radar.iOS.Services.Migration
{
    internal class BGTaskMigrator
    {
        // Array of old identifier.
        private static readonly string[] OLD_IDENTIFIER_ARRAY = {
            AppInfo.PackageName + ".delete-old-logs",
        };

        private readonly ILoggerService _loggerService;

        public BGTaskMigrator(
            ILoggerService loggerService
            )
        {
            _loggerService = loggerService;
        }


        internal Task ExecuteAsync()
        {
            _loggerService.StartMethod();

            foreach(var identifier in OLD_IDENTIFIER_ARRAY)
            {
                BGTaskScheduler.Shared.Cancel(identifier);
                _loggerService.Info($"BGTask {identifier} has been canceled.");
            }

            _loggerService.EndMethod();

            return Task.CompletedTask;
        }
    }
}
