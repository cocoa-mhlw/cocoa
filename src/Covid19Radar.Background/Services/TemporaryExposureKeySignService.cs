using Covid19Radar.Api.Protobuf;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace Covid19Radar.Background.Services
{
    public class TemporaryExposureKeySignService : ITemporaryExposureKeySignService
    {
        public readonly KeyVaultClient KeyVault;
        public readonly string TekExportKeyVaultKeyUrl;
        public readonly ILogger<TemporaryExposureKeySignService> Logger;
        private Microsoft.Azure.KeyVault.Models.KeyBundle KeyVaultKey;

        public TemporaryExposureKeySignService(
            IConfiguration config,
            ILogger<TemporaryExposureKeySignService> logger)
        {
            Logger = logger;
            Logger.LogInformation($"{nameof(TemporaryExposureKeySignService)} constructor");
            TekExportKeyVaultKeyUrl = config.TekExportKeyVaultKeyUrl();
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            var credentialCallback = new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback);
            KeyVault = new KeyVaultClient(credentialCallback);
        }

        private async Task InitializeAsync()
        {
            Logger.LogInformation($"start {nameof(InitializeAsync)}");
            if (KeyVaultKey == null)
            {
                KeyVaultKey = await KeyVault.GetKeyAsync(TekExportKeyVaultKeyUrl);
            }
        }

        public async Task<byte[]> SignAsync(Stream source)
        {
            Logger.LogInformation($"start {nameof(SignAsync)}");
            await InitializeAsync();
            byte[] hash;
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                hash = sha.ComputeHash(source);
            }
            var result = await KeyVault.SignAsync(TekExportKeyVaultKeyUrl, Microsoft.Azure.KeyVault.Cryptography.Algorithms.Es256.AlgorithmName, hash);
            return result.Result;
        }

        public async Task SetSignatureAsync(SignatureInfo info)
        {
            Logger.LogInformation($"start {nameof(SetSignatureAsync)}");
            await InitializeAsync();
            info.VerificationKeyId = KeyVaultKey.Key.Kid;
            info.VerificationKeyVersion = KeyVaultKey.KeyIdentifier.Version;
        }

    }
}
