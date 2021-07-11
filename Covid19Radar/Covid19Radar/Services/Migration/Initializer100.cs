/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services.Migration
{
    public class Initializer100 : IVersionMigrationService
    {
        private readonly IApplicationPropertyService _applicationPropertyService;
        private readonly ILoggerService _loggerService;

        public Initializer100(
            IApplicationPropertyService applicationPropertyService,
            ILoggerService loggerService
            )
        {
            _applicationPropertyService = applicationPropertyService;
            _loggerService = loggerService;
        }

        const string APPLICATION_PROPERTY_USER_DATA_KEY = "UserData";

        public override async Task MigrateAsync()
        {
            _loggerService.StartMethod();

            var existsUserData = _applicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY);
            if (existsUserData)
            {
                _loggerService.EndMethod();
                return;
            }

            var userData = new UserDataModel();
            await _applicationPropertyService.SavePropertiesAsync(APPLICATION_PROPERTY_USER_DATA_KEY, JsonConvert.SerializeObject(userData));

            _loggerService.EndMethod();
        }

    }
}
