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

        public async Task StartExposureNotificationAsync()
        {
            _loggerService.StartMethod();

            try
            {
                var enabled = await IsEnabledAsync();
                if (enabled)
                {
                    return;
                }
                await StartAsync();
            }
            finally
            {
                _loggerService.EndMethod();

            }
        }

    }
}
