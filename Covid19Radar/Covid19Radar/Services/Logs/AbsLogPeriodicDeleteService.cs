// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public abstract class AbsLogPeriodicDeleteService : IBackgroundService
    {
        protected readonly ILoggerService loggerService;

        public AbsLogPeriodicDeleteService(
            ILoggerService loggerService
            )
        {
            this.loggerService = loggerService;
        }

        public abstract void Schedule();
    }
}
