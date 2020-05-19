using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Services
{
    public class TemporaryExposureKeySignService : ITemporaryExposureKeySignService
    {
        public readonly KeyVaultClient KeyVault;
        public readonly string TekExportKeyVaultKeyUrl;
        public readonly ILogger<TemporaryExposureKeySignService> Logger;
        private Microsoft.Azure.KeyVault.Models.KeyBundle KeyVaultKey;
        private byte[] PublicKey;

        public TemporaryExposureKeySignService(
            IConfiguration config,
            ILogger<TemporaryExposureKeySignService> logger)
        {
            Logger = logger;
            TekExportKeyVaultKeyUrl = config["TekExportKeyVaultKeyUrl"];
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
                PublicKey = KeyVaultKey.Key.ToECDsa().ExportSubjectPublicKeyInfo();
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

        public async Task<byte[]> GetPublicKeyAsync()
        {
            Logger.LogInformation($"start {nameof(GetPublicKeyAsync)}");
            await InitializeAsync();
            return PublicKey;
        }
    }
}
