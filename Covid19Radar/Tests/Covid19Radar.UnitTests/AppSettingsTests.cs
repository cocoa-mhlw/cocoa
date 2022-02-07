/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Xunit;

namespace Covid19Radar.UnitTests
{
    public class AppSettingsTests
    {
        // TODO: Maintain the tests for debug and release compatibility.
        [Fact(Skip = "For debugging COCOA2, this test will be skipped.")]
        public void SettingsTests()
        {
            var settings = new AppSettings();
            Assert.Equal("APP_VERSION", settings.AppVersion);
            Assert.Equal("API_SECRET", settings.ApiSecret);
            Assert.Equal("API_KEY", settings.ApiKey);
            Assert.Equal("https://API_URL_BASE/api", settings.ApiUrlBase);
            Assert.Single(settings.SupportedRegions);
            Assert.Equal("440", settings.SupportedRegions[0]);
            Assert.Equal("c19r", settings.BlobStorageContainerName);
            Assert.Equal("https://CDN_URL_BASE/c19r", settings.ExposureConfigurationUrlBase);
            Assert.Equal("ANDROID_SAFETYNETKEY", settings.AndroidSafetyNetApiKey);
            Assert.Equal("https://CDN_URL_BASE/", settings.CdnUrlBase);
            Assert.Equal("https://itunes.apple.com/jp/app/id1516764458?mt=8", settings.AppStoreUrl);
            Assert.Equal("https://play.google.com/store/apps/details?id=jp.go.mhlw.covid19radar", settings.GooglePlayUrl);
            Assert.Equal("SUPPORT_EMAIL", settings.SupportEmail);
            Assert.Equal("https://LOG_STORAGE_URL_BASE/", settings.LogStorageEndpoint);
            Assert.Equal("LOG_STORAGE_CONTAINER_NAME", settings.LogStorageContainerName);
            Assert.Equal("LOG_STORAGE_ACCOUNT_NAME", settings.LogStorageAccountName);
        }
    }
}
