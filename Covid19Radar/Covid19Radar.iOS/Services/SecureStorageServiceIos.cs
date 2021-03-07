using System;
using Covid19Radar.iOS.Services;
using Covid19Radar.Services;
using Foundation;
using Security;
using Xamarin.Essentials;

namespace Covid19Radar.iOS.Services
{
    public class SecureStorageServiceIos : ISecureStorageDependencyService
    {
        static readonly string Service = $"{AppInfo.PackageName}.securestorage";

        public bool ContainsKey(string key)
        {
            var record = new SecRecord(SecKind.GenericPassword) { Account = key, Service = Service };
            return IsExists(record);
        }

        public byte[] GetBytes(string key)
        {
            byte[] result = null;

            var record = new SecRecord(SecKind.GenericPassword) { Account = key, Service = Service };
            var loadedData = SecKeyChain.QueryAsData(record, false, out var status);
            if (status == SecStatusCode.Success)
            {
                result = loadedData.ToArray();
            }
            else if (status != SecStatusCode.ItemNotFound)
            {
                throw new InvalidOperationException($"Failed to get from keychain. (status={status})");
            }

            return result;
        }

        public void SetBytes(string key, byte[] bytes)
        {
            var record = new SecRecord(SecKind.GenericPassword) { Account = key, Service = Service };
            if (IsExists(record))
            {
                SecKeyChain.Remove(record);
            }

            record.Label = key;
            record.Accessible = SecAccessible.AfterFirstUnlock;
            record.ValueData = NSData.FromArray(bytes);

            var status = SecKeyChain.Add(record);
            if (status != SecStatusCode.Success)
            {
                throw new InvalidOperationException($"Failed to add to keychain. (status={status})");
            }
        }

        public void Remove(string key)
        {
            var record = new SecRecord(SecKind.GenericPassword) { Account = key, Service = Service };
            if (IsExists(record))
            {
                SecKeyChain.Remove(record);
            }
        }

        private bool IsExists(SecRecord record)
        {
            var data = SecKeyChain.QueryAsData(record, false, out var status);
            return status == SecStatusCode.Success && data.Length > 0;
        }
    }
}
