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
    public class BackgroundService : AbsBackgroundService
    {
        private static string BGTASK_IDENTIFIER => AppInfo.PackageName + ".exposure-notification";

        private const int TIMEOUT_IN_MILLIS = 60 * 60 * 1000;

        private readonly AbsExposureNotificationApiService _exposureNotificationApiService;
        private readonly ILoggerService _loggerService;

        public BackgroundService(
            IDiagnosisKeyRepository diagnosisKeyRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository
            ) : base(
                diagnosisKeyRepository,
                exposureNotificationApiService,
                loggerService,
                userDataRepository
                )
        {
            _exposureNotificationApiService = exposureNotificationApiService;
            _loggerService = loggerService;
        }

        public override void ScheduleExposureDetection()
        {
            _loggerService.StartMethod();

            _ = BGTaskScheduler.Shared.Register(BGTASK_IDENTIFIER, null, task =>
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

            ScheduleBgTask();

            _loggerService.EndMethod();
        }

        private void ScheduleBgTask()
        {
            _loggerService.StartMethod();

            try
            {
                BGProcessingTaskRequest bgTaskRequest = new BGProcessingTaskRequest(BGTASK_IDENTIFIER);
                bgTaskRequest.RequiresNetworkConnectivity = true;

                BGTaskScheduler.Shared.Submit(bgTaskRequest, out var error);
                if (error != null)
                {
                    throw new NSErrorException(error);
                }
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

    }
}
