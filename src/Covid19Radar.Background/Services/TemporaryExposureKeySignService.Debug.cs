using Covid19Radar.Api.Protobuf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
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
            Logger.LogInformation($"{nameof(TemporaryExposureKeySignServiceDebug)} constructor");
            Key = System.Security.Cryptography.ECDsaCng.Create(ECCurve.NamedCurves.nistP256);
        }

        public Task<byte[]> SignAsync(Stream source)
        {
            Logger.LogInformation($"start {nameof(SignAsync)}");
            byte[] hash;
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                hash = sha.ComputeHash(source);
            }
            return Task.FromResult(Key.SignHash(hash));
        }

        public Task SetSignatureAsync(SignatureInfo info)
        {
            Logger.LogInformation($"start {nameof(SetSignatureAsync)}");
            info.VerificationKeyId = "DebugKey";
            info.VerificationKeyVersion = "DebugVersion";
            return Task.CompletedTask;
        }
    }
}
