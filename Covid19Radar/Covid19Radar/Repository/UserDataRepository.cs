/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public interface IUserDataRepository
    {
        private const string EVENT_EXPOSURE_NOTIFICATION_NOTIFIED = "exposure_notification_notified";

        void SetStartDate(DateTime dateTime);

        DateTime GetStartDate();
        int GetDaysOfUse();

        void RemoveStartDate();

        DateTime GetLastUpdateDate(TermsType termsType);
        void SaveLastUpdateDate(TermsType termsType, DateTime updateDate);

        bool IsAllAgreed();

        void RemoveAllUpdateDate();

        void SetCanConfirmExposure(bool canConfirmExposure);
        bool IsCanConfirmExposure();

        void SetIsMaxPerDayExposureDetectionAPILimitReached(bool isMaxPerDayExposureDetectionAPILimitReached);
        bool IsMaxPerDayExposureDetectionAPILimitReached();

        void SetLastConfirmedDate(DateTime utcNow);
        DateTime? GetLastConfirmedDate();

        void RemoveAllExposureNotificationStatus();

        Task<long> GetLastProcessDiagnosisKeyTimestampAsync(string region);
        Task SetLastProcessDiagnosisKeyTimestampAsync(string region, long timestamp);
        Task RemoveLastProcessDiagnosisKeyTimestampAsync();
    }

    public class UserDataRepository : IUserDataRepository
    {
        private readonly IPreferencesService _preferencesService;
        private readonly IDateTimeUtility _dateTimeUtility;
        private readonly ILoggerService _loggerService;

        public UserDataRepository(
            IPreferencesService preferencesService,
            IDateTimeUtility dateTimeUtility,
            ILoggerService loggerService
            )
        {
            _preferencesService = preferencesService;
            _dateTimeUtility = dateTimeUtility;
            _loggerService = loggerService;
        }

        public void SetStartDate(DateTime dateTime)
        {
            _preferencesService.SetLongValue(PreferenceKey.StartDateTimeEpoch, dateTime.ToUnixEpoch());
        }

        public DateTime GetStartDate()
        {
            long epoch = _preferencesService.GetLongValue(PreferenceKey.StartDateTimeEpoch, DateTime.UtcNow.ToUnixEpoch());
            return DateTime.UnixEpoch.AddSeconds(epoch);
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

        public Task<long> GetLastProcessDiagnosisKeyTimestampAsync(string region)
        {
            _loggerService.StartMethod();

            try
            {
                var result = 0L;

                var jsonString = _preferencesService.GetStringValue(PreferenceKey.LastProcessTekTimestamp, null);
                if (!string.IsNullOrEmpty(jsonString))
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, long>>(jsonString);
                    if (dict.ContainsKey(region))
                    {
                        result = dict[region];
                    }
                }

                return Task.FromResult(result);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public Task SetLastProcessDiagnosisKeyTimestampAsync(string region, long timestamp)
        {
            _loggerService.StartMethod();

            try
            {
                var jsonString = _preferencesService.GetStringValue(PreferenceKey.LastProcessTekTimestamp, null);

                Dictionary<string, long> newDict;
                if (!string.IsNullOrEmpty(jsonString))
                {
                    newDict = JsonConvert.DeserializeObject<Dictionary<string, long>>(jsonString);
                }
                else
                {
                    newDict = new Dictionary<string, long>();
                }
                newDict[region] = timestamp;
                _preferencesService.SetStringValue(PreferenceKey.LastProcessTekTimestamp, JsonConvert.SerializeObject(newDict));

                return Task.CompletedTask;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public Task RemoveLastProcessDiagnosisKeyTimestampAsync()
        {
            _loggerService.StartMethod();

            try
            {
                _preferencesService.RemoveValue(PreferenceKey.LastProcessTekTimestamp);
                return Task.CompletedTask;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public DateTime GetLastUpdateDate(TermsType termsType)
        {
            string key = termsType switch
            {
                TermsType.TermsOfService => PreferenceKey.TermsOfServiceLastUpdateDateTimeEpoch,
                TermsType.PrivacyPolicy => PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch,
                _ => throw new NotSupportedException()
            };

            long epoch = _preferencesService.GetLongValue(key, 0L);

            return DateTime.UnixEpoch.AddSeconds(epoch);
        }

        public void SaveLastUpdateDate(TermsType termsType, DateTime updateDate)
        {
            _loggerService.StartMethod();

            var key = termsType == TermsType.TermsOfService ? PreferenceKey.TermsOfServiceLastUpdateDateTimeEpoch : PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch;
            _preferencesService.SetLongValue(key, updateDate.ToUnixEpoch());

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

        public void SetCanConfirmExposure(bool canConfirmExposure)
        {
            _loggerService.StartMethod();
            _preferencesService.SetBoolValue(PreferenceKey.CanConfirmExposure, canConfirmExposure);
            _loggerService.EndMethod();
        }

        public bool IsCanConfirmExposure()
        {
            _loggerService.StartMethod();
            var canConfirmExposure = _preferencesService.GetBoolValue(PreferenceKey.CanConfirmExposure, true);
            _loggerService.EndMethod();

            return canConfirmExposure;
        }

        public void SetIsMaxPerDayExposureDetectionAPILimitReached(bool isMaxPerDayExposureDetectionAPILimitReached)
        {
            _loggerService.StartMethod();
            _preferencesService.SetBoolValue(PreferenceKey.IsMaxPerDayExposureDetectionAPILimitReached, isMaxPerDayExposureDetectionAPILimitReached);
            _loggerService.EndMethod();
        }

        public bool IsMaxPerDayExposureDetectionAPILimitReached()
        {
            _loggerService.StartMethod();
            var isMaxPerDayExposureDetectionAPILimitReached = _preferencesService.GetBoolValue(PreferenceKey.IsMaxPerDayExposureDetectionAPILimitReached, false);
            _loggerService.EndMethod();

            return isMaxPerDayExposureDetectionAPILimitReached;
        }

        public void SetLastConfirmedDate(DateTime dateTime)
        {
            _loggerService.StartMethod();
            _preferencesService.SetLongValue(PreferenceKey.LastConfirmedDateTimeEpoch, dateTime.ToUnixEpoch());
            _loggerService.EndMethod();
        }


        public DateTime? GetLastConfirmedDate()
        {
            _loggerService.StartMethod();
            try
            {
                if (!_preferencesService.ContainsKey(PreferenceKey.LastConfirmedDateTimeEpoch))
                {
                    return null;
                }

                long epoch = _preferencesService.GetLongValue(PreferenceKey.LastConfirmedDateTimeEpoch, 0L);
                return DateTime.UnixEpoch.AddSeconds(epoch);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public void RemoveAllExposureNotificationStatus()
        {
            _loggerService.StartMethod();
            _preferencesService.RemoveValue(PreferenceKey.CanConfirmExposure);
            _preferencesService.RemoveValue(PreferenceKey.LastConfirmedDateTimeEpoch);
            _loggerService.EndMethod();
        }
    }
}
