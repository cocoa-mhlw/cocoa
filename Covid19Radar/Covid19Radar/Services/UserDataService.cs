/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Common;
using Covid19Radar.Services.Logs;
using System;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public interface IUserDataService
    {
        Task<bool> RegisterUserAsync();

        DateTime GetStartDate();
        int GetDaysOfUse();

        void RemoveStartDate();
    }

    /// <summary>
    /// This service registers, retrieves, stores, and automatically updates user data.
    /// </summary>
    public class UserDataService : IUserDataService
    {
        private readonly ILoggerService loggerService;
        private readonly IHttpDataService httpDataService;
        private readonly IPreferencesService preferencesService;

        public UserDataService(IHttpDataService httpDataService, ILoggerService loggerService, IPreferencesService preferencesService)
        {
            this.httpDataService = httpDataService;
            this.loggerService = loggerService;
            this.preferencesService = preferencesService;
        }

        public async Task<bool> RegisterUserAsync()
        {
            loggerService.StartMethod();

            var registerResult = await httpDataService.PostRegisterUserAsync();
            if (!registerResult)
            {
                loggerService.Info("Failed register");
                loggerService.EndMethod();
                return false;
            }
            loggerService.Info("Success register");

            preferencesService.SetValue(PreferenceKey.StartDateTime, DateTime.UtcNow);

            loggerService.EndMethod();
            return true;
        }

        public DateTime GetStartDate()
        {
            return preferencesService.GetValue(PreferenceKey.StartDateTime, DateTime.UtcNow);
        }

        public int GetDaysOfUse()
        {
            TimeSpan timeSpan = DateTime.UtcNow - GetStartDate();
            return timeSpan.Days;
        }

        public void RemoveStartDate()
        {
            loggerService.StartMethod();

            preferencesService.RemoveValue(PreferenceKey.StartDateTime);

            loggerService.EndMethod();
        }
    }
}
