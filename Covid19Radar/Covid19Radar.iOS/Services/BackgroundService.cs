using System;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.iOS.Services
{
    public class BackgroundService : AbsBackgroundService
    {
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
            _loggerService = loggerService;
        }

        public override void ScheduleExposureDetection()
        {
            throw new NotImplementedException();
        }
    }
}
