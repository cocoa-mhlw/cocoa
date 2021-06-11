/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services
{
    public interface IUserDataService
    {
        Task Migrate();

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
        private readonly ITermsUpdateService termsUpdateService;
        private readonly IExposureNotificationService exposureNotificationService;
        private readonly IApplicationPropertyService applicationPropertyService;

        public UserDataService(IHttpDataService httpDataService, ILoggerService loggerService, IPreferencesService preferencesService, ITermsUpdateService termsUpdateService, IExposureNotificationService exposureNotificationService, IApplicationPropertyService applicationPropertyService)
        {
            this.httpDataService = httpDataService;
            this.loggerService = loggerService;
            this.preferencesService = preferencesService;
            this.termsUpdateService = termsUpdateService;
            this.exposureNotificationService = exposureNotificationService;
            this.applicationPropertyService = applicationPropertyService;
        }

        private readonly SemaphoreSlim _semaphoreForMigrage = new SemaphoreSlim(1, 1);

        public async Task Migrate()
        {
            await _semaphoreForMigrage.WaitAsync();
            loggerService.StartMethod();
            try
            {
                var userData = GetFromApplicationProperties();
                if (userData == null)
                {
                    return;
                }

                if (userData.StartDateTime != null && !userData.StartDateTime.Equals(new DateTime()))
                {
                    preferencesService.SetValue(PreferenceKey.StartDateTime, userData.StartDateTime);
                    userData.StartDateTime = new DateTime();
                    loggerService.Info("Migrated StartDateTime");
                }

                if (userData.IsOptined)
                {
                    await termsUpdateService.Migrate(TermsType.TermsOfService, userData.IsOptined);
                    userData.IsOptined = false;
                }
                if (userData.IsPolicyAccepted)
                {
                    await termsUpdateService.Migrate(TermsType.PrivacyPolicy, userData.IsPolicyAccepted);
                    userData.IsPolicyAccepted = false;
                }

                await exposureNotificationService.MigrateFromUserData(userData);

                await applicationPropertyService.Remove("UserData");
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed migrate", ex);
            }
            finally
            {
                _semaphoreForMigrage.Release();
                loggerService.EndMethod();
            }
        }

        private UserDataModel GetFromApplicationProperties()
        {
            loggerService.StartMethod();

            var existsUserData = applicationPropertyService.ContainsKey("UserData");
            loggerService.Info($"existsUserData: {existsUserData}");
            if (existsUserData)
            {
                loggerService.EndMethod();
                var userData = applicationPropertyService.GetProperties("UserData");
                return JsonConvert.DeserializeObject<UserDataModel>(userData.ToString());
            }

            loggerService.EndMethod();
            return null;
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
