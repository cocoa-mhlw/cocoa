using Covid19Radar.Protobuf;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public class TemporaryExposureKeySignServiceDebug : ITemporaryExposureKeySignService
    {
        public readonly ILogger<TemporaryExposureKeySignServiceDebug> Logger;
        private System.Security.Cryptography.ECDsa Key;

        public TemporaryExposureKeySignServiceDebug(
            IConfiguration config,
            ILogger<TemporaryExposureKeySignServiceDebug> logger)
        {
            Logger = logger;
            Key = System.Security.Cryptography.ECDsaCng.Create(ECCurve.NamedCurves.nistP256);
        }

        public async Task<byte[]> SignAsync(Stream source)
        {
            Logger.LogInformation($"start {nameof(SignAsync)}");
            byte[] hash;
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                hash = sha.ComputeHash(source);
            }
            return Key.SignHash(hash);
        }

        public async Task SetSignatureAsync(SignatureInfo info)
        {
            Logger.LogInformation($"start {nameof(SetSignatureAsync)}");
            info.VerificationKeyId = "DebugKey";
            info.VerificationKeyVersion = "DebugVersion";
        }
    }
}
