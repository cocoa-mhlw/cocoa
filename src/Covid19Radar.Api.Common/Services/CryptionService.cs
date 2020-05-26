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
            symmetric.Key = config.CryptionKey();
            symmetric.IV = config.CryptionIV();
            hashKey = config.CryptionHash();
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
            symmetric2.Key = config.CryptionKey2();
            symmetric2.IV = config.CryptionIV2();
        }

        [ThreadStatic]
        static byte[] bufHash;
        [ThreadStatic]
        static byte[] bufHash2;
        private void InitBufHash()
        {
            if (bufHash == null) { bufHash = new byte[64]; }
            if (bufHash2 == null) { bufHash2 = new byte[64]; }
        }

        [ThreadStatic]
        static byte[] bufInput;
        private void InitBufInput()
        {
            if (bufInput == null) { bufInput = new byte[2048]; }
        }

        private ValueTuple<byte[], byte[]> Random()
        {
            var range = RNGCryptoServiceProvider.GetInt32(128, Length);
            byte[] r1 = new byte[range];
            byte[] r2 = new byte[Length - range];
            RNGCryptoServiceProvider.Fill(r1);
            RNGCryptoServiceProvider.Fill(r2);
            return ValueTuple.Create(r1, r2);
        }

        public string CreateSecret(string userUuid)
        {
            var userUuidBytes = Encoding.UTF8.GetBytes(userUuid);
            InitBufHash();
            int hashSize;
            createHash().TryComputeHash(userUuidBytes, bufHash, out hashSize);
            var randoms = Random();
            var val = randoms.Item1.Concat(bufHash).Concat(randoms.Item2).ToArray();
            createHash().TryComputeHash(val, bufHash2, out hashSize);
            var secret = val.Concat(bufHash2).ToArray();
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
                var userUuidBytes = Encoding.UTF8.GetBytes(userUuid);
                InitBufHash();
                int hashSize;
                createHash().TryComputeHash(userUuidBytes, bufHash, out hashSize);
                for (var i = Length; i >= 0; i--)
                {
                    if (result.Skip(i).SkipLast(64 + (Length - i)).SequenceEqual(bufHash))
                    {
                        createHash().TryComputeHash(result.AsSpan(0, result.Length - 64), bufHash2, out hashSize);
                        return bufHash2.SequenceEqual(result.Skip(result.Length - 64).Take(64));
                    }
                }
                return false;
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
