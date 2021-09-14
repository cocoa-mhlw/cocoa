/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
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

        void SetCanConfirmExposure(bool canConfirmExposure);
        bool IsCanConfirmExposure();

        void SetLastConfirmedDate(DateTime utcNow);
        DateTime? GetLastConfirmedDate();

        void RemoveAllExposureNotificationStatus();

        List<UserExposureInfo> GetExposureInformationList();
        List<UserExposureInfo> GetExposureInformationList(int offsetDays);
        void SetExposureInformation(List<UserExposureInfo> informationList);
        void RemoveExposureInformation();
        void RemoveOutOfDateExposureInformation(int offsetDays);

        int GetExposureCount(int offsetDays);
    }

    public class UserDataRepository : IUserDataRepository
    {
        private readonly IPreferencesService _preferencesService;
        private readonly ISecureStorageService _secureStorageService;
        private readonly ILoggerService _loggerService;

        public UserDataRepository(
            IPreferencesService preferencesService,
            ISecureStorageService secureStorageService,
            ILoggerService loggerService
            )
        {
            _loggerService = loggerService;
            _secureStorageService = secureStorageService;
            _preferencesService = preferencesService;
        }

        public void SetStartDate(DateTime dateTime)
        {
            _preferencesService.SetValue(PreferenceKey.StartDateTimeEpoch, dateTime.ToUnixEpoch());
        }

        public DateTime GetStartDate()
        {
            long epoch = _preferencesService.GetValue(PreferenceKey.StartDateTimeEpoch, DateTime.UtcNow.ToUnixEpoch());
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

        public DateTime GetLastUpdateDate(TermsType termsType)
        {
            string key = termsType switch
            {
                TermsType.TermsOfService => PreferenceKey.TermsOfServiceLastUpdateDateTimeEpoch,
                TermsType.PrivacyPolicy => PreferenceKey.PrivacyPolicyLastUpdateDateTimeEpoch,
                _ => throw new NotSupportedException()
            };

            long epoch = _preferencesService.GetValue(key, 0L);

            return DateTime.UnixEpoch.AddSeconds(epoch);
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

        public void SetCanConfirmExposure(bool canConfirmExposure)
        {
            _loggerService.StartMethod();
            _preferencesService.SetValue(PreferenceKey.CanConfirmExposure, canConfirmExposure);
            _loggerService.EndMethod();
        }

        public bool IsCanConfirmExposure()
        {
            _loggerService.StartMethod();
            var canConfirmExposure = _preferencesService.GetValue(PreferenceKey.CanConfirmExposure, true);
            _loggerService.EndMethod();

            return canConfirmExposure;
        }

        public void SetLastConfirmedDate(DateTime dateTime)
        {
            _loggerService.StartMethod();
            _preferencesService.SetValue(PreferenceKey.LastConfirmedDateTimeEpoch, dateTime.ToUnixEpoch());
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

                long epoch = _preferencesService.GetValue(PreferenceKey.LastConfirmedDateTimeEpoch, 0L);
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

        public List<UserExposureInfo> GetExposureInformationList()
        {
            _loggerService.StartMethod();
            List<UserExposureInfo> result = null;
            var exposureInformationJson = _secureStorageService.GetValue<string>(PreferenceKey.ExposureInformation);
            if (!string.IsNullOrEmpty(exposureInformationJson))
            {
                result = JsonConvert.DeserializeObject<List<UserExposureInfo>>(exposureInformationJson);
            }
            _loggerService.EndMethod();
            return result;
        }

        public void SetExposureInformation(List<UserExposureInfo> informationList)
        {
            _loggerService.StartMethod();
            var informationListJson = JsonConvert.SerializeObject(informationList);
            _secureStorageService.SetValue(PreferenceKey.ExposureInformation, informationListJson);
            _loggerService.EndMethod();
        }

        public void RemoveExposureInformation()
        {
            _loggerService.StartMethod();
            _secureStorageService.RemoveValue(PreferenceKey.ExposureInformation);
            _loggerService.EndMethod();
        }

        public void RemoveOutOfDateExposureInformation(int offsetDays)
        {
            _loggerService.StartMethod();

            var informationList = GetExposureInformationList(offsetDays) ?? new List<UserExposureInfo>();
            SetExposureInformation(informationList);

            _loggerService.EndMethod();
        }

        public List<UserExposureInfo> GetExposureInformationList(int offsetDays)
        {
            _loggerService.StartMethod();
            var date = DateTimeUtility.Instance.UtcNow.AddDays(offsetDays);
            var list = GetExposureInformationList()?
                .Where(x => x.Timestamp.CompareTo(date) >= 0)
                .ToList();
            _loggerService.EndMethod();
            return list;
        }

        public int GetExposureCount(int offsetDays)
        {
            _loggerService.StartMethod();
            int result = 0;
            var exposureInformationList = GetExposureInformationList(offsetDays);
            if (exposureInformationList != null)
            {
                result = exposureInformationList.Count;
            }
            _loggerService.EndMethod();
            return result;
        }
    }
}
