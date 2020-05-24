using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Api.Protobuf;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public class TemporaryExposureKeyExportBatchService : ITemporaryExposureKeyExportBatchService
    {
        public const int MaxKeysPerFile = 25_000;
        const int FixedHeaderWidth = 16;
        const string Header = "EK Export v1    ";
        const string ExportBinFileName = "export.bin";
        const string ExportSigFileName = "export.sig";

        public readonly ITemporaryExposureKeyRepository TekRepository;
        public readonly ITemporaryExposureKeyExportRepository TekExportRepository;
        public readonly ITemporaryExposureKeySignService SignService;
        public readonly ITemporaryExposureKeySignatureInfoService SignatureService;
        public readonly ITemporaryExposureKeyBlobService BlobService;
        public readonly ILogger<TemporaryExposureKeyExportBatchService> Logger;

        public TemporaryExposureKeyExportBatchService(
            IConfiguration config,
            ITemporaryExposureKeyRepository tek,
            ITemporaryExposureKeyExportRepository tekExport,
            ITemporaryExposureKeySignService signService,
            ITemporaryExposureKeySignatureInfoService signatureService,
            ITemporaryExposureKeyBlobService blobService,
            ILogger<TemporaryExposureKeyExportBatchService> logger)
        {
            TekRepository = tek;
            TekExportRepository = tekExport;
            SignService = signService;
            SignatureService = signatureService;
            BlobService = blobService;
            Logger = logger;
        }

        public async Task RunAsync()
        {
            try
            {
                Logger.LogInformation($"start {nameof(RunAsync)}");
                var items = await TekRepository.GetNextAsync();
                foreach (var kv in items.GroupBy(_ => new { 
                    RollingStartUnixTimeSeconds = _.GetRollingStartUnixTimeSeconds(),
                    RollingPeriodSeconds = _.GetRollingPeriodSeconds(),
                    _.Region }))
                {
                    // Security considerations: Random Order TemporaryExposureKey
                    var sorted = kv.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue));
                    await CreateAsync((ulong)kv.Key.RollingStartUnixTimeSeconds,
                        (ulong)(kv.Key.RollingStartUnixTimeSeconds + kv.Key.RollingPeriodSeconds),
                        kv.Key.Region,
                        sorted.ToArray());
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error on {nameof(TemporaryExposureKeyExportBatchService)}");
                throw;
            }
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

        public async Task CreateAsync(ulong startTimestamp, ulong endTimestamp, string region, IEnumerable<TemporaryExposureKeyModel> keys)
        {
            Logger.LogInformation($"start {nameof(CreateAsync)}");
            var current = keys;
            while (current.Any())
            {
                var exportKeyModels = current.Take(MaxKeysPerFile).ToArray();
                var exportKeys = exportKeyModels.Select(_ => _.ToKey()).ToArray();
                current = current.Skip(MaxKeysPerFile);

                var signatureInfo = SignatureService.Create();
                await SignService.SetSignatureAsync(signatureInfo);

                var exportModel = await TekExportRepository.CreateAsync();
                exportModel.BatchSize = exportKeyModels.Length;
                exportModel.StartTimestamp = startTimestamp;
                exportModel.EndTimestamp = endTimestamp;
                exportModel.Region = region;
                exportModel.SignatureInfos = new SignatureInfo[] { signatureInfo };

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
                using var binStreamCoded = new CodedOutputStream(binStream);
                binStreamCoded.WriteBytes(ByteString.CopyFromUtf8(Header));
                bin.WriteTo(binStreamCoded);
                binStreamCoded.Flush();
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
                        using (var output = new CodedOutputStream(sigFile))
                        {
                            sig.WriteTo(output);
                            output.Flush();
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
