using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
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
        private System.Security.Cryptography.ECDsa Key;
        private byte[] PublicKey;

        public TemporaryExposureKeySignServiceDebug(IConfiguration config)
        {
            Key = System.Security.Cryptography.ECDsaCng.Create(ECCurve.NamedCurves.nistP256);
            PublicKey = Key.ExportSubjectPublicKeyInfo();
        }

        public async Task<byte[]> SignAsync(Stream source)
        {
            byte[] hash;
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                hash = sha.ComputeHash(source);
            }
            return Key.SignHash(hash);
        }

        public async Task<byte[]> GetPublicKeyAsync()
        {
            return PublicKey;
        }
    }
}
