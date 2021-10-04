/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public class UserDataRepository : IUserDataRepository
    {
        private const string KEY_LAST_PROCESS_DIAGNOSIS_KEY_TIMESTAMP = "last_process_diagnosis_key_timestamp";

        private const string KEY_DAILY_SUMMARIES = "daily_summaries";
        private const string KEY_EXPOSURE_WINDOWS = "exposure_windows";

        private const string EMPTY_LIST_JSON = "[]";

        private readonly IPreferencesService _preferencesService;
        private readonly ILoggerService _loggerService;

        public UserDataRepository(
            IPreferencesService preferencesService,
            ILoggerService loggerService
            )
        {
            _preferencesService = preferencesService;
            _loggerService = loggerService;
        }

        public Task<long> GetLastProcessDiagnosisKeyTimestampAsync(string region)
        {
            _loggerService.StartMethod();

            try
            {
                var result = 0L;

                var jsonString = _preferencesService.GetValue<string>(KEY_LAST_PROCESS_DIAGNOSIS_KEY_TIMESTAMP, null);
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
                var jsonString = _preferencesService.GetValue<string>(KEY_LAST_PROCESS_DIAGNOSIS_KEY_TIMESTAMP, null);

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
                _preferencesService.SetValue(KEY_LAST_PROCESS_DIAGNOSIS_KEY_TIMESTAMP, JsonConvert.SerializeObject(newDict));

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
                _preferencesService.RemoveValue(KEY_LAST_PROCESS_DIAGNOSIS_KEY_TIMESTAMP);
                return Task.CompletedTask;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        #region ExposureWindow mode
        public async Task SetExposureDataAsync(
            List<DailySummary> dailySummaryList,
            List<ExposureWindow> exposueWindowList
            )
        {
            _loggerService.StartMethod();

            dailySummaryList.AddRange(dailySummaryList);
            exposueWindowList.AddRange(exposueWindowList);

            dailySummaryList.Sort((a, b) => a.DateMillisSinceEpoch.CompareTo(b.DateMillisSinceEpoch));
            exposueWindowList.Sort((a, b) => a.DateMillisSinceEpoch.CompareTo(b.DateMillisSinceEpoch));

            await SaveExposureDataAsync(dailySummaryList, exposueWindowList);

            _loggerService.EndMethod();
        }

        public async Task<bool> AppendExposureDataAsync(
            List<DailySummary> dailySummaryList,
            List<ExposureWindow> exposueWindowList,
            bool ignoreDuplicate = true
            )
        {
            _loggerService.StartMethod();

            List<DailySummary> existDailySummaryList = await GetDailySummariesAsync();
            List<ExposureWindow> existExposureWindowList = await GetExposureWindowsAsync();

            bool isNewExposureDetected = false;

            foreach (var dailySummary in dailySummaryList)
            {
                if (!ignoreDuplicate || !existDailySummaryList.Contains(dailySummary))
                {
                    existDailySummaryList.Add(dailySummary);
                    isNewExposureDetected = true;
                }
            }

            foreach (var exposureWindow in exposueWindowList)
            {
                if (!ignoreDuplicate || !existExposureWindowList.Contains(exposureWindow))
                {
                    existExposureWindowList.Add(exposureWindow);
                    isNewExposureDetected = true;
                }
            }

            existDailySummaryList.Sort((a, b) => a.DateMillisSinceEpoch.CompareTo(b.DateMillisSinceEpoch));
            existExposureWindowList.Sort((a, b) => a.DateMillisSinceEpoch.CompareTo(b.DateMillisSinceEpoch));

            await SaveExposureDataAsync(existDailySummaryList, existExposureWindowList);

            _loggerService.EndMethod();

            return isNewExposureDetected;
        }

        private Task SaveExposureDataAsync(IList<DailySummary> dailySummaryList, IList<ExposureWindow> exposureWindowList)
        {
            _loggerService.StartMethod();

            string dailySummaryListJson = JsonConvert.SerializeObject(dailySummaryList);
            string exposureWindowListJson = JsonConvert.SerializeObject(exposureWindowList);

            try
            {
                _preferencesService.SetValue(KEY_DAILY_SUMMARIES, dailySummaryListJson);
                _preferencesService.SetValue(KEY_EXPOSURE_WINDOWS, exposureWindowListJson);
                return Task.CompletedTask;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public Task<List<DailySummary>> GetDailySummariesAsync()
        {
            _loggerService.StartMethod();

            try
            {
                string dailySummariesJson = _preferencesService.GetValue(KEY_DAILY_SUMMARIES, EMPTY_LIST_JSON);
                return Task.FromResult(
                    JsonConvert.DeserializeObject<List<DailySummary>>(dailySummariesJson)
                );
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task<List<DailySummary>> GetDailySummariesAsync(int offsetDays)
        {
            return (await GetDailySummariesAsync())
                .Where(dailySummary => dailySummary.GetDateTime().CompareTo(DateTimeUtility.Instance.UtcNow.AddDays(offsetDays)) >= 0)
                .ToList();
        }

        public Task<List<ExposureWindow>> GetExposureWindowsAsync()
        {
            _loggerService.StartMethod();

            try
            {
                string exposureWindowListJson = _preferencesService.GetValue(KEY_EXPOSURE_WINDOWS, EMPTY_LIST_JSON);
                return Task.FromResult(
                    JsonConvert.DeserializeObject<List<ExposureWindow>>(exposureWindowListJson)
                );
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task<List<ExposureWindow>> GetExposureWindowsAsync(int offsetDays)
        {
            return (await GetExposureWindowsAsync())
                .Where(dailySummary => dailySummary.GetDateTime().CompareTo(DateTimeUtility.Instance.UtcNow.AddDays(offsetDays)) >= 0)
                .ToList();
        }

        public Task RemoveDailySummariesAsync()
        {
            _loggerService.StartMethod();

            try
            {
                _preferencesService.RemoveValue(KEY_DAILY_SUMMARIES);
                return Task.CompletedTask;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public Task RemoveExposureWindowsAsync()
        {
            _loggerService.StartMethod();

            try
            {
                _preferencesService.RemoveValue(KEY_EXPOSURE_WINDOWS);
                return Task.CompletedTask;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
        #endregion

        #region Legacy v1
        public List<UserExposureInfo> GetExposureInformationList()
        {
            _loggerService.StartMethod();
            List<UserExposureInfo> result = null;
            var exposureInformationJson = _preferencesService.GetValue<string>(PreferenceKey.ExposureInformation, null);
            if (!string.IsNullOrEmpty(exposureInformationJson))
            {
                result = JsonConvert.DeserializeObject<List<UserExposureInfo>>(exposureInformationJson);
            }
            _loggerService.EndMethod();
            return result;
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

        public void SetExposureInformation(List<UserExposureInfo> exposureInformationList)
        {
            _loggerService.StartMethod();

            string exposureInformationListJson = JsonConvert.SerializeObject(exposureInformationList);

            try
            {
                _preferencesService.SetValue(PreferenceKey.ExposureInformation, exposureInformationListJson);
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public bool AppendExposureData(
            ExposureSummary exposureSummary,
            List<ExposureInformation> exposureInformationList,
            int minimumRiskScore
            )
        {
            var existExposureInformationList = GetExposureInformationList() ?? new List<UserExposureInfo>();
            bool isNewExposureDetected = false;

            if (exposureSummary.MaximumRiskScore >= minimumRiskScore)
            {
                foreach (var exposureInfo in exposureInformationList)
                {
                    _loggerService.Info($"Exposure.Timestamp: {exposureInfo.DateMillisSinceEpoch}");
                    _loggerService.Info($"Exposure.Duration: {exposureInfo.DurationInMillis}");
                    _loggerService.Info($"Exposure.AttenuationValue: {exposureInfo.AttenuationValue}");
                    _loggerService.Info($"Exposure.TotalRiskScore: {exposureInfo.TotalRiskScore}");
                    _loggerService.Info($"Exposure.TransmissionRiskLevel: {exposureInfo.TransmissionRiskLevel}");

                    if (exposureInfo.TotalRiskScore >= minimumRiskScore)
                    {
                        existExposureInformationList.Add(new UserExposureInfo(exposureInfo));
                        isNewExposureDetected = true;
                    }
                }

                _loggerService.Info($"Save ExposureSummary. MatchedKeyCount: {exposureSummary.MatchedKeyCount}");
                _loggerService.Info($"Save ExposureInformation. Count: {existExposureInformationList.Count}");

                existExposureInformationList.Sort((a, b) => a.Timestamp.CompareTo(b.Timestamp));

                SetExposureInformation(existExposureInformationList);
            }

            return isNewExposureDetected;
        }

        public void RemoveExposureInformation()
        {
            _loggerService.StartMethod();
            _preferencesService.RemoveValue(PreferenceKey.ExposureInformation);
            _loggerService.EndMethod();
        }

        public void RemoveOutOfDateExposureInformation(int offsetDays)
        {
            _loggerService.StartMethod();

            var informationList = GetExposureInformationList(offsetDays) ?? new List<UserExposureInfo>();
            SetExposureInformation(informationList);

            _loggerService.EndMethod();
        }

        public int GetV1ExposureCount(int offsetDays)
        {
            _loggerService.StartMethod();
            var exposureInformationList = GetExposureInformationList(offsetDays) ?? new List<UserExposureInfo>();
            _loggerService.EndMethod();
            return exposureInformationList.Count;
        }

        #endregion
    }
}
