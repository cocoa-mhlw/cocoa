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

        private const string KEY_EXPOSURE_SUMMARIES = "exposure_summaries";
        private const string KEY_EXPOSURE_INFORMATIONS = "exposure_informations";

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

            await SetExposureDataAsync(dailySummaryList, exposueWindowList);

            _loggerService.EndMethod();
        }

        public async Task AppendExposureDataAsync(
            List<DailySummary> dailySummaryList,
            List<ExposureWindow> exposueWindowList
            )
        {
            _loggerService.StartMethod();

            List<DailySummary> existDailySummaryList = await GetDailySummariesAsync();
            List<ExposureWindow> existExposureWindowList = await GetExposureWindowsAsync();

            existDailySummaryList.AddRange(dailySummaryList);
            existExposureWindowList.AddRange(exposueWindowList);

            existDailySummaryList.Sort((a, b) => a.DateMillisSinceEpoch.CompareTo(b.DateMillisSinceEpoch));
            existExposureWindowList.Sort((a, b) => a.DateMillisSinceEpoch.CompareTo(b.DateMillisSinceEpoch));

            await SetExposureDataAsync(existDailySummaryList, existExposureWindowList);

            _loggerService.EndMethod();
        }

        private Task SetExposureDataAsync(IList<DailySummary> dailySummaryList, IList<ExposureWindow> exposureWindowList)
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
                string dailySummariesJson = _preferencesService.GetValue<string>(KEY_DAILY_SUMMARIES, EMPTY_LIST_JSON);
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
                string exposureWindowListJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_WINDOWS, EMPTY_LIST_JSON);
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

        public Task<List<UserExposureSummary>> GetUserExposureSummariesAsync()
        {
            _loggerService.StartMethod();

            try
            {
                string exposureSummaryListJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_SUMMARIES, EMPTY_LIST_JSON);
                List<ExposureSummary> exposureSummaryList = JsonConvert.DeserializeObject<List<ExposureSummary>>(exposureSummaryListJson);

                return Task.FromResult(
                    exposureSummaryList
                        .Select(exposureSummary => new UserExposureSummary(exposureSummary))
                        .ToList()
                    );
            }
            finally
            {
                _loggerService.EndMethod();
            }
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
        public async Task<bool> AppendExposureDataAsync(
            ExposureSummary exposureSummary,
            List<ExposureInformation> exposureInformationList,
            int minimumRiskScore
            )
        {
            var (existExposureSummaryList, existExposureInformationList) = await GetExposureInformationDataAsync();
            bool isNewExposureDetected = false;

            if (exposureSummary.MaximumRiskScore >= minimumRiskScore)
            {
                existExposureSummaryList.Add(exposureSummary);

                foreach (var exposureInfo in exposureInformationList)
                {
                    _loggerService.Info($"Exposure.Timestamp: {exposureInfo.DateMillisSinceEpoch}");
                    _loggerService.Info($"Exposure.Duration: {exposureInfo.DurationInMillis}");
                    _loggerService.Info($"Exposure.AttenuationValue: {exposureInfo.AttenuationValue}");
                    _loggerService.Info($"Exposure.TotalRiskScore: {exposureInfo.TotalRiskScore}");
                    _loggerService.Info($"Exposure.TransmissionRiskLevel: {exposureInfo.TransmissionRiskLevel}");

                    if (exposureInfo.TotalRiskScore >= minimumRiskScore)
                    {
                        existExposureInformationList.Add(exposureInfo);
                        isNewExposureDetected = true;
                    }
                }

                _loggerService.Info($"Save ExposureSummary. MatchedKeyCount: {exposureSummary.MatchedKeyCount}");
                _loggerService.Info($"Save ExposureInformation. Count: {existExposureInformationList.Count}");

                existExposureInformationList.Sort((a, b) => a.DateMillisSinceEpoch.CompareTo(b.DateMillisSinceEpoch));

                await SetExposureDataAsync(existExposureSummaryList, existExposureInformationList);
            }

            return isNewExposureDetected;
        }

        private Task SetExposureDataAsync(IList<ExposureSummary> exposureSummaryList, IList<ExposureInformation> exposureInformationList)
        {
            _loggerService.StartMethod();

            string exposureSummaryListJson = JsonConvert.SerializeObject(exposureSummaryList);
            string exposureInformationListJson = JsonConvert.SerializeObject(exposureInformationList);

            try
            {
                _preferencesService.SetValue(KEY_EXPOSURE_SUMMARIES, exposureSummaryListJson);
                _preferencesService.SetValue(KEY_EXPOSURE_INFORMATIONS, exposureInformationListJson);
                return Task.CompletedTask;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public Task<List<UserExposureInfo>> GetUserExposureInfosAsync()
        {
            _loggerService.StartMethod();

            try
            {
                string exposureInformationListJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_INFORMATIONS, EMPTY_LIST_JSON);
                List<ExposureInformation> exposureInformationList = JsonConvert.DeserializeObject<List<ExposureInformation>>(exposureInformationListJson);

                return Task.FromResult(
                    exposureInformationList
                        .Select(exposureInfo => new UserExposureInfo(exposureInfo))
                        .ToList()
                    );
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        public async Task<List<UserExposureInfo>> GetUserExposureInfosAsync(int offsetDays)
        {
            return (await GetUserExposureInfosAsync())
                .Where(exposureInfo => exposureInfo.Timestamp.CompareTo(DateTimeUtility.Instance.UtcNow.AddDays(offsetDays)) >= 0)
                .ToList();
        }

        public Task RemoveUserExposureInformationAsync()
        {
            _loggerService.StartMethod();

            try
            {
                _preferencesService.RemoveValue(KEY_EXPOSURE_SUMMARIES);
                _preferencesService.RemoveValue(KEY_EXPOSURE_INFORMATIONS);
                return Task.CompletedTask;
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        private Task<(List<ExposureSummary>, List<ExposureInformation>)> GetExposureInformationDataAsync()
        {
            try
            {
                string exposureSummariesJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_SUMMARIES, EMPTY_LIST_JSON);
                string exposureInformationListJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_INFORMATIONS, EMPTY_LIST_JSON);

                List<ExposureSummary> exposureSummaryList = JsonConvert.DeserializeObject<List<ExposureSummary>>(exposureSummariesJson);
                List<ExposureInformation> exposureInformationList = JsonConvert.DeserializeObject<List<ExposureInformation>>(exposureInformationListJson);

                return Task.FromResult((exposureSummaryList, exposureInformationList));
            }
            finally
            {
                _loggerService.EndMethod();
            }
        }

        #endregion
    }
}
