using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

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

        public async Task<long> GetLastProcessDiagnosisKeyTimestampAsync(string region)
        {
            _loggerService.StartMethod();

            await _semaphore.WaitAsync();

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

                return result;
            }
            finally
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        public async Task SetLastProcessDiagnosisKeyTimestampAsync(string region, long timestamp)
        {
            _loggerService.StartMethod();

            await _semaphore.WaitAsync();

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

                _loggerService.EndMethod();
            }
            finally
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        public async Task RemoveLastProcessDiagnosisKeyTimestampAsync()
        {
            _loggerService.StartMethod();

            await _semaphore.WaitAsync();

            try
            {
                _preferencesService.RemoveValue(KEY_LAST_PROCESS_DIAGNOSIS_KEY_TIMESTAMP);
            }
            catch
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        public async Task RemoveExposureInformationAsync()
        {
            _loggerService.StartMethod();

            await _semaphore.WaitAsync();

            try
            {
                _preferencesService.RemoveValue(KEY_EXPOSURE_SUMMARIES);
                _preferencesService.RemoveValue(KEY_EXPOSURE_INFORMATIONS);
            }
            finally
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        public async Task<(IList<DailySummary>, IList<ExposureWindow>)> GetExposureWindowDataAsync()
        {
            _loggerService.StartMethod();

            await _semaphore.WaitAsync();

            try
            {
                string dailySummaryListJson = _preferencesService.GetValue<string>(KEY_DAILY_SUMMARIES, null);
                string exposureWindowListJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_WINDOWS, null);

                if (dailySummaryListJson is null || exposureWindowListJson is null)
                {
                    return (new List<DailySummary>(), new List<ExposureWindow>());
                }

                List<DailySummary> dailySummaryList = JsonConvert.DeserializeObject<List<DailySummary>>(dailySummaryListJson);
                List<ExposureWindow> exposureWindowList = JsonConvert.DeserializeObject<List<ExposureWindow>>(exposureWindowListJson);

                dailySummaryList.Sort((a, b) => a.DateMillisSinceEpoch.CompareTo(b.DateMillisSinceEpoch));
                exposureWindowList.Sort((a, b) => a.DateMillisSinceEpoch.CompareTo(b.DateMillisSinceEpoch));

                return (dailySummaryList, exposureWindowList);
            }
            finally
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        public async Task<(IList<DailySummary>, IList<ExposureWindow>)> GetExposureWindowDataAsync(int offsetDays)
        {
            var (summaries, exposureWindows) = await GetExposureWindowDataAsync();
            return (
                summaries
                    .Where(dailySummary => dailySummary.GetDateTime().CompareTo(DateTimeUtility.Instance.UtcNow.AddDays(offsetDays)) >= 0)
                .ToList(),
                exposureWindows
                    .Where(exposureWindow => exposureWindow.GetDateTime().CompareTo(DateTimeUtility.Instance.UtcNow.AddDays(offsetDays)) >= 0)
                .ToList());
        }

        public async Task<(IList<UserExposureSummary>, IList<UserExposureInfo>)> GetUserExposureDataAsync()
        {
            _loggerService.StartMethod();

            await _semaphore.WaitAsync();

            try
            {
                string exposureSummaryListJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_SUMMARIES, null);
                string exposureInformationListJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_INFORMATIONS, null);

                if (exposureSummaryListJson is null || exposureInformationListJson is null)
                {
                    return (new List<UserExposureSummary>(), new List<UserExposureInfo>());
                }

                List<ExposureSummary> exposureSummaryList = JsonConvert.DeserializeObject<List<ExposureSummary>>(exposureSummaryListJson);
                List<ExposureInformation> exposureInformationList = JsonConvert.DeserializeObject<List<ExposureInformation>>(exposureInformationListJson);

                List<UserExposureSummary> userExposureSumary = exposureSummaryList
                    .Select(exposureSummary => new UserExposureSummary(exposureSummary))
                    .ToList();
                List<UserExposureInfo> userExposureInfoList = exposureInformationList
                    .Select(exposureInfo => new UserExposureInfo(exposureInfo))
                    .ToList();

                return (userExposureSumary, userExposureInfoList);
            }
            finally
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        public async Task<(IList<UserExposureSummary>, IList<UserExposureInfo>)> GetUserExposureDataAsync(int offsetDays)
        {
            var (summaries, infomations) = await GetUserExposureDataAsync();
            return (summaries,
                infomations
                .Where(info => info.Timestamp.CompareTo(DateTimeUtility.Instance.UtcNow.AddDays(offsetDays)) >= 0)
                .ToList());
        }

        public async Task<bool> AppendExposureDataAsync(
            ExposureSummary exposureSummary,
            IList<ExposureInformation> exposureInformationList,
            int minimumRiskScore
            )
        {
            var (existExposureSummaryList, existExposureInformationList) = GetExposureInformationData();
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

        public async Task AppendExposureDataAsync(
            IList<DailySummary> dailySummaryList,
            IList<ExposureWindow> exposueWindowList
            )
        {
            var (existDailySummaryList, existExposureWindowList) = GetExposureWindowData();
            existDailySummaryList.AddRange(dailySummaryList);
            existExposureWindowList.AddRange(exposueWindowList);

            await SetExposureDataAsync(existDailySummaryList, existExposureWindowList);
        }

        private async Task SetExposureDataAsync(IList<ExposureSummary> exposureSummaryList, IList<ExposureInformation> exposureInformationList)
        {
            _loggerService.StartMethod();

            string exposureSummaryListJson = JsonConvert.SerializeObject(exposureSummaryList);
            string exposureInformationListJson = JsonConvert.SerializeObject(exposureInformationList);

            await _semaphore.WaitAsync();

            try
            {
                _preferencesService.SetValue(KEY_EXPOSURE_SUMMARIES, exposureSummaryListJson);
                _preferencesService.SetValue(KEY_EXPOSURE_INFORMATIONS, exposureInformationListJson);
            }
            finally
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        private async Task SetExposureDataAsync(IList<DailySummary> dailySummaryList, IList<ExposureWindow> exposureWindowList)
        {
            _loggerService.StartMethod();

            string dailySummaryListJson = JsonConvert.SerializeObject(dailySummaryList);
            string exposureWindowListJson = JsonConvert.SerializeObject(exposureWindowList);

            await _semaphore.WaitAsync();

            try
            {
                _preferencesService.SetValue(KEY_DAILY_SUMMARIES, dailySummaryListJson);
                _preferencesService.SetValue(KEY_EXPOSURE_WINDOWS, exposureWindowListJson);
            }
            finally
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        private (List<ExposureSummary>, List<ExposureInformation>) GetExposureInformationData()
        {
            string exposureSummariesJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_SUMMARIES, EMPTY_LIST_JSON);
            string exposureInformationListJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_INFORMATIONS, EMPTY_LIST_JSON);

            List<ExposureSummary> exposureSummaryList = JsonConvert.DeserializeObject<List<ExposureSummary>>(exposureSummariesJson);
            List<ExposureInformation> exposureInformationList = JsonConvert.DeserializeObject<List<ExposureInformation>>(exposureInformationListJson);

            return (exposureSummaryList, exposureInformationList);
        }

        private (List<DailySummary>, List<ExposureWindow>) GetExposureWindowData()
        {
            string dailySummariesJson = _preferencesService.GetValue<string>(KEY_DAILY_SUMMARIES, EMPTY_LIST_JSON);
            string exposureWindowListJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_WINDOWS, EMPTY_LIST_JSON);

            List<DailySummary> dailySummaryList = JsonConvert.DeserializeObject<List<DailySummary>>(dailySummariesJson);
            List<ExposureWindow> exposureWindowList = JsonConvert.DeserializeObject<List<ExposureWindow>>(exposureWindowListJson);

            return (dailySummaryList, exposureWindowList);
        }
    }
}
