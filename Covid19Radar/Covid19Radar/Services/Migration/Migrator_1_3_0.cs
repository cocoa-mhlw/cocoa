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

        private readonly DateTime FALLBACK_DATETIME = new DateTime(2020, 6, 19);
        private readonly TimeSpan TIME_DIFFERENCIAL_JST_UTC = TimeSpan.FromHours(+9);

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
                MigrateDateTimeToEpoch(START_DATETIME, PreferenceKey.StartDateTimeEpoch, TimeSpan.Zero, DateTime.UtcNow);
            }

            if (_preferencesService.ContainsKey(TERMS_OF_SERVICE_LAST_UPDATE_DATETIME))
            {
                MigrateDateTimeToEpoch(
                    TERMS_OF_SERVICE_LAST_UPDATE_DATETIME,
                    PreferenceKey.TermsOfServiceLastUpdateDateTimeEpoch,
                    -TIME_DIFFERENCIAL_JST_UTC,
                    FALLBACK_DATETIME
                    );
            }

            if (_preferencesService.ContainsKey(PRIVACY_POLICY_LAST_UPDATE_DATETIME))
            {
                MigrateDateTimeToEpoch(
                    PRIVACY_POLICY_LAST_UPDATE_DATETIME,
                    PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch,
                    -TIME_DIFFERENCIAL_JST_UTC,
                    FALLBACK_DATETIME
                    );
            }

            return Task.CompletedTask;
        }

        private void MigrateDateTimeToEpoch(string dateTimeKey, string epochKey, TimeSpan differential, DateTime fallbackDateTime)
        {
            string dateTimeStr = _preferencesService.GetValue(dateTimeKey, fallbackDateTime.ToString());

            DateTime dateTime;
            try
            {
                dateTime = DateTime.SpecifyKind(DateTime.Parse(dateTimeStr), DateTimeKind.Utc);
            }
            catch (FormatException exception)
            {
                _loggerService.Exception($"Parse dateTime FormatException occurred. {dateTimeStr}", exception);
                dateTime = fallbackDateTime;
            }

            try
            {
                dateTime += differential;
            }
            catch (ArgumentOutOfRangeException exception)
            {
                _loggerService.Exception($"{dateTimeStr} {differential} The added or subtracted value results in an un-representable DateTime.", exception);
            }

            _preferencesService.SetValue(epochKey, dateTime.ToUnixEpoch());
            _preferencesService.RemoveValue(dateTimeKey);
        }
    }
}
