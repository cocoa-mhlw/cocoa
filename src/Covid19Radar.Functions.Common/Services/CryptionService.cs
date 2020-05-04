using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Covid19Radar.Services
{
    public class CryptionService : ICryptionService
    {
        private readonly ILogger<CryptionService> Logger;
        private readonly SymmetricAlgorithm symmetric;
        private const int Length = 256;

        public CryptionService(
            Microsoft.Extensions.Configuration.IConfiguration config,
            ILogger<CryptionService> logger)
        {
            this.Logger = logger;
            symmetric = Aes.Create();
            symmetric.Mode = CipherMode.CBC;
            symmetric.Padding = PaddingMode.ISO10126;
            symmetric.KeySize = 256;
            symmetric.Key = Convert.FromBase64String(config.GetSection("CRYPTION_KEY").Value);
            symmetric.IV = Convert.FromBase64String(config.GetSection("CRYPTION_IV").Value);
        }


        private byte[] Random()
        {
            byte[] r = new byte[Length];
            RNGCryptoServiceProvider.Create().GetBytes(r);
            return r;
        }

        public string CreateSecret()
        {
            return Convert.ToBase64String(Random());
        }

        public string Protect(string secret)
        {
            var buf = Convert.FromBase64String(secret);
            using (var c = symmetric.CreateEncryptor())
            {
                var result = c.TransformFinalBlock(buf, 0, buf.Length);
                return Convert.ToBase64String(result);
            }
        }

        public string Unprotect(string protectSecret)
        {
            var buf = Convert.FromBase64String(protectSecret);
            using (var c = symmetric.CreateDecryptor())
            {
                var result = c.TransformFinalBlock(buf, 0, buf.Length);
                return Convert.ToBase64String(result);
            }
        }
    }
}
