/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services.Migration
{
    internal class Initializer_1_0_0
    {
        private const string APPLICATION_PROPERTY_USER_DATA_KEY = "UserData";

        private readonly IApplicationPropertyService _applicationPropertyService;
        private readonly ILoggerService _loggerService;

        public Initializer_1_0_0(
            IApplicationPropertyService applicationPropertyService,
            ILoggerService loggerService
            )
        {
            _applicationPropertyService = applicationPropertyService;
            _loggerService = loggerService;
        }

        public async Task ExecuteAsync()
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
