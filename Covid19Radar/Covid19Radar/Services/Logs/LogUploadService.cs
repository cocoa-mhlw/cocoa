/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Covid19Radar.Services.Logs
{
    public class LogUploadService : ILogUploadService
    {
        private readonly IHttpDataService httpDataService;
        private readonly ILoggerService loggerService;
        private readonly ILogPathService logPathService;
        private readonly IStorageService storageService;

        public LogUploadService(
            IHttpDataService httpDataService,
            ILoggerService loggerService,
            ILogPathService logPathService,
            IStorageService storageService)
        {
            this.httpDataService = httpDataService;
            this.loggerService = loggerService;
            this.logPathService = logPathService;
            this.storageService = storageService;
        }

        public async Task<bool> UploadAsync(string zipFileName)
        {
            loggerService.StartMethod();

            var result = false;

            try
            {
                // Get the storage SAS Token for upload.
                var logStorageSasResponse = await httpDataService.GetLogStorageSas();
                if (logStorageSasResponse.StatusCode != (int)HttpStatusCode.OK)
                {
                    throw new Exception("Status is error.");
                }
                if (string.IsNullOrEmpty(logStorageSasResponse.Result.SasToken))
                {
                    throw new Exception("Storage SAS Token is null or empty.");
                }

                // Upload to storage.
                var logTmpPath = logPathService.LogUploadingTmpPath;
                var logZipPath = Path.Combine(logTmpPath, zipFileName);

                var setting = AppSettings.Instance;
                var endpoint = setting.LogStorageEndpoint;
                var uploadPath = setting.LogStorageContainerName;
                var accountName = setting.LogStorageAccountName;
                var sasToken = logStorageSasResponse.Result.SasToken;

                var uploadResult = await storageService.UploadAsync(endpoint, uploadPath, accountName, sasToken, logZipPath);
                if (!uploadResult)
                {
                    throw new Exception("Failed to upload to storage.");
                }

                result = true;
            }
            catch (Exception ex)
            {
                loggerService.Exception("Failed upload.", ex);
            }

            loggerService.EndMethod();
            return result;
        }
    }
}
