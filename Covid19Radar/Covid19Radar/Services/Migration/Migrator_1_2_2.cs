/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Services.Migration
{
    internal class Migrator_1_2_2
    {
        private const string APPLICATION_PROPERTY_USER_DATA_KEY = "UserData";
        private const string APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY = "TermsOfServiceLastUpdateDateTime";
        private const string APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY = "PrivacyPolicyLastUpdateDateTime";

        public static string PREFERENCE_KEY_START_DATETIME = "StartDateTime";
        public static string PREFERENCE_KEY_TERMS_OF_SERVICE_LAST_UPDATE_DATETIME = "TermsOfServiceLastUpdateDateTime";
        public static string PREFERENCE_KEY_PRIVACY_POLICY_LAST_UPDATE_DATETIME = "PrivacyPolicyLastUpdateDateTime";

        private const string PREFERENCE_KEY_EXPOSURE_SUMMARY = "ExposureSummary";

        private readonly IApplicationPropertyService _applicationPropertyService;
        private readonly IPreferencesService _preferencesService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly ILoggerService _loggerService;

        public Migrator_1_2_2(
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

        public async Task ExecuteAsync()
        {
            _loggerService.StartMethod();

            if (await DetectCorruptData())
            {
                await TryRecoveryDataAsync();
            }

            var userData = GetUserDataFromApplicationProperties();

            if (userData != null)
            {
                await MigrateFromApplicationUserDataToPlatformPreferenceAsync(userData);
                await _applicationPropertyService.Remove(APPLICATION_PROPERTY_USER_DATA_KEY);
            }

            _loggerService.EndMethod();
        }

        private Task<bool> DetectCorruptData()
        {
            var existsCorruptData = false;

            try
            {
                _ = GetUserDataFromApplicationProperties();
            }
            catch (FormatException e)
            {
                existsCorruptData = true;
                _loggerService.Exception("GetUserDataFromApplicationProperties FormatException", e);
            }
            catch (JsonReaderException e)
            {
                existsCorruptData = true;
                _loggerService.Exception("GetUserDataFromApplicationProperties JsonReaderException", e);
            }

            if (_applicationPropertyService.ContainsKey(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY))
            {
                try
                {
                    _ = DateTime.Parse(_applicationPropertyService.GetProperties(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY).ToString());
                }
                catch (FormatException e)
                {
                    existsCorruptData = true;
                    _loggerService.Exception("TermsOfServiceLastUpdateDateTime FormatException", e);
                }
            }

            if (_applicationPropertyService.ContainsKey(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY))
            {
                try
                {
                    _ = DateTime.Parse(_applicationPropertyService.GetProperties(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY).ToString());
                }
                catch (FormatException e)
                {
                    existsCorruptData = true;
                    _loggerService.Exception("PrivacyPolicyLastUpdateDateTime FormatException", e);
                }
            }

            return Task.FromResult(existsCorruptData);
        }

        private async Task TryRecoveryDataAsync()
        {
            // Try recovery UserData
            var userData = _applicationPropertyService.GetProperties(APPLICATION_PROPERTY_USER_DATA_KEY).ToString();
            var userDataModelForFailback = JsonConvert.DeserializeObject<UserDataModelForFailback>(userData);

            // Clear DateTime
            userDataModelForFailback.StartDateTime = DateTime.UtcNow.ToString();

            // Save UserData recovered
            string userDataModelForFailbackString = JsonConvert.SerializeObject(userDataModelForFailback);
            await _applicationPropertyService.SavePropertiesAsync(APPLICATION_PROPERTY_USER_DATA_KEY, userDataModelForFailbackString);

            // Remove all ApplicationProperties
            await _applicationPropertyService.Remove(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY);
            await _applicationPropertyService.Remove(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY);
        }

        private UserDataModel GetUserDataFromApplicationProperties()
        {
            _loggerService.StartMethod();

            var existsUserData = _applicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY);
            if (existsUserData)
            {
                _loggerService.EndMethod();

                var userData = _applicationPropertyService.GetProperties(APPLICATION_PROPERTY_USER_DATA_KEY);
                return JsonConvert.DeserializeObject<UserDataModel>(userData.ToString());
            }

            _loggerService.EndMethod();
            return null;
        }

        private async Task MigrateTermAsync(TermsType termsType, bool isAgree)
        {
            _loggerService.StartMethod();

            var applicationPropertyKey = termsType == TermsType.TermsOfService ? APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY : APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY;
            var preferenceKey = termsType == TermsType.TermsOfService ? PREFERENCE_KEY_TERMS_OF_SERVICE_LAST_UPDATE_DATETIME : PREFERENCE_KEY_PRIVACY_POLICY_LAST_UPDATE_DATETIME;

            if (_preferencesService.ContainsKey(applicationPropertyKey))
            {
                _loggerService.EndMethod();
                return;
            }

            if (isAgree)
            {
                if (_applicationPropertyService.ContainsKey(applicationPropertyKey))
                {
                    var lastUpdateDate = _applicationPropertyService.GetProperties(applicationPropertyKey).ToString();
                    _preferencesService.SetStringValue(preferenceKey, lastUpdateDate);
                }
                else
                {
                    /// **WARNING**
                    /// `new DateTime()` means `DateTime.MinValue`, it equals `0001/01/01 00:00:00`.
                    /// For converting timezone, please use `TimeZoneInfo.ContertTimeTo*`.
                    /// Do not use direct calculation (e.g. subtract `TimeSpan.FromHours(9)` from `DateTime.MinValue`)
                    /// because it cause an ArgumentOutOfRangeException.
                    _preferencesService.SetStringValue(preferenceKey, new DateTime().ToString());
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
                _preferencesService.SetStringValue(PREFERENCE_KEY_START_DATETIME, userData.StartDateTime.ToString());
                userData.StartDateTime = new DateTime();
                _loggerService.Info("Migrated StartDateTime");
            }

            if (userData.LastProcessTekTimestamp != null && userData.LastProcessTekTimestamp.Count > 0)
            {
                var stringValue = JsonConvert.SerializeObject(userData.LastProcessTekTimestamp);
                _preferencesService.SetStringValue(PreferenceKey.LastProcessTekTimestamp, stringValue);
                userData.LastProcessTekTimestamp.Clear();
                _loggerService.Info("Migrated LastProcessTekTimestamp");
            }

            const string ConfigurationPropertyKey = "ExposureNotificationConfigration";

            if (_applicationPropertyService.ContainsKey(ConfigurationPropertyKey))
            {
                var configuration = _applicationPropertyService.GetProperties(ConfigurationPropertyKey).ToString();
                if (!string.IsNullOrEmpty(configuration))
                {
                    _preferencesService.SetStringValue(PreferenceKey.ExposureNotificationConfiguration, configuration);
                }
                await _applicationPropertyService.Remove(ConfigurationPropertyKey);
                _loggerService.Info("Migrated ExposureNotificationConfiguration");
            }

            if (userData.ExposureInformation != null)
            {
                _secureStorageService.SetStringValue(PreferenceKey.ExposureInformation, JsonConvert.SerializeObject(userData.ExposureInformation));
                userData.ExposureInformation = null;
                _loggerService.Info("Migrated ExposureInformation");
            }

            if (userData.ExposureSummary != null)
            {
                _secureStorageService.SetStringValue(PREFERENCE_KEY_EXPOSURE_SUMMARY, JsonConvert.SerializeObject(userData.ExposureSummary));
                userData.ExposureSummary = null;
                _loggerService.Info("Migrated ExposureSummary");
            }

            _loggerService.EndMethod();
        }

        class UserDataModelForFailback
        {
            public string StartDateTime { get; set; }
            public bool IsOptined { get; set; } = false;
            public bool IsPolicyAccepted { get; set; } = false;
            public Dictionary<string, long> LastProcessTekTimestamp { get; set; } = new Dictionary<string, long>();
            public ObservableCollection<UserExposureInfo> ExposureInformation { get; set; } = new ObservableCollection<UserExposureInfo>();
            public UserExposureSummary ExposureSummary { get; set; }
        }
    }
}
