using Covid19Radar.Background.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public class TemporaryExposureKeySignService : ITemporaryExposureKeySignService
    {
        public readonly string TekExportKeyVaultKeyUrl;
        public readonly ILogger<TemporaryExposureKeySignService> Logger;
        public readonly string VerificationKeyId;
        public readonly string VerificationKeyVersion;
        public readonly string VerificationKeySecret;
        public readonly ECDsa VerificationKey;

        public TemporaryExposureKeySignService(
            IConfiguration config,
            ILogger<TemporaryExposureKeySignService> logger)
        {
            Logger = logger;
            Logger.LogInformation($"{nameof(TemporaryExposureKeySignService)} constructor");
            TekExportKeyVaultKeyUrl = config.TekExportKeyVaultKeyUrl();
            VerificationKeyId = config.VerificationKeyId();
            VerificationKeyVersion = config.VerificationKeyVersion();
            VerificationKeySecret = config.VerificationKeySecret();

            var key = CngKey.Import(Convert.FromBase64String(VerificationKeySecret), CngKeyBlobFormat.Pkcs8PrivateBlob);
            VerificationKey = new ECDsaCng(key);
        }


        public Task<byte[]> SignAsync(Stream source)
        {
            Logger.LogInformation($"start {nameof(SignAsync)}");

            return Task.FromResult(VerificationKey.SignData(source, HashAlgorithmName.SHA256));
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
