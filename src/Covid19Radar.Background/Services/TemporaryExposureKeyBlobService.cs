/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Api.Models;
using Covid19Radar.Background.Models;
using Covid19Radar.Background.Protobuf;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        public readonly CloudStorageAccount StorageAccount;
        public readonly CloudBlobClient BlobClient;
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
            StorageAccount = CloudStorageAccount.Parse(TekExportBlobStorageConnectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        private static string GetBlobDirectory(string region, string subRegion)
        {
            var blobDirectoryString = $"{region}";
            if (!string.IsNullOrEmpty(subRegion))
            {
                blobDirectoryString += $"/{subRegion}";
            }
            return blobDirectoryString.ToLower();
        }

        public async Task WriteToBlobAsync(Stream s, TemporaryExposureKeyExportModel model, TemporaryExposureKeyExport bin, TEKSignatureList sig)
        {
            Logger.LogInformation($"start {nameof(WriteToBlobAsync)}");
            // skip if completed upload
            if (model.Uploaded) return;
            //  write to blob storage
            var blobContainerName = $"{TekExportBlobStorageContainerPrefix}".ToLower();
            var cloudBlobContainer = BlobClient.GetContainerReference(blobContainerName);
            await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext());
            var blobDirectory = cloudBlobContainer.GetDirectoryReference(GetBlobDirectory(model.Region, model.SubRegion));

            // Filename is inferable as batch number
            var exportFileName = $"{model.BatchNum}{fileNameSuffix}";
            //var blockBlob = cloudBlobContainer.GetBlockBlobReference(exportFileName);
            var blockBlob = blobDirectory.GetBlockBlobReference(exportFileName);

            // Set the batch number and region as metadata
            blockBlob.Metadata[batchNumberMetadataKey] = model.BatchNum.ToString();
            blockBlob.Metadata[batchRegionMetadataKey] = model.Region;

            await blockBlob.UploadFromStreamAsync(s);
            Logger.LogInformation($" {nameof(WriteToBlobAsync)} upload {exportFileName}");
            await blockBlob.SetMetadataAsync();
            Logger.LogInformation($" {nameof(WriteToBlobAsync)} set metadata {exportFileName}");
            // set upload
            model.Uploaded = true;
        }

        public async Task DeleteAsync(TemporaryExposureKeyExportModel model)
        {
            Logger.LogInformation($"start {nameof(DeleteAsync)}");
            var blobContainerName = $"{TekExportBlobStorageContainerPrefix}".ToLower();
            var cloudBlobContainer = BlobClient.GetContainerReference(blobContainerName);
            await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext());
            var blobDirectory = cloudBlobContainer.GetDirectoryReference(GetBlobDirectory(model.Region, model.SubRegion));

            // Filename is inferable as batch number
            var exportFileName = $"{model.BatchNum}{fileNameSuffix}";
            var blockBlob = blobDirectory.GetBlockBlobReference(exportFileName);

            await blockBlob.DeleteIfExistsAsync();
        }

        public async Task WriteFilesJsonAsync(IEnumerable<TemporaryExposureKeyExportModel> models, string region, string subRegion)
        {
            Logger.LogInformation($"start {nameof(WriteFilesJsonAsync)}");
            var blobContainerName = $"{TekExportBlobStorageContainerPrefix}".ToLower();
            var cloudBlobContainer = BlobClient.GetContainerReference(blobContainerName);
            await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext());

            var urlPrefix = $"{TekExportKeyUrl}/{TekExportBlobStorageContainerPrefix}/{region}";
            if (!string.IsNullOrEmpty(subRegion))
            {
                urlPrefix += $"/{subRegion}";
            }

            var postedGroupedRegions = models.GroupBy(_ => _.Region);

            var blobDirectory = cloudBlobContainer.GetDirectoryReference(GetBlobDirectory(region, subRegion));
            var exportFileName = "list.json";
            var blockBlob = blobDirectory.GetBlockBlobReference(exportFileName);

            var grp = postedGroupedRegions?.FirstOrDefault(_ => _.Key == region);

            var filesJson = "[]";
            if (grp != null)
            {
                var files = grp.Select(_ => new TemporaryExposureKeyExportFileModel()
                {
                    Region = region,
                    Created = _.TimestampSecondsSinceEpoch,
                    Url = $"{urlPrefix}/{_.BatchNum}.zip"
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
                await blockBlob.UploadFromStreamAsync(stream);
            }

        }

    }
}
