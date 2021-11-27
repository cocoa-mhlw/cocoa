/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Services.Logs;
using TimeZoneConverter;

namespace Covid19Radar.Services.Migration
{
    internal class Migrator_1_3_0
    {
        private const string START_DATETIME = "StartDateTime";
        private const string TERMS_OF_SERVICE_LAST_UPDATE_DATETIME = "TermsOfServiceLastUpdateDateTime";
        private const string PRIVACY_POLICY_LAST_UPDATE_DATETIME = "PrivacyPolicyLastUpdateDateTime";

        private readonly DateTime COCOA_RELEASE_DATE
            = DateTime.SpecifyKind(new DateTime(2021, 06, 19, 9, 0, 0), DateTimeKind.Utc);

        private readonly TimeZoneInfo TIMEZONE_JST = TZConvert.GetTimeZoneInfo("ASIA/Tokyo");

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
                MigrateDateTimeToEpoch(START_DATETIME, PreferenceKey.StartDateTimeEpoch,
                    timeZoneInfo: null,
                    fallbackDateTime: DateTime.UtcNow
                    );
            }

            if (_preferencesService.ContainsKey(TERMS_OF_SERVICE_LAST_UPDATE_DATETIME))
            {
                MigrateDateTimeToEpoch(
                    TERMS_OF_SERVICE_LAST_UPDATE_DATETIME,
                    PreferenceKey.TermsOfServiceLastUpdateDateTimeEpoch,
                    timeZoneInfo: TIMEZONE_JST,
                    fallbackDateTime: COCOA_RELEASE_DATE
                    );
            }

            if (_preferencesService.ContainsKey(PRIVACY_POLICY_LAST_UPDATE_DATETIME))
            {
                MigrateDateTimeToEpoch(
                    PRIVACY_POLICY_LAST_UPDATE_DATETIME,
                    PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch,
                    timeZoneInfo: TIMEZONE_JST,
                    fallbackDateTime: COCOA_RELEASE_DATE
                    );
            }

            return Task.CompletedTask;
        }

        public void MigrateDateTimeToEpoch(string dateTimeKey, string epochKey, TimeZoneInfo? timeZoneInfo, DateTime fallbackDateTime)
        {
            string dateTimeStr = _preferencesService.GetValue(dateTimeKey, DateTime.UtcNow.ToString());
            
            DateTime dateTime;
            try
            {
                dateTime = DateTime.Parse(dateTimeStr);

                // dateTime must be after COCOA_RELEASE_DATE.
                if (dateTime < COCOA_RELEASE_DATE)
                {
                    dateTime = COCOA_RELEASE_DATE;
                }

                if (timeZoneInfo is null)
                {
                    dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                }
                else
                {
                    dateTime = TimeZoneInfo.ConvertTimeToUtc(
                        DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified),
                        timeZoneInfo
                        );
                }
            }
            catch (FormatException exception)
            {
                _loggerService.Exception($"Parse dateTime FormatException occurred. {dateTimeStr}", exception);
                dateTime = fallbackDateTime;
            }
            _preferencesService.SetValue(epochKey, dateTime.ToUnixEpoch());
            _preferencesService.RemoveValue(dateTimeKey);
        }
    }
}
