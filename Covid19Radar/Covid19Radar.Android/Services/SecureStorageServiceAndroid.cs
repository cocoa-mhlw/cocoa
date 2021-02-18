using System;
using Android.Content;
using Android.Security.Keystore;
using Covid19Radar.Droid.Services;
using Covid19Radar.Services;
using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(SecureStorageServiceAndroid))]
namespace Covid19Radar.Droid.Services
{
    public class SecureStorageServiceAndroid : ISecureStorageDependencyService
    {
        private readonly string Alias = $"{AppInfo.PackageName}.securestorage";
        private readonly int IvLength = 12;
        private readonly int GcmTagLength = 128;
        private readonly string KeyStoreType = "AndroidKeyStore";
        private readonly string CipherTransformation = "AES/GCM/NoPadding";

        private readonly ISharedPreferences sharedPreferences;

        public SecureStorageServiceAndroid()
        {
            sharedPreferences = Android.App.Application.Context.GetSharedPreferences(Alias, FileCreationMode.Private);
        }

        public bool ContainsKey(string key)
        {
            return sharedPreferences.Contains(key);
        }

        public byte[] GetBytes(string key)
        {
            byte[] result = null;

            if (sharedPreferences.Contains(key))
            {
                var loadedText = sharedPreferences.GetString(key, "");
                var loadedBytes = Convert.FromBase64String(loadedText);
                if (loadedBytes.Length <= IvLength)
                {
                    throw new InvalidOperationException("Invalid read data.");
                }

                var iv = new byte[IvLength];
                Array.Copy(loadedBytes, 0, iv, 0, iv.Length);

                var encryptedBytes = new byte[loadedBytes.Length - iv.Length];
                Array.Copy(loadedBytes, iv.Length, encryptedBytes, 0, encryptedBytes.Length);

                var keyStore = KeyStore.GetInstance(KeyStoreType);
                keyStore.Load(null);

                var storeKey = keyStore.GetKey(Alias, null);
                if (storeKey == null)
                {
                    throw new InvalidOperationException("Could not get the KeyStore key.");
                }

                var cipher = Cipher.GetInstance(CipherTransformation);
                cipher.Init(CipherMode.DecryptMode, storeKey, new GCMParameterSpec(GcmTagLength, iv));
                result = cipher.DoFinal(encryptedBytes);
            }

            return result;
        }

        public void Remove(string key)
        {
            if (sharedPreferences.Contains(key))
            {
                var edit = sharedPreferences.Edit();
                edit.Remove(key);
                edit.Commit();
            }
        }

        public void SetBytes(string key, byte[] bytes)
        {
            var keyStore = KeyStore.GetInstance(KeyStoreType);
            keyStore.Load(null);

            var storeKey = keyStore.GetKey(Alias, null);
            if (storeKey == null)
            {
                var generator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, KeyStoreType);
                var spec = new KeyGenParameterSpec.Builder(Alias, KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                    .SetBlockModes(KeyProperties.BlockModeGcm)
                    .SetEncryptionPaddings(KeyProperties.EncryptionPaddingNone)
                    .SetRandomizedEncryptionRequired(false)
                    .Build();

                generator.Init(spec);
                generator.GenerateKey();

                storeKey = keyStore.GetKey(Alias, null);
                if (storeKey == null)
                {
                    throw new InvalidOperationException("Failed KeyStore key generation.");
                }
            }

            var iv = new byte[IvLength];
            var randam = new SecureRandom();
            randam.NextBytes(iv);

            var cipher = Cipher.GetInstance(CipherTransformation);
            cipher.Init(CipherMode.EncryptMode, storeKey, new GCMParameterSpec(GcmTagLength, iv));

            var encryptedBytes = cipher.DoFinal(bytes);

            var saveBytes = new byte[iv.Length + encryptedBytes.Length];
            Array.Copy(iv, 0, saveBytes, 0, iv.Length);
            Array.Copy(encryptedBytes, 0, saveBytes, iv.Length, encryptedBytes.Length);

            var saveText = Convert.ToBase64String(saveBytes);

            var edit = sharedPreferences.Edit();
            edit.PutString(key, saveText);
            edit.Commit();
        }
    }
}
