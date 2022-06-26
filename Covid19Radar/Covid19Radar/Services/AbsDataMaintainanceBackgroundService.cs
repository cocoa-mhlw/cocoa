// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using System.Threading.Tasks;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public abstract class AbsDataMaintainanceBackgroundService : IBackgroundService
    {
        protected readonly ILoggerService loggerService;

        private readonly ILogFileService _logFileService;

        public AbsDataMaintainanceBackgroundService(
            ILogFileService logFileService,
            ILoggerService loggerService
            )
        {
            _logFileService = logFileService;
            this.loggerService = loggerService;
        }

        public abstract void Schedule();

        public async Task ExecuteAsync()
        {
            _logFileService.Rotate();
        }
    }
}
