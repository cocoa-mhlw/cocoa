// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Repository;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public abstract class AbsDataMaintainanceBackgroundService : IBackgroundService
    {
        protected ILoggerService LoggerService { get; private set; }

        private readonly IEventLogRepository _eventLogRepository;

        public AbsDataMaintainanceBackgroundService(
            ILoggerService loggerService,
            IEventLogRepository eventLogRepository)
        {
            LoggerService = loggerService;
            _eventLogRepository = eventLogRepository;
        }

        public abstract void Schedule();

        public async Task ExecuteAsync()
        {
            LoggerService.StartMethod();

            try
            {
                await _eventLogRepository.RotateAsync(
                    AppConstants.EventLogFileExpiredSeconds);
            }
            finally
            {
                LoggerService.EndMethod();
            }
        }
    }
}

