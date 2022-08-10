/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Models;
using Covid19Radar.Background.Models;
using Covid19Radar.Background.Protobuf;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace Covid19Radar.Background.Services
{
    public class TemporaryExposureKeyBlobService : ITemporaryExposureKeyBlobService
    {
        const string batchNumberMetadataKey = "batch_number";
        const string batchRegionMetadataKey = "batch_region";
        const string fileNameSuffix = ".zip";

        public readonly string TekExportKeyUrl;
        public readonly string TekExportBlobStorageConnectionString;
        public readonly string TekExportBlobStorageContainerPrefix;
        public readonly BlobServiceClient BlobServiceClient;
        public readonly ILogger<TemporaryExposureKeyBlobService> Logger;

        public TemporaryExposureKeyBlobService(
            IConfiguration config,
            ILogger<TemporaryExposureKeyBlobService> logger)
        {
            Logger = logger;
            Logger.LogInformation($"{nameof(TemporaryExposureKeyBlobService)} constructor");
            TekExportKeyUrl = config.TekExportKeyUrl();
            TekExportBlobStorageConnectionString = config.TekExportBlobStorage();
            TekExportBlobStorageContainerPrefix = config.TekExportBlobStorageContainerPrefix();
            BlobServiceClient = new BlobServiceClient(TekExportBlobStorageConnectionString);
        }

        public async Task WriteToBlobAsync(Stream s, TemporaryExposureKeyExportModel model, TemporaryExposureKeyExport bin, TEKSignatureList sig)
        {
            Logger.LogInformation($"start {nameof(WriteToBlobAsync)}");
            // skip if completed upload
            if (model.Uploaded) return;
            //  write to blob storage
            var blobContainerName = $"{TekExportBlobStorageContainerPrefix}".ToLower();
            var blobContainerClient = BlobServiceClient.GetBlobContainerClient(blobContainerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
            var blobNamePrefix = $"{model.Region}".ToLower();
            // Filename is inferable as batch number
            var exportFileName = $"{blobNamePrefix}/{model.BatchNum}{fileNameSuffix}";
            var blob = blobContainerClient.GetBlobClient(exportFileName);

            // Set the batch number and region as metadata
            var metaData = new Dictionary<string, string> {
                {batchNumberMetadataKey, model.BatchNum.ToString()},
                {batchRegionMetadataKey, model.Region}
            };      

            await blob.UploadAsync(s, new BlobHttpHeaders { ContentType = MediaTypeNames.Application.Zip });
            Logger.LogInformation($" {nameof(WriteToBlobAsync)} upload {exportFileName}");
            await blob.SetMetadataAsync(metaData);
            Logger.LogInformation($" {nameof(WriteToBlobAsync)} set metadata {exportFileName}");
            // set upload
            model.Uploaded = true;
        }

        public async Task DeleteAsync(TemporaryExposureKeyExportModel model)
        {
            Logger.LogInformation($"start {nameof(DeleteAsync)}");
            var blobContainerName = $"{TekExportBlobStorageContainerPrefix}".ToLower();
            var blobContainerClient = BlobServiceClient.GetBlobContainerClient(blobContainerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
            var blobNamePrefix = $"{model.Region}".ToLower();
            var exportFileName = $"{blobNamePrefix}/{model.BatchNum}{fileNameSuffix}";
            var blob = blobContainerClient.GetBlobClient(exportFileName);

            await blob.DeleteIfExistsAsync();
        }

        public async Task WriteFilesJsonAsync(IEnumerable<TemporaryExposureKeyExportModel> models, string[] supportRegions)
        {
            Logger.LogInformation($"start {nameof(WriteFilesJsonAsync)}");
            var blobContainerName = $"{TekExportBlobStorageContainerPrefix}".ToLower();
            var blobContainerClient = BlobServiceClient.GetBlobContainerClient(blobContainerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

            var postedGroupedRegions = models.GroupBy(_ => _.Region);

            foreach (var region in supportRegions)
            {
                var blobNamePrefix = $"{region}".ToLower();
                var exportFileName = $"{blobNamePrefix}/list.json";
                var blob = blobContainerClient.GetBlobClient(exportFileName);

                var grp = postedGroupedRegions?.FirstOrDefault(_ => _.Key == region);

                var filesJson = "[]";
                if (grp != null)
                {
                    var files = grp.Select(_ => new TemporaryExposureKeyExportFileModel()
                    {
                        Region = region,
                        Created = _.TimestampSecondsSinceEpoch,
                        Url = $"{TekExportKeyUrl}/{TekExportBlobStorageContainerPrefix}/{region}/{_.BatchNum}.zip"
                    }).ToArray();

                    filesJson = JsonConvert.SerializeObject(files);
                }


                using (var stream = new MemoryStream())
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(filesJson);
                    await writer.FlushAsync();
                    await stream.FlushAsync();
                    stream.Position = 0;
                    await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = MediaTypeNames.Application.Json });
                }
            }

        }

    }
}
