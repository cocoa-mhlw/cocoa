using Covid19Radar.Api.DataAccess;
using Covid19Radar.Api.Models;
using Covid19Radar.Background.Extentions;
using Covid19Radar.Background.Protobuf;
using Google.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public class TemporaryExposureKeyExportBatchService : ITemporaryExposureKeyExportBatchService
    {
        /// <summary>
        /// The maximum number of keys in the TEKExport file
        /// </summary>
        public const int MaxKeysPerFile = 250_000;
        const int FixedHeaderWidth = 16;
        const string Header = "EK Export v1    ";
        const string ExportBinFileName = "export.bin";
        const string ExportSigFileName = "export.sig";
        const string SequenceName = "BatchNum";

        private readonly byte[] FixedHeader = Encoding.UTF8.GetBytes(Header);

        public readonly ISequenceRepository Sequence;
        public readonly ITemporaryExposureKeyRepository TekRepository;
        public readonly ITemporaryExposureKeyExportRepository TekExportRepository;
        public readonly ITemporaryExposureKeySignService SignService;
        public readonly ITemporaryExposureKeySignatureInfoService SignatureService;
        public readonly ITemporaryExposureKeyBlobService BlobService;
        public readonly ILogger<TemporaryExposureKeyExportBatchService> Logger;
        private readonly string[] Regions;

        public TemporaryExposureKeyExportBatchService(
            IConfiguration config,
            ISequenceRepository sequence,
            ITemporaryExposureKeyRepository tek,
            ITemporaryExposureKeyExportRepository tekExport,
            ITemporaryExposureKeySignService signService,
            ITemporaryExposureKeySignatureInfoService signatureService,
            ITemporaryExposureKeyBlobService blobService,
            ILogger<TemporaryExposureKeyExportBatchService> logger)
        {
            Logger = logger;
            Logger.LogInformation($"{nameof(TemporaryExposureKeyExportBatchService)} constructor");
            Sequence = sequence;
            TekRepository = tek;
            TekExportRepository = tekExport;
            SignService = signService;
            SignatureService = signatureService;
            BlobService = blobService;
            Regions = config.SupportRegions();
        }

        public async Task RunAsync()
        {
            try
            {
                Logger.LogInformation($"start {nameof(RunAsync)}");
                var batchTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var items = await TekRepository.GetNextAsync();
                foreach (var kv in items.GroupBy(_ => new
                {
                    RollingStartUnixTimeSeconds = _.GetRollingStartUnixTimeSeconds(),
                    RollingPeriodSeconds = _.GetRollingPeriodSeconds()
                }))
                {
                    var batchNum = (int)await Sequence.GetNextAsync(SequenceName, 1);

                    foreach (var region in Regions)
                    {
                        // Security considerations: Random Order TemporaryExposureKey
                        var sorted = kv
                            .OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue));
                        await CreateAsync((ulong)kv.Key.RollingStartUnixTimeSeconds,
                            (ulong)(kv.Key.RollingStartUnixTimeSeconds + kv.Key.RollingPeriodSeconds),
                            region,
                            batchNum,
                            batchTimestamp,
                            sorted.ToArray());
                    }

                    foreach (var key in kv)
                    {
                        key.Exported = true;
                        await TekRepository.UpsertAsync(key);
                    }
                }

                // Write Export Files json
                var models = await TekExportRepository.GetKeysAsync(0);
                await BlobService.WriteFilesJsonAsync(models);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error on {nameof(TemporaryExposureKeyExportBatchService)}");
                throw;
            }
        }

        public async Task<TEKSignature> CreateSignatureAsync(MemoryStream source, int batchNum, int batchSize)
        {
            Logger.LogInformation($"start {nameof(CreateSignatureAsync)}");
            var s = new TEKSignature();
            s.BatchNum = batchNum;
            s.BatchSize = batchSize;
            // Signature
            s.Signature = ByteString.CopyFrom(await SignService.SignAsync(source));
            return s;
        }

        public async Task CreateAsync(ulong startTimestamp,
                                      ulong endTimestamp,
                                      string region,
                                      int batchNum,
                                      long batchTimestamp,
                                      IEnumerable<TemporaryExposureKeyModel> keys)
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

                var exportModel = new TemporaryExposureKeyExportModel();
                exportModel.id = batchNum.ToString();
                exportModel.PartitionKey = region;
                exportModel.BatchNum = batchNum;
                exportModel.Region = region;
                exportModel.BatchSize = exportKeyModels.Length;
                exportModel.StartTimestamp = startTimestamp;
                exportModel.EndTimestamp = endTimestamp;
                exportModel.TimestampSecondsSinceEpoch = batchTimestamp;
                exportModel = await TekExportRepository.CreateAsync(exportModel);

                var bin = new TemporaryExposureKeyExport();
                bin.Keys.AddRange(exportKeys);
                // TODO: not support apple
                bin.BatchNum = 1;
                bin.BatchSize = 1;
                bin.Region = exportModel.Region;
                bin.StartTimestamp = exportModel.StartTimestamp;
                bin.EndTimestamp = exportModel.EndTimestamp;
                bin.SignatureInfos.Add(signatureInfo);

                var sig = new TEKSignatureList();

                using var binStream = new MemoryStream();
                await binStream.WriteAsync(FixedHeader, 0, FixedHeaderWidth);
                using var binStreamCoded = new CodedOutputStream(binStream, true);
                bin.WriteTo(binStreamCoded);
                binStreamCoded.Flush();
                await binStream.FlushAsync();
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
