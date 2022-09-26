/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading;
using System.Threading.Tasks;
using BackgroundTasks;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Foundation;
using Xamarin.Essentials;

namespace Covid19Radar.iOS.Services
{
    public class ExposureDetectionBackgroundService : AbsExposureDetectionBackgroundService
    {
        /*
         * [IMPORTANT]
         * From README.md( https://developer.apple.com/documentation/exposurenotification/building_an_app_to_notify_users_of_covid-19_exposure )
         *
         * The Background Task framework automatically detects apps that contain the Exposure Notification entitlement and a background task that ends in `exposure-notification`.
         * The operating system automatically launches these apps when they aren't running and guarantees them more background time to ensure that the app can test and report results promptly.
         */
        private static string BGTASK_IDENTIFIER => AppInfo.PackageName + ".exposure-notification";

        private readonly ILoggerService _loggerService;

        public ExposureDetectionBackgroundService(
            IDiagnosisKeyRepository diagnosisKeyRepository,
            AbsExposureNotificationApiService exposureNotificationApiService,
            IExposureConfigurationRepository exposureConfigurationRepository,
            ILoggerService loggerService,
            IUserDataRepository userDataRepository,
            IServerConfigurationRepository serverConfigurationRepository,
            ILocalPathService localPathService,
            IDateTimeUtility dateTimeUtility,
            ILocalNotificationService localNotificationService
            ) : base(
                diagnosisKeyRepository,
                exposureNotificationApiService,
                exposureConfigurationRepository,
                loggerService,
                userDataRepository,
                serverConfigurationRepository,
                localPathService,
                dateTimeUtility,
                localNotificationService
                )
        {
            _loggerService = loggerService;
        }

        public override void Schedule()
        {
            _loggerService.StartMethod();

            var result = BGTaskScheduler.Shared.Register(BGTASK_IDENTIFIER, null, task =>
            {
                _loggerService.Info("Background task has been started.");

                ScheduleBgTask();

                var cancellationTokenSource = new CancellationTokenSource();
                task.ExpirationHandler = cancellationTokenSource.Cancel;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await ShowEndOfServiceNotificationAync();
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
                    finally
                    {
                        cancellationTokenSource.Dispose();
                    }
                }, cancellationTokenSource.Token);
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

        public override void Cancel()
        {
            BGTaskScheduler.Shared.Cancel(BGTASK_IDENTIFIER);
        }
    }
}
