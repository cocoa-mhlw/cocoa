/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Globalization;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services.Migration
{
    internal class Migrator_1_3_0
    {
        private const string START_DATETIME = "StartDateTime";
        private const string TERMS_OF_SERVICE_LAST_UPDATE_DATETIME = "TermsOfServiceLastUpdateDateTime";
        private const string PRIVACY_POLICY_LAST_UPDATE_DATETIME = "PrivacyPolicyLastUpdateDateTime";

        private static readonly CultureInfo GB_CULTURE_INFO = new CultureInfo("en-GB");

        private readonly IPreferencesService _preferencesService;
        private readonly ILoggerService _loggerService;

        public Migrator_1_3_0(
            IPreferencesService preferencesService,
            ILoggerService loggerService
            )
        {
            _preferencesService = preferencesService;
            _loggerService = loggerService;
        }

        public Task ExecuteAsync()
        {
            if (_preferencesService.ContainsKey(START_DATETIME))
            {
                MigrateDateTimeToEpoch(START_DATETIME, PreferenceKey.StartDateTimeEpoch);
            }

            if (_preferencesService.ContainsKey(TERMS_OF_SERVICE_LAST_UPDATE_DATETIME))
            {
                MigrateDateTimeToEpoch(TERMS_OF_SERVICE_LAST_UPDATE_DATETIME, PreferenceKey.TermsOfServiceLastUpdateDateTimeEpoch);
            }

            if (_preferencesService.ContainsKey(PRIVACY_POLICY_LAST_UPDATE_DATETIME))
            {
                MigrateDateTimeToEpoch(PRIVACY_POLICY_LAST_UPDATE_DATETIME, PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch);
            }

            return Task.CompletedTask;
        }

        private void MigrateDateTimeToEpoch(string dateTimeKey, string epochKey)
        {
            string dateTimeStr = _preferencesService.GetValue(dateTimeKey, DateTime.UtcNow.ToString());

            DateTime dateTime;
            try
            {
                dateTime = DateTime.SpecifyKind(DateTime.Parse(dateTimeStr), DateTimeKind.Utc);
            }
            catch (FormatException exception)
            {
                _loggerService.Exception($"Parse dateTime FormatException occurred. {dateTimeStr}", exception);
                dateTime = ParceDateTimeAsGBLocaleForFailback(dateTimeStr);
            }
            _preferencesService.SetValue(epochKey, dateTime.ToUnixEpoch());
            _preferencesService.RemoveValue(dateTimeKey);
        }

        private DateTime ParceDateTimeAsGBLocaleForFailback(string dateTimeStr)
        {
            try
            {
                return DateTime.SpecifyKind(DateTime.Parse(dateTimeStr, GB_CULTURE_INFO), DateTimeKind.Utc);
            }
            catch (FormatException exception)
            {
                _loggerService.Exception($"Parse dateTime as GB locale, FormatException occurred. {dateTimeStr}", exception);
                return DateTime.UtcNow;
            }
        }
    }
}
