using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
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
        private Microsoft.Azure.KeyVault.Models.KeyBundle KeyVaultKey;
        private byte[] PublicKey;

        public TemporaryExposureKeySignService(IConfiguration config)
        {
            TekExportKeyVaultKeyUrl = config["TekExportKeyVaultKeyUrl"];
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            var credentialCallback = new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback);
            KeyVault = new KeyVaultClient(credentialCallback);
        }

        private async Task InitializeAsync()
        {
            if (KeyVaultKey == null)
            {
                KeyVaultKey = await KeyVault.GetKeyAsync(TekExportKeyVaultKeyUrl);
                PublicKey = KeyVaultKey.Key.ToECDsa().ExportSubjectPublicKeyInfo();
            }
        }

        public async Task<byte[]> SignAsync(Stream source)
        {
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
            await InitializeAsync();
            return PublicKey;
        }
    }
}
