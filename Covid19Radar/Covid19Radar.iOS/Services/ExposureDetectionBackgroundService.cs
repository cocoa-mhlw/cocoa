using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackgroundTasks;
using Chino;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Foundation;
using Xamarin.Essentials;

namespace Covid19Radar.iOS.Services
{
    public class ExposureDetectionBackgroundService : AbsExposureDetectionBackgroundService
    {
        private static string BGTASK_IDENTIFIER => AppInfo.PackageName + ".exposure-detection";

        private const int TIMEOUT_IN_MILLIS = 60 * 60 * 1000;

        private readonly AbsExposureNotificationApiService _exposureNotificationApiService;
        private readonly ILoggerService _loggerService;


        public ExposureDetectionBackgroundService(
            IDiagnosisKeyRepository diagnosisKeyRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository
            ) : base(
                diagnosisKeyRepository,
                exposureNotificationApiService,
                exposureConfigurationRepository,
                loggerService,
                userDataRepository
                )
        {
            _exposureNotificationApiService = exposureNotificationApiService;
            _loggerService = loggerService;
        }

        public override void Schedule()
        {
            _loggerService.StartMethod();

            _loggerService.Debug($"BGTASK_IDENTIFIER: {BGTASK_IDENTIFIER}");

            var result = BGTaskScheduler.Shared.Register(BGTASK_IDENTIFIER, null, task =>
            {
                _loggerService.Info("Background task has been started.");

                ScheduleBgTask();

                var cancellationTokenSource = new CancellationTokenSource(TIMEOUT_IN_MILLIS);
                task.ExpirationHandler = cancellationTokenSource.Cancel;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        IList<ExposureNotificationStatus> statuses = _exposureNotificationApiService.GetStatusesAsync()
                            .GetAwaiter().GetResult();

                        bool isUnauthorized = statuses
                            .Where(status => status.Code == ExposureNotificationStatus.Code_iOS.Unauthorized)
                            .Count() != 0;
                        if (isUnauthorized)
                        {
                            _loggerService.Error("Exposure notofication is not authorized.");
                            task.SetTaskCompleted(true);
                            return;
                        }

                        await ExposureDetectionAsync();
                        task.SetTaskCompleted(true);
                    }
                    catch (OperationCanceledException exception)
                    {
                        _loggerService.Exception($"Background task canceled.", exception);
                        task.SetTaskCompleted(false);
                    }
                    catch (Exception exception)
                    {
                        _loggerService.Exception($"Exception", exception);
                        task.SetTaskCompleted(false);
                    }
                });
            });

            if (result)
            {
                _loggerService.Debug("BGTaskScheduler.Shared.Register succeeded.");
            }
            else
            {
                _loggerService.Info("BGTaskScheduler.Shared.Register failed.");
            }

            ScheduleBgTask();

            _loggerService.EndMethod();
        }

        private void ScheduleBgTask()
        {
            _loggerService.StartMethod();

            try
            {
                BGProcessingTaskRequest bgTaskRequest = new BGProcessingTaskRequest(BGTASK_IDENTIFIER)
                {
                    RequiresNetworkConnectivity = true
                };

                BGTaskScheduler.Shared.Submit(bgTaskRequest, out var error);
                if (error != null)
                {
                    NSErrorException exception = new NSErrorException(error);
                    _loggerService.Exception("BGTaskScheduler submit failed.", exception);
                    throw exception;
                }
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

    }
}
