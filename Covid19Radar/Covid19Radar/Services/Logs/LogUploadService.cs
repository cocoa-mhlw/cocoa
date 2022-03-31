/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Net;
using System.Threading.Tasks;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Newtonsoft.Json;

namespace Covid19Radar.Services.Logs
{
    public class LogUploadService : ILogUploadService
    {
        private readonly ILoggerService loggerService;
        private readonly ILogPathService logPathService;
        private readonly IStorageService storageService;
        private readonly IHttpClientService httpClientService;
        private readonly IServerConfigurationRepository serverConfigurationRepository;

        public LogUploadService(
            ILoggerService loggerService,
            ILogPathService logPathService,
            IStorageService storageService,
            IHttpClientService httpClientService,
            IServerConfigurationRepository serverConfigurationRepository
            )
        {
            this.loggerService = loggerService;
            this.logPathService = logPathService;
            this.storageService = storageService;
            this.httpClientService = httpClientService;
            this.serverConfigurationRepository = serverConfigurationRepository;
        }

        public async Task<ApiResponse<LogStorageSas>> GetLogStorageSas()
        {
            loggerService.StartMethod();

            int statusCode;
            LogStorageSas logStorageSas = default;

            try
            {
                await serverConfigurationRepository.LoadAsync();

                var url = serverConfigurationRepository.InquiryLogApiUrl;

                var response = await httpClientService.ApiClient.GetAsync(url);

                statusCode = (int)response.StatusCode;
                loggerService.Info($"Response status: {statusCode}");

                if (statusCode == (int)HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    logStorageSas = JsonConvert.DeserializeObject<LogStorageSas>(content);
                }

                return new ApiResponse<LogStorageSas>(statusCode, logStorageSas);
            }
            finally
            {
                loggerService.EndMethod();
            }
        }

        public async Task<HttpStatusCode> UploadAsync(string zipFilePath, string sasToken)
        {
            loggerService.StartMethod();

            try
            {
                // Upload to storage.
                var logTmpPath = logPathService.LogUploadingTmpPath;

                var setting = AppSettings.Instance;
                var endpoint = setting.LogStorageEndpoint;
                var uploadPath = setting.LogStorageContainerName;
                var accountName = setting.LogStorageAccountName;

                return await storageService.UploadAsync(endpoint, uploadPath, accountName, sasToken, zipFilePath);
            }
            finally
            {
                loggerService.EndMethod();
            }
        }
    }
}
