using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;


namespace Covid19Radar.Api.Services
{
    public class InquiryLogBlobService : IInquiryLogBlobService
    {
        private readonly IConfiguration Config;
        private readonly ILogger<InquiryLogBlobService> Logger;
        private readonly BlobSasBuilder BlobSasBuilder;

        public InquiryLogBlobService(IConfiguration config,
            ILogger<InquiryLogBlobService> logger,
            BlobSasBuilder blobSasBuilder = null)
        {
            Config = config;
            Logger = logger;
            Logger.LogInformation($"{nameof(InquiryLogBlobService)} constructor");
            BlobSasBuilder = blobSasBuilder ?? new BlobSasBuilder();
        }
        

        public string GetServiceSASToken()
        {
            Logger.LogInformation($"{nameof(InquiryLogBlobService)} {nameof(GetServiceSASToken)}");
            BlobSasBuilder.BlobContainerName = Config.InquiryLogBlobStorageContainerPrefix().ToLower();
            BlobSasBuilder.Resource = "c";
            BlobSasBuilder.StartsOn = DateTimeOffset.UtcNow;
            BlobSasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10);
            BlobSasBuilder.SetPermissions(BlobContainerSasPermissions.Create);

            var credential = new StorageSharedKeyCredential(Config.InquiryLogBlobAccountName(), Config.InquiryLogBlobAccountKey());
            var sasToken = BlobSasBuilder.ToSasQueryParameters(credential).ToString();

            return sasToken;
        }
    }
}
