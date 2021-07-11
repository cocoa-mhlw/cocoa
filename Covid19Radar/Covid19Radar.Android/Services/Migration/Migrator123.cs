/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using AndroidX.Work;
using Covid19Radar.Services.Logs;
using Covid19Radar.Services.Migration;

namespace Covid19Radar.Droid.Services.Migration
{
    public class Migrator123: IVersionMigrationService
    {
        private readonly ILoggerService _loggerService;

        public Migrator123(
            ILoggerService loggerService
            )
        {
            _loggerService = loggerService;
        }

        public override Task MigrateAsync()
        {
            _loggerService.StartMethod();

            // Array of old work-name.
            string[] oldWorkName = {
                "exposurenotification",
            };

            var workManager = WorkManager.GetInstance(Xamarin.Essentials.Platform.AppContext);
            CancelOldWorks(workManager, oldWorkName, _loggerService);

            Xamarin.ExposureNotifications.ExposureNotification.Init();

            _loggerService.EndMethod();

            return Task.CompletedTask;
        }

        private static void CancelOldWorks(WorkManager workManager, string[] oldWorkNames, ILoggerService loggerService)
        {
            loggerService.StartMethod();

            foreach (var oldWorkName in oldWorkNames)
            {
                workManager.CancelUniqueWork(oldWorkName);
                loggerService.Debug($"Worker {oldWorkName} is canceled.");
            }

            loggerService.EndMethod();
        }
    }
}
