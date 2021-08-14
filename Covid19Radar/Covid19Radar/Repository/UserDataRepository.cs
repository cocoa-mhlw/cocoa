using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public class UserDataRepository : IUserDataRepository
    {
        private const string KEY_LAST_PROCESS_DIAGNOSIS_KEY_TIMESTAMP = "last_process_diagnosis_key_timestamp";

        private const string KEY_EXPOSURE_SUMMARY = "exposure_sumary";
        private const string KEY_EXPOSURE_INFORMATIONS = "exposure_informations";

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

        public async Task SetExposureDataAsync(ExposureSummary exposureSummary, IList<ExposureInformation> exposureInformationList)
        {
            _loggerService.StartMethod();

            string exposureSummaryJson = JsonConvert.SerializeObject(exposureSummary);
            string exposureInformationListJson = JsonConvert.SerializeObject(exposureInformationList);

            await _semaphore.WaitAsync();

            try
            {
                _preferencesService.SetValue(KEY_EXPOSURE_SUMMARY, exposureSummaryJson);
                _preferencesService.SetValue(KEY_EXPOSURE_INFORMATIONS, exposureInformationListJson);
            }
            finally
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
                _preferencesService.RemoveValue(KEY_EXPOSURE_SUMMARY);
                _preferencesService.RemoveValue(KEY_EXPOSURE_INFORMATIONS);
            }
            finally
            {
                _semaphore.Release();

                _loggerService.EndMethod();
            }
        }

        public async Task<(UserExposureSummary, IList<UserExposureInfo>)> GetUserExposureDataAsync()
        {
            _loggerService.StartMethod();

            await _semaphore.WaitAsync();

            try
            {
                string exposureSummaryJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_SUMMARY, null);
                string exposureInformationListJson = _preferencesService.GetValue<string>(KEY_EXPOSURE_INFORMATIONS, null);

                if (exposureSummaryJson is null || exposureInformationListJson is null)
                {
                    return (null, new List<UserExposureInfo>());
                }

                ExposureSummary exposureSummary = JsonConvert.DeserializeObject<ExposureSummary>(exposureSummaryJson);
                List<ExposureInformation> exposureInformationList = JsonConvert.DeserializeObject<List<ExposureInformation>>(exposureInformationListJson);

                UserExposureSummary userExposureSumary = new UserExposureSummary(exposureSummary);
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
    }
}
