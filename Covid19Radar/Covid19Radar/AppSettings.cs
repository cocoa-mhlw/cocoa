/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Covid19Radar
{
    public class AppSettings
    {
        static AppSettings instance;

        public static AppSettings Instance
            => instance ??= new AppSettings();

        public AppSettings()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var file = assembly.GetManifestResourceStream("Covid19Radar.settings.json");
            using var sr = new StreamReader(file);
            var json = sr.ReadToEnd();
            var j = JObject.Parse(json);

            AppVersion = j.Value<string>("appVersion");
            LicenseFilename = j.Value<string>("licenseFilename");
            AppStoreUrl = j.Value<string>("appStoreUrl");
            GooglePlayUrl = j.Value<string>("googlePlayUrl");
            ApiUrlBase = j.Value<string>("apiUrlBase");
            ApiSecret = j.Value<string>("apiSecret");
            ApiKey = j.Value<string>("apiKey");
            CdnUrlBase = j.Value<string>("cdnUrlBase");
            BlobStorageContainerName = j.Value<string>("blobStorageContainerName");
            SupportedRegions = j.Value<string>("supportedRegions").ToUpperInvariant().Split(';', ',', ':');
            AndroidSafetyNetApiKey = j.Value<string>("androidSafetyNetApiKey");
            SupportEmail = j.Value<string>("supportEmail");
            LogStorageEndpoint = j.Value<string>("logStorageEndpoint");
            LogStorageContainerName = j.Value<string>("logStorageContainerName");
            LogStorageAccountName = j.Value<string>("logStorageAccountName");
        }

        public string SupportEmail { get; }
        public string AppVersion { get; }
        public string LicenseFilename { get; }
        public string ApiUrlBase { get; }
        public string ApiSecret { get; }
        public string ApiKey { get; }
        public string AppStoreUrl { get; }
        public string GooglePlayUrl { get; }
        public string CdnUrlBase { get; }

        public string[] SupportedRegions { get; }

        public string BlobStorageContainerName { get; }

        public string AndroidSafetyNetApiKey { get; }

        public string LogStorageEndpoint { get; }
        public string LogStorageContainerName { get; }
        public string LogStorageAccountName { get; }

        internal Dictionary<string, ulong> GetDefaultBatch() =>
            Instance.SupportedRegions.ToDictionary(r => r, r => (ulong)0);
    }
}
