using Covid19Radar.Models;
using Covid19Radar.Protobuf;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Covid19Radar.Services
{
    public class TemporaryExposureKeyService : ITemporaryExposureKeyService
    {
        public const int MaxKeysPerFile = 17_000;

        public readonly string AppBundleId;
        public readonly string AndroidPackage;
        public readonly string SignatureAlgorithm;
        public readonly string[] VerificationKeyIds;
        public readonly string[] VerificationKeyVersions;
        public readonly byte[] Signature;
        public readonly string Region;
        public readonly SignatureInfo SigInfo;

        public TemporaryExposureKeyService(IConfiguration config)
        {
            AppBundleId = config["AppBundleId"];
            AndroidPackage = config["AndroidPackage"];
            SignatureAlgorithm = config["SignatureAlgorithm"];
            VerificationKeyIds = config["VerificationKeyIds"].Split(' ');
            VerificationKeyVersions = config["VerificationKeyVersions"].Split(' ');
            Signature = Convert.FromBase64String(config["Signature"]);
            Region = config["Region"];
            var sig = new SignatureInfo();
            sig.AppBundleId = AppBundleId;
            sig.AndroidPackage = AndroidPackage;
            sig.SignatureAlgorithm = SignatureAlgorithm;
            sig.VerificationKeyId = VerificationKeyIds.Last();
            sig.VerificationKeyVersion = VerificationKeyVersions.Last();
            SigInfo = sig;               
        }

        public TEKSignature CreateSignature(int batchNum, int batchSize)
        {
            var s = new TEKSignature();
            s.SignatureInfo = SigInfo;
            s.BatchNum = batchNum;
            s.BatchSize = batchSize;
            s.Signature = ByteString.CopyFrom(Signature);
            return s;
        }

        public IEnumerator<TemporaryExposureKeyExportModel> Create(
            ulong startTimestamp, ulong endTimestamp, IEnumerable<TemporaryExposureKey> keys)
        {
            var current = keys;
            var num = 1;
            while (current.Any())
            {
                var exportKeys = current.Take(MaxKeysPerFile);
                current = current.Skip(MaxKeysPerFile);

                var bin = new TemporaryExposureKeyExport();
                bin.Keys.AddRange(keys);

                bin.BatchNum = num++;
                bin.BatchSize = bin.Keys.Count;
                bin.Region = Region;
                bin.StartTimestamp = startTimestamp;
                bin.EndTimestamp = endTimestamp;

                var signature = CreateSignature(bin.BatchNum, bin.BatchSize);
                bin.SignatureInfos.Add(signature.SignatureInfo);

                var sig = new Covid19Radar.Protobuf.TEKSignatureList();
                sig.Signatures.Add(signature);

                var model = new TemporaryExposureKeyExportModel();
                model.Url = WriteToBlob(bin, sig);
                model.startTimestamp = startTimestamp;
                model.endTimestamp = endTimestamp;

                yield return model;
            }

        }

        public string WriteToBlob(TemporaryExposureKeyExport bin, TEKSignatureList sig)
        {
            // TODO: write to blob storage
            return "URL";
        }

    }
}
