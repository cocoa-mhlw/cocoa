using Covid19Radar.DataAccess;
using Covid19Radar.Models;
using Covid19Radar.Protobuf;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
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
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public class TemporaryExposureKeyService : ITemporaryExposureKeyService
    {
        public const int MaxKeysPerFile = 17_000;
        const int fixedHeaderWidth = 16;
        const string ExportBinFileName = "export.bin";
        const string ExportSigFileName = "export.sig";

        public readonly ITemporaryExposureKeyRepository TekRepository;
        public readonly ITemporaryExposureKeyExportRepository TekExportRepository;
        public readonly ILogger<TemporaryExposureKeyService> Logger;
        public readonly ITemporaryExposureKeySignService SignService;
        public readonly ITemporaryExposureKeySignatureInfoService SignatureService;
        public readonly ITemporaryExposureKeyBlobService BlobService;

        public TemporaryExposureKeyService(IConfiguration config,
            ITemporaryExposureKeyRepository tek,
            ITemporaryExposureKeyExportRepository tekExport,
            ITemporaryExposureKeySignService signService,
            ITemporaryExposureKeySignatureInfoService signatureService,
            ITemporaryExposureKeyBlobService blobService,
            ILogger<TemporaryExposureKeyService> logger)
        {
            TekRepository = tek;
            TekExportRepository = tekExport;
            SignService = signService;
            SignatureService = signatureService;
            BlobService = blobService;
            Logger = logger;
        }

        public async Task<TEKSignature> CreateSignatureAsync(MemoryStream source, int batchNum, int batchSize)
        {
            var s = new TEKSignature();
            s.BatchNum = batchNum;
            s.BatchSize = batchSize;
            // Signature
            s.Signature = ByteString.CopyFrom(await SignService.SignAsync(source));
            return s;
        }

        public async Task RunAsync()
        {
            try
            {
                var items = await TekRepository.GetNextAsync();
                foreach (var kv in items.GroupBy(_ => new { _.RollingStartUnixTimeSeconds, _.RollingPeriodSeconds, _.Region }))
                {
                    await CreateAsync((ulong)kv.Key.RollingStartUnixTimeSeconds,
                        (ulong)(kv.Key.RollingStartUnixTimeSeconds + kv.Key.RollingPeriodSeconds),
                        kv.Key.Region,
                        kv.ToArray());
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error on {nameof(TemporaryExposureKeyService)}");
                throw;
            }
        }

        public async Task CreateAsync(ulong startTimestamp, ulong endTimestamp, string region, IEnumerable<TemporaryExposureKeyModel> keys)
        {
            var current = keys;
            while (current.Any())
            {
                var exportKeyModels = current.Take(MaxKeysPerFile).ToImmutableArray();
                var exportKeys = exportKeyModels.Select(_ => _.ToKey());
                current = current.Skip(MaxKeysPerFile);

                var exportModel = await TekExportRepository.CreateAsync();
                exportModel.BatchSize = exportKeyModels.Length;
                exportModel.StartTimestamp = startTimestamp;
                exportModel.EndTimestamp = endTimestamp;
                exportModel.Region = region;

                var signatureKey = await SignService.GetX509PublicKeyAsync();
                var signatureInfo = SignatureService.Create(signatureKey);

                var bin = new TemporaryExposureKeyExport();
                bin.Keys.AddRange(exportKeys);
                bin.BatchNum = exportModel.BatchNum;
                bin.BatchSize = exportModel.BatchSize;
                bin.Region = exportModel.Region;
                bin.StartTimestamp = exportModel.StartTimestamp;
                bin.EndTimestamp = exportModel.EndTimestamp;
                bin.SignatureInfos.Add(signatureInfo);

                var sig = new TEKSignatureList();

                using var binStream = new MemoryStream();
                bin.WriteTo(binStream);
                await binStream.FlushAsync();
                binStream.Seek(0, SeekOrigin.Begin);
                var signature = await CreateSignatureAsync(binStream, bin.BatchNum, bin.BatchSize);
                signature.SignatureInfo = signatureInfo;
                sig.Signatures.Add(signature);

                using (var s = new MemoryStream())
                {
                    using (var z = new System.IO.Compression.ZipArchive(s, System.IO.Compression.ZipArchiveMode.Create, true))
                    {
                        var binEntry = z.CreateEntry(ExportBinFileName);
                        using (var binFile = binEntry.Open())
                        {
                            binStream.Seek(0, SeekOrigin.Begin);
                            await binStream.CopyToAsync(binFile);
                            await binFile.FlushAsync();
                        }

                        var sigEntry = z.CreateEntry(ExportSigFileName);
                        using (var sigFile = sigEntry.Open())
                        {
                            sig.WriteTo(sigFile);
                            await sigFile.FlushAsync();
                        }
                    }
                    s.Seek(0, SeekOrigin.Begin);
                    await BlobService.WriteToBlobAsync(s, exportModel, bin, sig);
                }
                await TekExportRepository.UpdateAsync(exportModel);
            }
        }
    }
}
