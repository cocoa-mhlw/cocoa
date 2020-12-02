using Xunit;

namespace Covid19Radar.UnitTests
{
    public class AppSettingsTests
    {
        [Fact]
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
            Assert.Equal("ANDROID_SAFETYNETKEY", settings.AndroidSafetyNetApiKey);
            Assert.Equal("https://CDN_URL_BASE/", settings.CdnUrlBase);
            Assert.Equal("https://covid19radarjpnprod.z11.web.core.windows.net/license.html", settings.LicenseUrl);
            Assert.Equal("https://itunes.apple.com/jp/app/id1516764458?mt=8", settings.AppStoreUrl);
            Assert.Equal("https://play.google.com/store/apps/details?id=jp.go.mhlw.covid19radar", settings.GooglePlayUrl);
            Assert.Equal("SUPPORT_EMAIL", settings.SupportEmail);
            Assert.Equal("https://LOG_STORAGE_URL_BASE/", settings.LogStorageEndpoint);
            Assert.Equal("LOG_STORAGE_CONTAINER_NAME", settings.LogStorageContainerName);
            Assert.Equal("LOG_STORAGE_ACCOUNT_NAME", settings.LogStorageAccountName);
        }
    }
}
