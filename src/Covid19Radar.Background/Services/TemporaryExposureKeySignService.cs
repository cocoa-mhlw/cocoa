using Covid19Radar.Background.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public class TemporaryExposureKeySignService : ITemporaryExposureKeySignService
    {
        public readonly ILogger<TemporaryExposureKeySignService> Logger;
        public readonly string VerificationKeyId;
        public readonly string VerificationKeyVersion;
        public readonly string VerificationKeySecret;
        public readonly ECDsa VerificationKey;
        public readonly X509SignatureGenerator VerificationKeyGenerator;

        public TemporaryExposureKeySignService(
            IConfiguration config,
            ILogger<TemporaryExposureKeySignService> logger)
        {
            Logger = logger;
            Logger.LogInformation($"{nameof(TemporaryExposureKeySignService)} constructor");
            VerificationKeyId = config.VerificationKeyId();
            VerificationKeyVersion = config.VerificationKeyVersion();
            VerificationKeySecret = config.VerificationKeySecret();

            //var key = CngKey.Import(Convert.FromBase64String(VerificationKeySecret), CngKeyBlobFormat.Pkcs8PrivateBlob);
            //VerificationKey = new ECDsaCng(key);
            var key = Convert.FromBase64String(VerificationKeySecret);
            VerificationKey = ECDsa.Create();
            int read;
            VerificationKey.ImportECPrivateKey(key, out read);
            VerificationKeyGenerator = System.Security.Cryptography.X509Certificates.X509SignatureGenerator.CreateForECDsa(VerificationKey);
        }


        public async Task<byte[]> SignAsync(MemoryStream source)
        {
            Logger.LogInformation($"start {nameof(SignAsync)}");
            var pos = source.Position;
            source.Position = 0;
            var signature = VerificationKey.SignData(source, HashAlgorithmName.SHA256);
            source.Position = 0;
            var signatureCert = VerificationKeyGenerator.SignData(source.ToArray(), HashAlgorithmName.SHA256);
            source.Position = pos;
            return signatureCert;

        }

        public Task SetSignatureAsync(SignatureInfo info)
        {
            Logger.LogInformation($"start {nameof(SetSignatureAsync)}");
            info.VerificationKeyId = VerificationKeyId;
            info.VerificationKeyVersion = VerificationKeyVersion;
            return Task.CompletedTask;
        }

    }
}
