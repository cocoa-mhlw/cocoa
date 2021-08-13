using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Newtonsoft.Json;

namespace Covid19Radar.Repository
{
    public class UserDataRepository : IUserDataRepository
    {
        private const string KEY_LAST_PROCESS_DIAGNOSIS_KEY_TIMESTAMP = "last_process_diagnosis_key_timestamp";

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
    }
}
