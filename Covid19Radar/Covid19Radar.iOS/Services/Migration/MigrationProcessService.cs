/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Covid19Radar.Services.Logs;
using Covid19Radar.Services.Migration;

namespace Covid19Radar.iOS.Services.Migration
{
    public class MigrationProcessService : IMigrationProcessService
    {
        private readonly ILoggerService _loggerService;

        public MigrationProcessService(
            ILoggerService loggerService
        )
        {
            _loggerService = loggerService;
        }

        public async Task SetupAsync()
        {
            _loggerService.StartMethod();

            await new BGTaskMigrator(
                _loggerService
                ).ExecuteAsync();

            _loggerService.EndMethod();
        }
    }
}
