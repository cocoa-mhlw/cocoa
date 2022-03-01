/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Threading.Tasks;

namespace Covid19Radar.Services.Logs
{
    public class LogUploadService : ILogUploadService
    {
        private readonly ILoggerService loggerService;
        private readonly ILogPathService logPathService;
        private readonly IStorageService storageService;

        public LogUploadService(
            ILoggerService loggerService,
            ILogPathService logPathService,
            IStorageService storageService)
        {
            this.loggerService = loggerService;
            this.logPathService = logPathService;
            this.storageService = storageService;
        }

        public async Task<bool> UploadAsync(string zipFilePath, string sasToken)
        {
            loggerService.StartMethod();

            var result = false;

            try
            {
                // Upload to storage.
                var logTmpPath = logPathService.LogUploadingTmpPath;

                var setting = AppSettings.Instance;
                var endpoint = setting.LogStorageEndpoint;
                var uploadPath = setting.LogStorageContainerName;
                var accountName = setting.LogStorageAccountName;

                var uploadResult = await storageService.UploadAsync(endpoint, uploadPath, accountName, sasToken, zipFilePath);
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
