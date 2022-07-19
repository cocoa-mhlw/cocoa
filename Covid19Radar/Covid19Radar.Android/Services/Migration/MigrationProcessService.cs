/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Android.App;
using Android.Content;
using AndroidX.Work;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Services.Migration;
using Prism.Ioc;
using Xamarin.Forms.Internals;

namespace Covid19Radar.Droid.Services.Migration
{
    [Preserve]
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionMyPackageReplaced })]
    public class AppVersionUpgradeReceiver : BroadcastReceiver
    {
        private readonly ILoggerService _loggerService = ContainerLocator.Current.Resolve<ILoggerService>();

        public override void OnReceive(Context context, Intent intent)
        {
            _loggerService.StartMethod();

            if (intent.Action != Intent.ActionMyPackageReplaced)
            {
                _loggerService.EndMethod();
                return;
            }

            WorkManager workManager = WorkManager.GetInstance(context);
            var worker = new OneTimeWorkRequest.Builder(
                            Java.Lang.Class.FromType(typeof(UpgradeWorker))
                            ).Build();
            _ = workManager.Enqueue(worker);

            _loggerService.EndMethod();
        }
    }

    public class UpgradeWorker : Worker
    {
        private readonly ILoggerService _loggerService = ContainerLocator.Current.Resolve<ILoggerService>();
        private readonly IMigrationService _migrationService = ContainerLocator.Current.Resolve<IMigrationService>();

        public UpgradeWorker(
            Context context,
            WorkerParameters workerParams
            ) : base(context, workerParams)
        {
            // do nothing
        }

        public override Result DoWork()
        {
            _loggerService.StartMethod();

            Task.Run(() => _migrationService.MigrateAsync()).GetAwaiter().GetResult();

            _loggerService.EndMethod();

            return Result.InvokeSuccess();
        }
    }

    public class MigrationProccessService : IMigrationProcessService
    {
        private readonly AbsExposureDetectionBackgroundService _exposureDetectionBackgroundService;
        private readonly AbsDataMaintainanceBackgroundService _dataMaintainanceBackgroundService;
        private readonly AbsEventLogSubmissionBackgroundService _eventLogSubmissionBackgroundService;

        private readonly ILoggerService _loggerService;

        public MigrationProccessService(
            AbsExposureDetectionBackgroundService exposureDetectionBackgroundService,
            AbsDataMaintainanceBackgroundService dataMaintainanceBackgroundService,
            AbsEventLogSubmissionBackgroundService eventLogSubmissionBackgroundService,
            ILoggerService loggerService
            )
        {
            _exposureDetectionBackgroundService = exposureDetectionBackgroundService;
            _dataMaintainanceBackgroundService = dataMaintainanceBackgroundService;
            _eventLogSubmissionBackgroundService = eventLogSubmissionBackgroundService;
            _loggerService = loggerService;
        }

        public async Task SetupAsync()
        {
            _loggerService.StartMethod();

            await new WorkManagerMigrator(
                _exposureDetectionBackgroundService,
                _dataMaintainanceBackgroundService,
                _eventLogSubmissionBackgroundService,
                _loggerService
                ).ExecuteAsync();

            _loggerService.EndMethod();
        }
    }
}
