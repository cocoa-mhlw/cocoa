using Covid19Radar.Models;
using Covid19Radar.Protobuf;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
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
        public readonly ILogger<TemporaryExposureKeyBlobService> Logger;

        public TemporaryExposureKeyBlobService(
            IConfiguration config,
            ILogger<TemporaryExposureKeyBlobService> logger)
        {
            TekExportBlobStorageConnectionString = config["TekExportBlobStorage"];
            TekExportBlobStorageContainerPrefix = config["TekExportBlobStorageContainerPrefix"];
            Logger = logger;
            StorageAccount = CloudStorageAccount.Parse(TekExportBlobStorageConnectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        public async Task WriteToBlobAsync(Stream s, TemporaryExposureKeyExportModel model, TemporaryExposureKeyExport bin, TEKSignatureList sig)
        {
            Logger.LogInformation($"start {nameof(WriteToBlobAsync)}");
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
            Logger.LogInformation($" {nameof(WriteToBlobAsync)} upload {exportFileName}");
            await blockBlob.SetMetadataAsync();
            Logger.LogInformation($" {nameof(WriteToBlobAsync)} set metadata {exportFileName}");
        }

        public async Task DeleteAsync(TemporaryExposureKeyExportModel model)
        {
            Logger.LogInformation($"start {nameof(DeleteAsync)}");
            var blobContainerName = $"{TekExportBlobStorageContainerPrefix}{model.Region}".ToLower();
            var cloudBlobContainer = BlobClient.GetContainerReference(blobContainerName);
            await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext());

            // Filename is inferable as batch number
            var exportFileName = $"{model.BatchNum}{fileNameSuffix}";
            var blockBlob = cloudBlobContainer.GetBlockBlobReference(exportFileName);

            await blockBlob.DeleteIfExistsAsync();
        }


    }
}
