/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Covid19Radar.Services.Logs;

namespace Covid19Radar.Services
{
    public interface IStorageService
    {
        Task<bool> UploadAsync(string endpoint, string uploadPath, string accountName, string sasToken, string sourceFilePath);
    }

    public class StorageService : IStorageService
    {
        private readonly ILoggerService loggerService;

        public StorageService(ILoggerService loggerService)
        {
            this.loggerService = loggerService;
        }

        public async Task<bool> UploadAsync(string endpoint, string uploadPath, string accountName, string sasToken, string sourceFilePath)
        {
            loggerService.StartMethod();

            var result = false;
            try
            {
                var fileName = Path.GetFileName(sourceFilePath);
                var uri = new UriBuilder(endpoint)
                {
                    Path = $"{uploadPath.Trim('/')}/{fileName}",
                    Query = sasToken.TrimStart('?')
                }.Uri;

                var client = new BlobClient(uri);
                using (var fileStream = File.OpenRead(sourceFilePath))
                {
                    var response = await client.UploadAsync(fileStream);
                    var rawResponse = response.GetRawResponse();
                    if (rawResponse.Status == (int)HttpStatusCode.Created)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                loggerService.Error("Failed upload to storage");
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
            }

            loggerService.EndMethod();
            return result;
        }
    }
}
