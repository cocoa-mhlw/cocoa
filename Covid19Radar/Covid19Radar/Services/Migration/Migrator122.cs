/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services.Migration
{
    public class Migrator122 : IVersionMigrationService
    {
        private readonly IApplicationPropertyService _applicationPropertyService;
        private readonly IPreferencesService _preferencesService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly ILoggerService _loggerService;

        public Migrator122(
            IApplicationPropertyService applicationPropertyService,
            IPreferencesService preferencesService,
            ISecureStorageService secureStorageService,
            ILoggerService loggerService
            )
        {
            _applicationPropertyService = applicationPropertyService;
            _preferencesService = preferencesService;
            _secureStorageService = secureStorageService;
            _loggerService = loggerService;
        }

        public override async Task MigrateAsync()
        {
            _loggerService.StartMethod();

            var userData = GetUserDataFromApplicationProperties();

            if (userData != null)
            {
                await MigrateFromApplicationUserDataToPlatformPreferenceAsync(userData);
                await _applicationPropertyService.Remove(APPLICATION_PROPERTY_USER_DATA_KEY);
            }

            _loggerService.EndMethod();
        }

        const string APPLICATION_PROPERTY_USER_DATA_KEY = "UserData";

        private UserDataModel GetUserDataFromApplicationProperties()
        {
            _loggerService.StartMethod();

            var existsUserData = _applicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY);
            if (existsUserData)
            {
                _loggerService.EndMethod();

                var userData = _applicationPropertyService.GetProperties(APPLICATION_PROPERTY_USER_DATA_KEY);
                return Utils.DeserializeFromJson<UserDataModel>(userData.ToString());
            }

            _loggerService.EndMethod();
            return null;
        }

        private async Task MigrateTermAsync(TermsType termsType, bool isAgree)
        {
            const string TermsOfServiceLastUpdateDateKey = "TermsOfServiceLastUpdateDateTime";
            const string PrivacyPolicyLastUpdateDateKey = "PrivacyPolicyLastUpdateDateTime";

            _loggerService.StartMethod();

            var applicationPropertyKey = termsType == TermsType.TermsOfService ? TermsOfServiceLastUpdateDateKey : PrivacyPolicyLastUpdateDateKey;
            var preferenceKey = termsType == TermsType.TermsOfService ? PreferenceKey.TermsOfServiceLastUpdateDateTime : PreferenceKey.PrivacyPolicyLastUpdateDateTime;

            if (_preferencesService.ContainsKey(applicationPropertyKey))
            {
                _loggerService.EndMethod();
                return;
            }

            if (isAgree)
            {
                if (_applicationPropertyService.ContainsKey(applicationPropertyKey))
                {
                    var lastUpdateDate = (DateTime)_applicationPropertyService.GetProperties(applicationPropertyKey);
                    _preferencesService.SetValue(preferenceKey, lastUpdateDate);
                }
                else
                {
                    _preferencesService.SetValue(preferenceKey, new DateTime());
                    _loggerService.Info($"Migrated {applicationPropertyKey}");
                }
            }

            await _applicationPropertyService.Remove(applicationPropertyKey);

            _loggerService.EndMethod();
        }

        private async Task MigrateFromApplicationUserDataToPlatformPreferenceAsync(UserDataModel userData)
        {
            _loggerService.StartMethod();

            await MigrateTermAsync(TermsType.TermsOfService, userData.IsOptined);
            await MigrateTermAsync(TermsType.PrivacyPolicy, userData.IsPolicyAccepted);

            if (userData.StartDateTime != null && !userData.StartDateTime.Equals(new DateTime()))
            {
                _preferencesService.SetValue(PreferenceKey.StartDateTime, userData.StartDateTime);
                userData.StartDateTime = new DateTime();
                _loggerService.Info("Migrated StartDateTime");
            }

            if (userData.LastProcessTekTimestamp != null && userData.LastProcessTekTimestamp.Count > 0)
            {
                var stringValue = Utils.SerializeToJson(userData.LastProcessTekTimestamp);
                _preferencesService.SetValue(PreferenceKey.LastProcessTekTimestamp, stringValue);
                userData.LastProcessTekTimestamp.Clear();
                _loggerService.Info("Migrated LastProcessTekTimestamp");
            }

            const string ConfigurationPropertyKey = "ExposureNotificationConfigration";

            if (_applicationPropertyService.ContainsKey(ConfigurationPropertyKey))
            {
                var configuration = _applicationPropertyService.GetProperties(ConfigurationPropertyKey) as string;
                if (!string.IsNullOrEmpty(configuration))
                {
                    _preferencesService.SetValue(PreferenceKey.ExposureNotificationConfiguration, configuration);
                }
                await _applicationPropertyService.Remove(ConfigurationPropertyKey);
                _loggerService.Info("Migrated ExposureNotificationConfiguration");
            }

            if (userData.ExposureInformation != null)
            {
                _secureStorageService.SetValue(PreferenceKey.ExposureInformation, JsonConvert.SerializeObject(userData.ExposureInformation));
                userData.ExposureInformation = null;
                _loggerService.Info("Migrated ExposureInformation");
            }

            if (userData.ExposureSummary != null)
            {
                _secureStorageService.SetValue(PreferenceKey.ExposureSummary, JsonConvert.SerializeObject(userData.ExposureSummary));
                userData.ExposureSummary = null;
                _loggerService.Info("Migrated ExposureSummary");
            }

            _loggerService.EndMethod();
        }
    }
}
