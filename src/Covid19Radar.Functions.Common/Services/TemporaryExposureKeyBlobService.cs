using Covid19Radar.Models;
using Covid19Radar.Protobuf;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public class TemporaryExposureKeyBlobService : ITemporaryExposureKeyBlobService
    {
        const string batchNumberMetadataKey = "batch_number";
        const string batchRegionMetadataKey = "batch_region";
        const string fileNameSuffix = ".zip";

        public readonly string TekExportBlobStorageConnectionString;
        public readonly string TekExportBlobStorageContainerPrefix;
        public readonly CloudStorageAccount StorageAccount;
        public readonly CloudBlobClient BlobClient;

        public TemporaryExposureKeyBlobService(IConfiguration config)
        {
            TekExportBlobStorageConnectionString = config["TekExportBlobStorage"];
            TekExportBlobStorageContainerPrefix = config["TekExportBlobStorageContainerPrefix"];
            StorageAccount = CloudStorageAccount.Parse(TekExportBlobStorageConnectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        public async Task WriteToBlobAsync(Stream s, TemporaryExposureKeyExportModel model, TemporaryExposureKeyExport bin, TEKSignatureList sig)
        {
            //  write to blob storage
            var blobContainerName = $"{TekExportBlobStorageContainerPrefix}{model.Region}".ToLower();
            var cloudBlobContainer = BlobClient.GetContainerReference(blobContainerName);
            await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext());

            // Filename is inferable as batch number
            var exportFileName = $"{model.BatchNum}{fileNameSuffix}";
            var blockBlob = cloudBlobContainer.GetBlockBlobReference(exportFileName);

            // Set the batch number and region as metadata
            blockBlob.Metadata[batchNumberMetadataKey] = model.BatchNum.ToString();
            blockBlob.Metadata[batchRegionMetadataKey] = model.Region;

            await blockBlob.UploadFromStreamAsync(s);
            await blockBlob.SetMetadataAsync();
        }
    }
}
