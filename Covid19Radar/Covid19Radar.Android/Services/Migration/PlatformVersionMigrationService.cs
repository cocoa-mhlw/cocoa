/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Android.App;
using Android.Content;
using CommonServiceLocator;
using Covid19Radar.Services.Logs;
using Covid19Radar.Services.Migration;
using Xamarin.Forms.Internals;

namespace Covid19Radar.Droid.Services.Migration
{
    [Preserve]
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionMyPackageReplaced })]
    public class AppVersionUpgradeReceiver : BroadcastReceiver
    {
        private readonly ILoggerService _loggerService = ServiceLocator.Current.GetInstance<ILoggerService>();
        private readonly IVersionMigrationService _versionMigrationService = ServiceLocator.Current.GetInstance<IVersionMigrationService>();

        public override void OnReceive(Context context, Intent intent)
        {
            _loggerService.StartMethod();

            if (intent.Action != Intent.ActionMyPackageReplaced)
            {
                return;
            }

            _versionMigrationService.MigrateAsync();

            _loggerService.EndMethod();
        }
    }

    public class PlatformVersionMigrationService : IVersionMigration
    {
        private readonly ILoggerService _loggerService;

        public PlatformVersionMigrationService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public async Task MigrateTo123Async()
        {
            _loggerService.StartMethod();

            await new Migrator123(
                _loggerService
                ).MigrateAsync();

            _loggerService.EndMethod();
        }

    }
}
