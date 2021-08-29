using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Chino;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public abstract class AbsExposureNotificationApiService : AbsExposureNotificationClient
    {
        private readonly ILoggerService _loggerService;

        public AbsExposureNotificationApiService(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public async Task<bool> StartExposureNotificationAsync()
        {
            _loggerService.StartMethod();

            try
            {
                var enabled = await IsEnabledAsync();
                if (enabled)
                {
                    return false;
                }
                await StartAsync();
            }
            catch(Exception ex)
            {
                _loggerService.Exception("Failed to start exposure notification.", ex);
            }
            finally
            {
                _loggerService.EndMethod();
            }
            return true;
        }

        public async Task<bool> StopExposureNotificationAsync()
        {
            _loggerService.StartMethod();

            try
            {
                var enabled = await IsEnabledAsync();
                if (!enabled)
                {
                    return false;
                }
                await StopAsync();
                return true;

            }
            finally
            {
                _loggerService.EndMethod();
            }
        }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public class MockExposureNotificationApiService : AbsExposureNotificationApiService
    {
        private const int DUMMY_VERSION = 2;

        private bool _isEnabled = false;

        public MockExposureNotificationApiService(ILoggerService loggerService)
            : base(loggerService)
        {
        }

        public override async Task<IList<ExposureNotificationStatus>> GetStatusesAsync()
        {
            IList<ExposureNotificationStatus> emptyList = new List<ExposureNotificationStatus>();
            return emptyList;
        }

        public override async Task<List<TemporaryExposureKey>> GetTemporaryExposureKeyHistoryAsync()
        {
            List<TemporaryExposureKey> emptyList = new List<TemporaryExposureKey>();
            return emptyList;
        }

        public override async Task<long> GetVersionAsync()
        {
            return DUMMY_VERSION;
        }

        public override async Task<bool> IsEnabledAsync()
        {
            return _isEnabled;
        }

        public override async Task ProvideDiagnosisKeysAsync(List<string> keyFiles)
        {
        }

        public override async Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration)
        {
        }

        public override async Task ProvideDiagnosisKeysAsync(List<string> keyFiles, ExposureConfiguration configuration, string token)
        {
        }

        public override async Task RequestPreAuthorizedTemporaryExposureKeyHistoryAsync()
        {
        }

        public override async Task RequestPreAuthorizedTemporaryExposureKeyReleaseAsync()
        {
        }

        public override async Task StartAsync()
        {
            _isEnabled = true;
        }

        public override async Task StopAsync()
        {
            _isEnabled = false;
        }
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}
