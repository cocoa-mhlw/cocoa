using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Covid19Radar.Api.Services
{
    public class CryptionService : ICryptionService
    {
        private readonly ILogger<CryptionService> Logger;
        private readonly SymmetricAlgorithm symmetric;
        private readonly SymmetricAlgorithm symmetric2;
        private const int Length = 256;

        [ThreadStatic]
        private static HashAlgorithm hash;
        private Func<HashAlgorithm> createHash;
        private string hashKey;

        public CryptionService(
            Microsoft.Extensions.Configuration.IConfiguration config,
            ILogger<CryptionService> logger)
        {
            Logger = logger;
            symmetric = Aes.Create();
            symmetric.Mode = CipherMode.CBC;
            symmetric.Padding = PaddingMode.ISO10126;
            symmetric.KeySize = 256;
            symmetric.Key = Convert.FromBase64String(config.GetSection("CRYPTION_KEY").Value);
            symmetric.IV = Convert.FromBase64String(config.GetSection("CRYPTION_IV").Value);
            hashKey = config.GetSection("CRYPTION_HASH").Value;
            createHash = () =>
            {
                if (hash == null)
                {
                    hash = new HMACSHA512(Convert.FromBase64String(hashKey));
                }
                return hash;
            };
            symmetric2 = Aes.Create();
            symmetric2.Mode = CipherMode.CBC;
            symmetric2.Padding = PaddingMode.ISO10126;
            symmetric2.KeySize = 256;
            symmetric2.Key = Convert.FromBase64String(config.GetSection("CRYPTION_KEY2").Value);
            symmetric2.IV = Convert.FromBase64String(config.GetSection("CRYPTION_IV2").Value);
        }

        [ThreadStatic]
        static byte[] bufHash;
        private void InitBufHash()
        {
            if (bufHash == null) { bufHash = new byte[64]; }
        }

        [ThreadStatic]
        static byte[] bufInput;
        private void InitBufInput()
        {
            if (bufInput == null) { bufInput = new byte[2048]; }
        }

        private byte[] Random()
        {
            byte[] r = new byte[Length];
            RNGCryptoServiceProvider.Fill(r);
            return r;
        }

        public string CreateSecret(string userUuid)
        {
            var val = Random().Concat(Encoding.UTF8.GetBytes(userUuid)).ToArray();
            int hashSize;
            InitBufHash();
            createHash().TryComputeHash(val, bufHash, out hashSize);
            var secret = val.Concat(bufHash).ToArray();
            using (var c = symmetric2.CreateEncryptor())
            {
                return Convert.ToBase64String(c.TransformFinalBlock(secret, 0, secret.Length));
            }
        }

        public bool ValidateSecret(string userUuid, string secret)
        {
            if (string.IsNullOrWhiteSpace(userUuid)) { return false; }
            if (userUuid.Length > 256) { return false; }
            int bufInputLength;
            InitBufInput();
            Array.Clear(bufInput, 0, bufInput.Length);
            if (!Convert.TryFromBase64String(secret, bufInput, out bufInputLength)) { return false; }
            using (var c = symmetric2.CreateDecryptor())
            {
                var result = c.TransformFinalBlock(bufInput, 0, bufInputLength);
                if (userUuid != Encoding.UTF8.GetString(result, Length, result.Length - Length - 64))
                {
                    return false;
                }
                int hashSize;
                InitBufHash();
                createHash().TryComputeHash(result.AsSpan(0, result.Length - 64), bufHash, out hashSize);
                return bufHash.SequenceEqual(result.Skip(result.Length - 64).Take(64));
            }
        }

        public string Protect(string secret)
        {
            int bufInputLength;
            InitBufInput();
            Array.Clear(bufInput, 0, bufInput.Length);
            Convert.TryFromBase64String(secret, bufInput, out bufInputLength);
            using (var c = symmetric.CreateEncryptor())
            {
                var result = c.TransformFinalBlock(bufInput, 0, bufInputLength);
                return Convert.ToBase64String(result);
            }
        }

        public string Unprotect(string protectSecret)
        {
            int bufInputLength;
            InitBufInput();
            Array.Clear(bufInput, 0, bufInput.Length);
            Convert.TryFromBase64String(protectSecret, bufInput, out bufInputLength);
            using (var c = symmetric.CreateDecryptor())
            {
                var result = c.TransformFinalBlock(bufInput, 0, bufInputLength);
                return Convert.ToBase64String(result);
            }
        }
    }
}
