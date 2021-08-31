/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public interface IUserDataRepository
    {
        void SetStartDate(DateTime dateTime);

        DateTime GetStartDate();
        int GetDaysOfUse();

        void RemoveStartDate();

        DateTime GetLastUpdateDate(TermsType termsType);
        void SaveLastUpdateDate(TermsType termsType, DateTime updateDate);

        bool IsAllAgreed();

        void RemoveAllUpdateDate();

        long GetLastProcessTekTimestamp(string region);
        void SetLastProcessTekTimestamp(string region, long created);
        void RemoveLastProcessTekTimestamp();
    }

    public class UserDataRepository : IUserDataRepository
    {
        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        private readonly IPreferencesService _preferencesService;
        private readonly ILoggerService _loggerService;

        public UserDataRepository(
            IPreferencesService preferencesService,
            ILoggerService loggerService
            )
        {
            _loggerService = loggerService;
            _preferencesService = preferencesService;
        }

        public void SetStartDate(DateTime dateTime)
        {
            _preferencesService.SetValue(PreferenceKey.StartDateTimeEpoch, dateTime.ToUnixEpoch());
        }

        public DateTime GetStartDate()
        {
            long epoch = _preferencesService.GetValue(PreferenceKey.StartDateTimeEpoch, DateTime.UtcNow.ToUnixEpoch());
            return UNIX_EPOCH.AddSeconds(epoch);
        }

        public int GetDaysOfUse()
        {
            TimeSpan timeSpan = DateTime.UtcNow - GetStartDate();
            return timeSpan.Days;
        }

        public void RemoveStartDate()
        {
            _loggerService.StartMethod();

            _preferencesService.RemoveValue(PreferenceKey.StartDateTimeEpoch);

            _loggerService.EndMethod();
        }

        public DateTime GetLastUpdateDate(TermsType termsType)
        {
            string key = termsType switch
            {
                TermsType.TermsOfService => PreferenceKey.TermsOfServiceLastUpdateDateTimeEpoch,
                TermsType.PrivacyPolicy => PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch,
                _ => throw new NotSupportedException()
            };

            long epoch = _preferencesService.GetValue(key, 0);

            return UNIX_EPOCH.AddSeconds(epoch);
        }

        public void SaveLastUpdateDate(TermsType termsType, DateTime updateDate)
        {
            _loggerService.StartMethod();

            var key = termsType == TermsType.TermsOfService ? PreferenceKey.TermsOfServiceLastUpdateDateTimeEpoch : PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch;
            _preferencesService.SetValue(key, updateDate.ToUnixEpoch());

            _loggerService.EndMethod();
        }

        public bool IsAllAgreed()
        {
            return (_preferencesService.ContainsKey(PreferenceKey.TermsOfServiceLastUpdateDateTimeEpoch) && _preferencesService.ContainsKey(PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch))
                ;
        }

        public void RemoveAllUpdateDate()
        {
            _loggerService.StartMethod();
            _preferencesService.RemoveValue(PreferenceKey.TermsOfServiceLastUpdateDateTimeEpoch);
            _preferencesService.RemoveValue(PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch);
            _loggerService.EndMethod();
        }

        public long GetLastProcessTekTimestamp(string region)
        {
            _loggerService.StartMethod();
            var result = 0L;
            var jsonString = _preferencesService.GetValue<string>(PreferenceKey.LastProcessTekTimestamp, null);
            if (!string.IsNullOrEmpty(jsonString))
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, long>>(jsonString);
                if (dict.ContainsKey(region))
                {
                    result = dict[region];
                }
            }
            _loggerService.EndMethod();
            return result;
        }

        public void SetLastProcessTekTimestamp(string region, long created)
        {
            _loggerService.StartMethod();
            var jsonString = _preferencesService.GetValue<string>(PreferenceKey.LastProcessTekTimestamp, null);
            Dictionary<string, long> newDict;
            if (!string.IsNullOrEmpty(jsonString))
            {
                newDict = JsonConvert.DeserializeObject<Dictionary<string, long>>(jsonString);
            }
            else
            {
                newDict = new Dictionary<string, long>();
            }
            newDict[region] = created;
            _preferencesService.SetValue(PreferenceKey.LastProcessTekTimestamp, JsonConvert.SerializeObject(newDict));
            _loggerService.EndMethod();
        }

        public void RemoveLastProcessTekTimestamp()
        {
            _loggerService.StartMethod();
            _preferencesService.RemoveValue(PreferenceKey.LastProcessTekTimestamp);
            _loggerService.EndMethod();
        }
    }
}
