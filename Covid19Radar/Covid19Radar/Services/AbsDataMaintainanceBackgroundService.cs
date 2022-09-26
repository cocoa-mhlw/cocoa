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

        private readonly ILogFileService _logFileService;

        protected AbsDataMaintainanceBackgroundService(
            ILoggerService loggerService,
            ILogFileService logFileService)
        {
            LoggerService = loggerService;
            _logFileService = logFileService;
        }

        public abstract void Schedule();
        public abstract void Cancel();

        public void Execute()
        {
            LoggerService.StartMethod();

            try
            {
                _logFileService.Rotate();
            }
            finally
            {
                LoggerService.EndMethod();
            }
        }
    }
}

