/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
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
                    timeZoneInfo: AppConstants.TIMEZONE_JST,
                    fallbackDateTime: AppConstants.COCOA_FIRST_RELEASE_DATE
                    );
            }

            if (_preferencesService.ContainsKey(PRIVACY_POLICY_LAST_UPDATE_DATETIME))
            {
                MigrateDateTimeToEpoch(
                    PRIVACY_POLICY_LAST_UPDATE_DATETIME,
                    PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch,
                    timeZoneInfo: AppConstants.TIMEZONE_JST,
                    fallbackDateTime: AppConstants.COCOA_FIRST_RELEASE_DATE
                    );
            }

            return Task.CompletedTask;
        }

        private void MigrateDateTimeToEpoch(string dateTimeKey, string epochKey, TimeZoneInfo? timeZoneInfo, DateTime fallbackDateTime)
        {
            string dateTimeStr = _preferencesService.GetStringValue(dateTimeKey, fallbackDateTime.ToString());

            /// **Note**
            /// `dateTime` still can be `0001/01/01 00:00:00` (= UNIX Epoch:`-62135596800`).
            /// For compatibility reasons, do not change this behavior.
            DateTime dateTime;
            try
            {
                dateTime = DateTime.Parse(dateTimeStr);

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

            _preferencesService.SetLongValue(epochKey, dateTime.ToUnixEpoch());
            _preferencesService.RemoveValue(dateTimeKey);
        }
    }
}
