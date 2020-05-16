using Covid19Radar.DataAccess;
using Covid19Radar.Models;
using Covid19Radar.Protobuf;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public class TemporaryExposureKeyService : ITemporaryExposureKeyService
    {
        public const int MaxKeysPerFile = 17_000;

        const string ExportBinFileName = "export.bin";
        const string ExportSigFileName = "export.sig";
        const string batchNumberMetadataKey = "batch_number";
        const string batchRegionMetadataKey = "batch_region";

        public readonly string AppBundleId;
        public readonly string AndroidPackage;
        public readonly string SignatureAlgorithm;
        public readonly string[] VerificationKeyIds;
        public readonly string[] VerificationKeyVersions;
        public readonly byte[] Signature;
        public readonly string Region;
        public readonly SignatureInfo SigInfo;
        public readonly string TekExportBlobStorageConnectionString;
        public readonly string TekExportBlobStorageContainerPrefix;
        public readonly CloudStorageAccount StorageAccount;
        public readonly CloudBlobClient BlobClient;
        public readonly string BlobContainerName;
        public readonly ITemporaryExposureKeyRepository TekRepository;
        public readonly ITemporaryExposureKeyExportRepository TekExportRepository;
        public readonly ILogger<TemporaryExposureKeyService> Logger;

        public TemporaryExposureKeyService(IConfiguration config,
            ITemporaryExposureKeyRepository tek,
            ITemporaryExposureKeyExportRepository tekExport,
            ILogger<TemporaryExposureKeyService> logger) 
        {
            AppBundleId = config["AppBundleId"];
            AndroidPackage = config["AndroidPackage"];
            SignatureAlgorithm = config["SignatureAlgorithm"];
            VerificationKeyIds = config["VerificationKeyIds"].Split(' ');
            VerificationKeyVersions = config["VerificationKeyVersions"].Split(' ');
            Signature = Convert.FromBase64String(config["Signature"]);
            TekExportBlobStorageConnectionString = config["TekExportBlobStorage"];
            TekExportBlobStorageContainerPrefix = config["TekExportBlobStorageContainerPrefix"];
            Region = config["Region"];
            TekRepository = tek;
            TekExportRepository = tekExport;
            Logger = logger;
            var sig = new SignatureInfo();
            sig.AppBundleId = AppBundleId;
            sig.AndroidPackage = AndroidPackage;
            sig.SignatureAlgorithm = SignatureAlgorithm;
            sig.VerificationKeyId = VerificationKeyIds.LastOrDefault();
            sig.VerificationKeyVersion = VerificationKeyVersions.LastOrDefault();
            SigInfo = sig;
            StorageAccount = CloudStorageAccount.Parse(TekExportBlobStorageConnectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
            BlobContainerName = $"{TekExportBlobStorageContainerPrefix}{Region}".ToLower();

        }

        public TEKSignature CreateSignature(Stream source, int batchNum, int batchSize)
        {
            var s = new TEKSignature();
            s.SignatureInfo = SigInfo;
            s.BatchNum = batchNum;
            s.BatchSize = batchSize;
            // TODO: Signature
            s.Signature = ByteString.CopyFrom(Signature);
            return s;
        }

        public async Task RunAsync()
        {
            try
            {
                var items = await TekRepository.GetNextAsync();
                await CreateAsync(items);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error on {nameof(TemporaryExposureKeyService)}");
                throw;
            }
        }

        public async Task CreateAsync(IEnumerable<TemporaryExposureKeyModel> keys)
        {
            var current = keys;
            while (current.Any())
            {
                var exportKeyModels = current.Take(MaxKeysPerFile).ToImmutableArray();
                var exportKeys = exportKeyModels.Select(_ => _.ToKey());
                current = current.Skip(MaxKeysPerFile);

                var exportModel = await TekExportRepository.CreateAsync();
                exportModel.BatchSize = exportKeyModels.Length;
                exportModel.StartTimestamp = exportKeyModels.Min(_ => _.Timestamp);
                exportModel.EndTimestamp = exportKeyModels.Max(_ => _.Timestamp);
                exportModel.Region = Region;

                var bin = new TemporaryExposureKeyExport();
                bin.Keys.AddRange(exportKeys);
                bin.BatchNum = exportModel.BatchNum;
                bin.BatchSize = exportModel.BatchSize;
                bin.Region = exportModel.Region;
                bin.StartTimestamp = exportModel.StartTimestamp;
                bin.EndTimestamp = exportModel.EndTimestamp;
                bin.SignatureInfos.Add(SigInfo);

                var sig = new TEKSignatureList();

                using var binStream = new MemoryStream();
                bin.WriteTo(binStream);
                await binStream.FlushAsync();
                binStream.Seek(0, SeekOrigin.Begin);
                var signature = CreateSignature(binStream, bin.BatchNum, bin.BatchSize);
                sig.Signatures.Add(signature);
                binStream.Seek(0, SeekOrigin.Begin);

                using (var s = new MemoryStream())
                using (var z = new System.IO.Compression.ZipArchive(s, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    var binEntry = z.CreateEntry(ExportBinFileName);
                    using (var binFile = binEntry.Open())
                    {
                        await binStream.CopyToAsync(binFile);
                        await binFile.FlushAsync();
                    }

                    var sigEntry = z.CreateEntry(ExportSigFileName);
                    using (var sigFile = sigEntry.Open())
                    {
                        sig.WriteTo(sigFile);
                        await sigFile.FlushAsync();
                    }
                    s.Seek(0, SeekOrigin.Begin);
                    await WriteToBlobAsync(s, exportModel, bin, sig);
                }
                await TekExportRepository.UpdateAsync(exportModel);
            }
        }

        public async Task WriteToBlobAsync(Stream s, TemporaryExposureKeyExportModel model, TemporaryExposureKeyExport bin, TEKSignatureList sig)
        {
            //  write to blob storage
            var cloudBlobContainer = BlobClient.GetContainerReference(BlobContainerName);
            await cloudBlobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext());

            // Filename is inferable as batch number
            var exportFileName = $"{model.BatchNum}.tekexport";
            var blockBlob = cloudBlobContainer.GetBlockBlobReference(exportFileName);

            // Set the batch number and region as metadata
            blockBlob.Metadata[batchNumberMetadataKey] = model.BatchNum.ToString();
            blockBlob.Metadata[batchRegionMetadataKey] = model.Region;

            await blockBlob.UploadFromStreamAsync(s);
            await blockBlob.SetMetadataAsync();
        }

    }
}
