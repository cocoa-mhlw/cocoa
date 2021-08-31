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
        public static string StartDateTime = "StartDateTime";
        public static string TermsOfServiceLastUpdateDateTime = "TermsOfServiceLastUpdateDateTime";
        public static string PrivacyPolicyLastUpdateDateTime = "PrivacyPolicyLastUpdateDateTime";

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
            if (_preferencesService.ContainsKey(StartDateTime))
            {
                string dateTimeStr = _preferencesService.GetValue(StartDateTime, DateTime.UtcNow.ToString());

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
                _preferencesService.SetValue(PreferenceKey.StartDateTimeEpoch, dateTime.ToUnixEpoch());
                _preferencesService.RemoveValue(StartDateTime);
            }

            if (_preferencesService.ContainsKey(TermsOfServiceLastUpdateDateTime))
            {
                string dateTimeStr = _preferencesService.GetValue(TermsOfServiceLastUpdateDateTime, DateTime.UtcNow.ToString());

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
                _preferencesService.SetValue(PreferenceKey.TermsOfServiceLastUpdateDateTimeEpoch, dateTime.ToUnixEpoch());
                _preferencesService.RemoveValue(TermsOfServiceLastUpdateDateTime);
            }

            if (_preferencesService.ContainsKey(PrivacyPolicyLastUpdateDateTime))
            {
                string dateTimeStr = _preferencesService.GetValue(PrivacyPolicyLastUpdateDateTime, DateTime.UtcNow.ToString());

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
                _preferencesService.SetValue(PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch, dateTime.ToUnixEpoch());
                _preferencesService.RemoveValue(PrivacyPolicyLastUpdateDateTime);
            }

            return Task.CompletedTask;
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
