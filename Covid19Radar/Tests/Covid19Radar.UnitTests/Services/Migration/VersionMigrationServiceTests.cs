/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Covid19Radar.Common;
using Covid19Radar.Model;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Covid19Radar.Services.Migration;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Covid19Radar.UnitTests.Services.Migration
{
    public class VersionMigrationServiceTests
    {
        const string APPLICATION_PROPERTY_USER_DATA_KEY = "UserData";
        const string APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY = "TermsOfServiceLastUpdateDateTime";
        const string APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY = "PrivacyPolicyLastUpdateDateTime";

        private readonly MockRepository _mockRepository = new MockRepository(MockBehavior.Default);

        private readonly Mock<IVersionMigration> _mockVersionMigration;
        private readonly Mock<ILoggerService> _mockLoggerService;
        private readonly Mock<IEssentialsService> _mockEssentialService;

        private readonly ISecureStorageService _dummySecureStorageService;
        private readonly IPreferencesService _dummyPreferencesService;
        private readonly IApplicationPropertyService _dummyApplicationPropertyService;

        public VersionMigrationServiceTests()
        {
            _mockVersionMigration = _mockRepository.Create<IVersionMigration>();
            _mockLoggerService = _mockRepository.Create<ILoggerService>();
            _mockEssentialService = _mockRepository.Create<IEssentialsService>();

            _dummySecureStorageService = new InMemorySecureStorageService();
            _dummyPreferencesService = new InMemoryPreferencesService();
            _dummyApplicationPropertyService = new InMemoryApplicationPropertyService();

        }

        private VersionMigrationService CreateService()
        {
            return new VersionMigrationService(
                _mockVersionMigration.Object,
                _dummyApplicationPropertyService,
                _dummyPreferencesService,
                _dummySecureStorageService,
                _mockEssentialService.Object,
                _mockLoggerService.Object
                );
        }

        [Fact]
        public async Task Initialize100Async()
        {
            _mockEssentialService.SetupGet(x => x.AppVersion).Returns("1.0.0");

            await CreateService()
                .MigrateAsync();

            Assert.True(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY));

            string userData = (string)_dummyApplicationPropertyService.GetProperties(APPLICATION_PROPERTY_USER_DATA_KEY);
            Assert.Equal(
                "{\"StartDateTime\":\"0001-01-01T00:00:00\",\"IsOptined\":false,\"IsPolicyAccepted\":false,\"LastProcessTekTimestamp\":{},\"ExposureInformation\":[],\"ExposureSummary\":null}",
                userData
                );
        }

        [Fact]
        public async Task Migrate_InitTo122Async()
        {
            _mockEssentialService.SetupGet(x => x.AppVersion).Returns("1.2.2");

            await CreateService()
                .MigrateAsync();

            // Application-properties must not be exist
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureSummary));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureInformation));

            // AppVersion
            var preferenceAppVersion = _dummyPreferencesService.GetValue<string>(PreferenceKey.AppVersion, null);
            Assert.Equal("1.2.2", preferenceAppVersion);

            // TermsOfServiceLastUpdateDateTime
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.TermsOfServiceLastUpdateDateTime));

            // PrivacyPolicyLastUpdateDateTime
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.PrivacyPolicyLastUpdateDateTime));

            // LastProcessTekTimestamp
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            var lastProcessTekTimestampString = _dummyPreferencesService.GetValue<string>(PreferenceKey.LastProcessTekTimestamp, null);

            // ExposureNotificationConfiguration
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));

            Assert.False(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureSummary));
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureInformation));
        }

        [Fact]
        public async Task Migrate_InitTo123Async()
        {
            _mockEssentialService.SetupGet(x => x.AppVersion).Returns("1.2.3");

            await CreateService()
                .MigrateAsync();

            // Application-properties must not be exist
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureSummary));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureInformation));

            // AppVersion
            var preferenceAppVersion = _dummyPreferencesService.GetValue<string>(PreferenceKey.AppVersion, null);
            Assert.Equal("1.2.3", preferenceAppVersion);

            // TermsOfServiceLastUpdateDateTime
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.TermsOfServiceLastUpdateDateTime));

            // PrivacyPolicyLastUpdateDateTime
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.PrivacyPolicyLastUpdateDateTime));

            // LastProcessTekTimestamp
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            var lastProcessTekTimestampString = _dummyPreferencesService.GetValue<string>(PreferenceKey.LastProcessTekTimestamp, null);

            // ExposureNotificationConfiguration
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));

            Assert.False(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureSummary));
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureInformation));
        }

        private async Task<(DateTime, DateTime, DateTime, Dictionary<string, long>, UserExposureInfo, UserExposureInfo, UserExposureSummary)> SetUpVersion100(
            bool isOptIned = true,
            bool isPrivaryPolicyAgreed = true,
            bool hasAppVersionAtPreference = true)
        {
            _mockEssentialService.SetupGet(x => x.AppVersion).Returns("1.0.0");

            var startDateTime = DateTime.UtcNow;
            var termsOfServiceLastUpdateDate = DateTime.UtcNow.AddMinutes(1);
            var privacyPolicyLastUpdateDate = DateTime.UtcNow.AddMinutes(2);

            var userExposureInfo1DateTime = DateTime.UtcNow;
            var userExposureInfo2DateTime = DateTime.UtcNow.AddDays(1);

            Dictionary<string, long> lastProcesTekTimestamp = new Dictionary<string, long>{
                    {"404", 53454353 }
                };

            var userExposureInfo1 = new UserExposureInfo(userExposureInfo1DateTime, new TimeSpan(50), 3, 11, UserRiskLevel.High);
            var userExposureInfo2 = new UserExposureInfo(userExposureInfo2DateTime, new TimeSpan(100), 8, 10, UserRiskLevel.Medium);
            var userExposureSummary = new UserExposureSummary(4, 6, 11, new TimeSpan[0], 5);

            var userData = new UserDataModel()
            {
                StartDateTime = startDateTime,
                IsOptined = isOptIned,
                IsPolicyAccepted = isPrivaryPolicyAgreed,
                LastProcessTekTimestamp = lastProcesTekTimestamp,
                ExposureInformation = new ObservableCollection<UserExposureInfo> {
                    userExposureInfo1,
                    userExposureInfo2,
                },
                ExposureSummary = userExposureSummary
            };

            var userDataString = JsonConvert.SerializeObject(userData);

            // Setup Application-properties
            await _dummyApplicationPropertyService.SavePropertiesAsync(APPLICATION_PROPERTY_USER_DATA_KEY, userDataString);
            await _dummyApplicationPropertyService.SavePropertiesAsync(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY, termsOfServiceLastUpdateDate);
            await _dummyApplicationPropertyService.SavePropertiesAsync(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY, privacyPolicyLastUpdateDate);

            await CreateService()
                .MigrateAsync();

            if (!hasAppVersionAtPreference)
            {
                // Remove AppVersion from preference
                _dummyPreferencesService.RemoveValue(PreferenceKey.AppVersion);
            }

            return (
                startDateTime,
                termsOfServiceLastUpdateDate, privacyPolicyLastUpdateDate,
                lastProcesTekTimestamp,
                userExposureInfo1, userExposureInfo2, userExposureSummary
                );
        }

        [Fact]
        public async Task Migrate_100to122Async()
        {
            var (
                startDateTime,
                termsOfServiceLastUpdateDate, privacyPolicyLastUpdateDate,
                lastProcesTekTimestamp,
                userExposureInfo1, userExposureInfo2, userExposureSummary
            ) = await SetUpVersion100();

            _mockEssentialService.SetupGet(x => x.AppVersion).Returns("1.2.2");

            await CreateService()
                .MigrateAsync();

            var preferenceAppVersion = _dummyPreferencesService.GetValue<string>(PreferenceKey.AppVersion, null);
            Assert.Equal("1.2.2", preferenceAppVersion);

            // Application-properties must not be exist
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureSummary));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureInformation));

            // StartDateTime
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.StartDateTime));
            var startDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.StartDateTime, new DateTime());
            Assert.Equal(startDateTime, startDateTimePref);

            // TermsOfServiceLastUpdateDateTime
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.TermsOfServiceLastUpdateDateTime));
            var termsOfServiceLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.TermsOfServiceLastUpdateDateTime, new DateTime());
            Assert.Equal(termsOfServiceLastUpdateDate, termsOfServiceLastUpdateDateTimePref);

            // PrivacyPolicyLastUpdateDateTime
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.PrivacyPolicyLastUpdateDateTime));
            var privacyPolicyLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.PrivacyPolicyLastUpdateDateTime, new DateTime());
            Assert.Equal(privacyPolicyLastUpdateDate, privacyPolicyLastUpdateDateTimePref);

            // LastProcessTekTimestamp
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            var lastProcessTekTimestampPrefString = _dummyPreferencesService.GetValue(PreferenceKey.LastProcessTekTimestamp, "{}");
            var lastProcessTekTimestampPref = JsonConvert.DeserializeObject<IDictionary<string, long>>(lastProcessTekTimestampPrefString);
            Assert.Equal(lastProcesTekTimestamp, lastProcessTekTimestampPref);

            // ExposureNotificationConfiguration
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));

            // ExposureInformation
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureInformation));
            var userExposureInfosPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureInformation, "{}");
            var userExposureInfosPref = JsonConvert.DeserializeObject<ObservableCollection<UserExposureInfo>>(userExposureInfosPrefString);
            Assert.Equal(2, userExposureInfosPref.Count);
            AssertUserExposureInfo(userExposureInfo1, userExposureInfosPref[0]);
            AssertUserExposureInfo(userExposureInfo2, userExposureInfosPref[1]);

            // ExposureSummary
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureSummary));
            var userExposureSummaryPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureSummary, "{}");
            var userExposureSummaryPref = JsonConvert.DeserializeObject<UserExposureSummary>(userExposureSummaryPrefString);
            AssertUserExposureInfo(userExposureSummaryPref, userExposureSummary);
        }

        [Fact]
        public async Task Migrate_100to122NotOptInedAsync()
        {
            var (
                startDateTime,
                termsOfServiceLastUpdateDate, privacyPolicyLastUpdateDate,
                lastProcesTekTimestamp,
                userExposureInfo1, userExposureInfo2, userExposureSummary
            ) = await SetUpVersion100(isOptIned: false);

            _mockEssentialService.SetupGet(x => x.AppVersion).Returns("1.2.2");

            await CreateService()
                .MigrateAsync();

            var preferenceAppVersion = _dummyPreferencesService.GetValue<string>(PreferenceKey.AppVersion, null);
            Assert.Equal("1.2.2", preferenceAppVersion);

            // Application-properties must not be exist
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureSummary));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureInformation));

            // StartDateTime
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.StartDateTime));
            var startDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.StartDateTime, new DateTime());
            Assert.Equal(startDateTime, startDateTimePref);

            // TermsOfServiceLastUpdateDateTime
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.TermsOfServiceLastUpdateDateTime));
            //var termsOfServiceLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.TermsOfServiceLastUpdateDateTime, new DateTime());
            //Assert.Equal(termsOfServiceLastUpdateDate, termsOfServiceLastUpdateDateTimePref);

            // PrivacyPolicyLastUpdateDateTime
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.PrivacyPolicyLastUpdateDateTime));
            var privacyPolicyLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.PrivacyPolicyLastUpdateDateTime, new DateTime());
            Assert.Equal(privacyPolicyLastUpdateDate, privacyPolicyLastUpdateDateTimePref);

            // LastProcessTekTimestamp
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            var lastProcessTekTimestampPrefString = _dummyPreferencesService.GetValue(PreferenceKey.LastProcessTekTimestamp, "{}");
            var lastProcessTekTimestampPref = JsonConvert.DeserializeObject<IDictionary<string, long>>(lastProcessTekTimestampPrefString);
            Assert.Equal(lastProcesTekTimestamp, lastProcessTekTimestampPref);

            // ExposureNotificationConfiguration
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));

            // ExposureInformation
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureInformation));
            var userExposureInfosPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureInformation, "{}");
            var userExposureInfosPref = JsonConvert.DeserializeObject<ObservableCollection<UserExposureInfo>>(userExposureInfosPrefString);
            Assert.Equal(2, userExposureInfosPref.Count);
            AssertUserExposureInfo(userExposureInfo1, userExposureInfosPref[0]);
            AssertUserExposureInfo(userExposureInfo2, userExposureInfosPref[1]);

            // ExposureSummary
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureSummary));
            var userExposureSummaryPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureSummary, "{}");
            var userExposureSummaryPref = JsonConvert.DeserializeObject<UserExposureSummary>(userExposureSummaryPrefString);
            AssertUserExposureInfo(userExposureSummaryPref, userExposureSummary);
        }

        [Fact]
        public async Task Migrate_100to122NotPrivacyPolicyAgreedAsync()
        {
            var (
                startDateTime,
                termsOfServiceLastUpdateDate, privacyPolicyLastUpdateDate,
                lastProcesTekTimestamp,
                userExposureInfo1, userExposureInfo2, userExposureSummary
            ) = await SetUpVersion100(isPrivaryPolicyAgreed: false);

            _mockEssentialService.SetupGet(x => x.AppVersion).Returns("1.2.2");

            await CreateService()
                .MigrateAsync();

            var preferenceAppVersion = _dummyPreferencesService.GetValue<string>(PreferenceKey.AppVersion, null);
            Assert.Equal("1.2.2", preferenceAppVersion);

            // Application-properties must not be exist
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureSummary));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureInformation));

            // StartDateTime
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.StartDateTime));
            var startDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.StartDateTime, new DateTime());
            Assert.Equal(startDateTime, startDateTimePref);

            // TermsOfServiceLastUpdateDateTime
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.TermsOfServiceLastUpdateDateTime));
            var termsOfServiceLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.TermsOfServiceLastUpdateDateTime, new DateTime());
            Assert.Equal(termsOfServiceLastUpdateDate, termsOfServiceLastUpdateDateTimePref);

            // PrivacyPolicyLastUpdateDateTime
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.PrivacyPolicyLastUpdateDateTime));
            //var privacyPolicyLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.PrivacyPolicyLastUpdateDateTime, new DateTime());
            //Assert.Equal(privacyPolicyLastUpdateDate, privacyPolicyLastUpdateDateTimePref);

            // LastProcessTekTimestamp
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            var lastProcessTekTimestampPrefString = _dummyPreferencesService.GetValue(PreferenceKey.LastProcessTekTimestamp, "{}");
            var lastProcessTekTimestampPref = JsonConvert.DeserializeObject<IDictionary<string, long>>(lastProcessTekTimestampPrefString);
            Assert.Equal(lastProcesTekTimestamp, lastProcessTekTimestampPref);

            // ExposureNotificationConfiguration
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));

            // ExposureInformation
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureInformation));
            var userExposureInfosPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureInformation, "{}");
            var userExposureInfosPref = JsonConvert.DeserializeObject<ObservableCollection<UserExposureInfo>>(userExposureInfosPrefString);
            Assert.Equal(2, userExposureInfosPref.Count);
            AssertUserExposureInfo(userExposureInfo1, userExposureInfosPref[0]);
            AssertUserExposureInfo(userExposureInfo2, userExposureInfosPref[1]);

            // ExposureSummary
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureSummary));
            var userExposureSummaryPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureSummary, "{}");
            var userExposureSummaryPref = JsonConvert.DeserializeObject<UserExposureSummary>(userExposureSummaryPrefString);
            AssertUserExposureInfo(userExposureSummaryPref, userExposureSummary);
        }

        [Fact]
        public async Task Migrate_100to123Async()
        {
            var (
                startDateTime,
                termsOfServiceLastUpdateDate, privacyPolicyLastUpdateDate,
                lastProcesTekTimestamp,
                userExposureInfo1, userExposureInfo2, userExposureSummary
            ) = await SetUpVersion100(hasAppVersionAtPreference: false);

            _mockEssentialService.SetupGet(x => x.AppVersion).Returns("1.2.3");

            await CreateService()
                .MigrateAsync();

            var preferenceAppVersion = _dummyPreferencesService.GetValue<string>(PreferenceKey.AppVersion, null);
            Assert.Equal("1.2.3", preferenceAppVersion);

            // Application-properties must not be exist
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureSummary));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureInformation));

            // StartDateTime
            var startDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.StartDateTime, new DateTime());
            Assert.Equal(startDateTime, startDateTimePref);

            // TermsOfServiceLastUpdateDateTime
            var termsOfServiceLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.TermsOfServiceLastUpdateDateTime, new DateTime());
            Assert.Equal(termsOfServiceLastUpdateDate, termsOfServiceLastUpdateDateTimePref);

            // PrivacyPolicyLastUpdateDateTime
            var privacyPolicyLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.PrivacyPolicyLastUpdateDateTime, new DateTime());
            Assert.Equal(privacyPolicyLastUpdateDate, privacyPolicyLastUpdateDateTimePref);

            // LastProcessTekTimestamp
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            var lastProcessTekTimestampPrefString = _dummyPreferencesService.GetValue(PreferenceKey.LastProcessTekTimestamp, "{}");
            var lastProcessTekTimestampPref = JsonConvert.DeserializeObject<IDictionary<string, long>>(lastProcessTekTimestampPrefString);
            Assert.Equal(lastProcesTekTimestamp, lastProcessTekTimestampPref);

            // ExposureNotificationConfiguration
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));

            // ExposureInformation
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureInformation));
            var userExposureInfosPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureInformation, "{}");
            var userExposureInfosPref = JsonConvert.DeserializeObject<ObservableCollection<UserExposureInfo>>(userExposureInfosPrefString);
            Assert.Equal(2, userExposureInfosPref.Count);
            AssertUserExposureInfo(userExposureInfo1, userExposureInfosPref[0]);
            AssertUserExposureInfo(userExposureInfo2, userExposureInfosPref[1]);

            // ExposureSummary
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureSummary));
            var userExposureSummaryPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureSummary, "{}");
            var userExposureSummaryPref = JsonConvert.DeserializeObject<UserExposureSummary>(userExposureSummaryPrefString);
            AssertUserExposureInfo(userExposureSummaryPref, userExposureSummary);
        }

        class UserDataModelForTest
        {
            public string StartDateTime { get; set; }
            public bool IsOptined { get; set; } = false;
            public bool IsPolicyAccepted { get; set; } = false;
            public Dictionary<string, long> LastProcessTekTimestamp { get; set; } = new Dictionary<string, long>();
            public ObservableCollection<UserExposureInfo> ExposureInformation { get; set; } = new ObservableCollection<UserExposureInfo>();
            public UserExposureSummary ExposureSummary { get; set; }
        }

        [Fact]
        public async Task Migrate_Corrupt100to123Async()
        {
            var (
                startDateTime,
                termsOfServiceLastUpdateDate, privacyPolicyLastUpdateDate,
                lastProcesTekTimestamp,
                userExposureInfo1, userExposureInfo2, userExposureSummary
            ) = await SetUpVersion100(hasAppVersionAtPreference: false);

            // Save corrupted datetime
            string userDataString = (string)_dummyApplicationPropertyService.GetProperties(APPLICATION_PROPERTY_USER_DATA_KEY);
            var userData = JsonConvert.DeserializeObject<UserDataModelForTest>(userDataString);
            userData.StartDateTime = "Corrupted Datetime Format";
            userDataString = JsonConvert.SerializeObject(userData);
            await _dummyApplicationPropertyService.SavePropertiesAsync(APPLICATION_PROPERTY_USER_DATA_KEY, userDataString);

            _mockEssentialService.SetupGet(x => x.AppVersion).Returns("1.2.3");

            await CreateService()
                .MigrateAsync();

            var preferenceAppVersion = _dummyPreferencesService.GetValue<string>(PreferenceKey.AppVersion, null);
            Assert.Equal("1.2.3", preferenceAppVersion);

            // Application-properties must not be exist
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureSummary));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureInformation));

            // StartDateTime
            var startDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.StartDateTime, new DateTime());
            Assert.NotEqual(startDateTime, startDateTimePref);

            // TermsOfServiceLastUpdateDateTime
            var termsOfServiceLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.TermsOfServiceLastUpdateDateTime, new DateTime());
            Assert.Equal(new DateTime(), termsOfServiceLastUpdateDateTimePref);

            // PrivacyPolicyLastUpdateDateTime
            var privacyPolicyLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.PrivacyPolicyLastUpdateDateTime, new DateTime());
            Assert.Equal(new DateTime(), privacyPolicyLastUpdateDateTimePref);

            // LastProcessTekTimestamp
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            var lastProcessTekTimestampPrefString = _dummyPreferencesService.GetValue(PreferenceKey.LastProcessTekTimestamp, "{}");
            var lastProcessTekTimestampPref = JsonConvert.DeserializeObject<IDictionary<string, long>>(lastProcessTekTimestampPrefString);
            Assert.Equal(lastProcesTekTimestamp, lastProcessTekTimestampPref);

            // ExposureNotificationConfiguration
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));

            // ExposureInformation
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureInformation));
            var userExposureInfosPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureInformation, "{}");
            var userExposureInfosPref = JsonConvert.DeserializeObject<ObservableCollection<UserExposureInfo>>(userExposureInfosPrefString);
            Assert.Equal(2, userExposureInfosPref.Count);
            AssertUserExposureInfo(userExposureInfo1, userExposureInfosPref[0]);
            AssertUserExposureInfo(userExposureInfo2, userExposureInfosPref[1]);

            // ExposureSummary
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureSummary));
            var userExposureSummaryPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureSummary, "{}");
            var userExposureSummaryPref = JsonConvert.DeserializeObject<UserExposureSummary>(userExposureSummaryPrefString);
            AssertUserExposureInfo(userExposureSummaryPref, userExposureSummary);
        }

        private async Task<(DateTime, DateTime, DateTime, Dictionary<string, long>, UserExposureInfo, UserExposureInfo, UserExposureSummary)> SetUpVersion122(
            bool isOptIned = true,
            bool isPrivaryPolicyAgreed = true,
            bool hasAppVersionAtPreference = true)
        {
            var values = await SetUpVersion100(isOptIned, isPrivaryPolicyAgreed, hasAppVersionAtPreference);

            _mockEssentialService.SetupGet(x => x.AppVersion).Returns("1.2.2");

            await CreateService()
                .MigrateAsync();

            // Application-properties must not be exist
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_USER_DATA_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_TERMS_OF_SERVICE_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(APPLICATION_PROPERTY_PRIVACY_POLICY_LAST_UPDATE_DATE_KEY));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureSummary));
            Assert.False(_dummyApplicationPropertyService.ContainsKey(PreferenceKey.ExposureInformation));

            if (!hasAppVersionAtPreference)
            {
                // Remove AppVersion from preference
                _dummyPreferencesService.RemoveValue(PreferenceKey.AppVersion);
            }

            return values;
        }

        [Fact]
        public async Task Migrate_122to123Async()
        {
            var (
                startDateTime,
                termsOfServiceLastUpdateDate, privacyPolicyLastUpdateDate,
                lastProcesTekTimestamp,
                userExposureInfo1, userExposureInfo2, userExposureSummary
            ) = await SetUpVersion122(hasAppVersionAtPreference: false);

            _mockEssentialService.SetupGet(x => x.AppVersion).Returns("1.2.3");

            await CreateService()
                .MigrateAsync();

            // AppVersion
            var preferenceAppVersion = _dummyPreferencesService.GetValue<string>(PreferenceKey.AppVersion, null);
            Assert.Equal("1.2.3", preferenceAppVersion);

            // StartDateTime
            var startDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.StartDateTime, new DateTime());
            Assert.Equal(startDateTime, startDateTimePref);

            // TermsOfServiceLastUpdateDateTime
            var termsOfServiceLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.TermsOfServiceLastUpdateDateTime, new DateTime());
            Assert.Equal(termsOfServiceLastUpdateDate, termsOfServiceLastUpdateDateTimePref);

            // PrivacyPolicyLastUpdateDateTime
            var privacyPolicyLastUpdateDateTimePref = _dummyPreferencesService.GetValue(PreferenceKey.PrivacyPolicyLastUpdateDateTime, new DateTime());
            Assert.Equal(privacyPolicyLastUpdateDate, privacyPolicyLastUpdateDateTimePref);

            // LastProcessTekTimestamp
            Assert.True(_dummyPreferencesService.ContainsKey(PreferenceKey.LastProcessTekTimestamp));
            var lastProcessTekTimestampPrefString = _dummyPreferencesService.GetValue(PreferenceKey.LastProcessTekTimestamp, "{}");
            var lastProcessTekTimestampPref = JsonConvert.DeserializeObject<IDictionary<string, long>>(lastProcessTekTimestampPrefString);
            Assert.Equal(lastProcesTekTimestamp, lastProcessTekTimestampPref);

            // ExposureNotificationConfiguration
            Assert.False(_dummyPreferencesService.ContainsKey(PreferenceKey.ExposureNotificationConfiguration));

            // ExposureInformation
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureInformation));
            var userExposureInfosPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureInformation, "{}");
            var userExposureInfosPref = JsonConvert.DeserializeObject<ObservableCollection<UserExposureInfo>>(userExposureInfosPrefString);
            Assert.Equal(2, userExposureInfosPref.Count);
            AssertUserExposureInfo(userExposureInfo1, userExposureInfosPref[0]);
            AssertUserExposureInfo(userExposureInfo2, userExposureInfosPref[1]);

            // ExposureSummary
            Assert.True(_dummySecureStorageService.ContainsKey(PreferenceKey.ExposureSummary));
            var userExposureSummaryPrefString = _dummySecureStorageService.GetValue(PreferenceKey.ExposureSummary, "{}");
            var userExposureSummaryPref = JsonConvert.DeserializeObject<UserExposureSummary>(userExposureSummaryPrefString);
            AssertUserExposureInfo(userExposureSummaryPref, userExposureSummary);
        }

        private static void AssertUserExposureInfo(UserExposureInfo expected, UserExposureInfo actual)
        {
            Assert.Equal(expected.AttenuationValue, actual.AttenuationValue);
            Assert.Equal(expected.Duration, actual.Duration);
            Assert.Equal(expected.Timestamp, actual.Timestamp);
            Assert.Equal(expected.TotalRiskScore, actual.TotalRiskScore);
            Assert.Equal(expected.TransmissionRiskLevel, actual.TransmissionRiskLevel);
        }

        private static void AssertUserExposureInfo(UserExposureSummary expected, UserExposureSummary actual)
        {
            Assert.Equal(expected.AttenuationDurations, actual.AttenuationDurations);
            Assert.Equal(expected.DaysSinceLastExposure, actual.DaysSinceLastExposure);
            Assert.Equal(expected.HighestRiskScore, actual.HighestRiskScore);
            Assert.Equal(expected.MatchedKeyCount, actual.MatchedKeyCount);
            Assert.Equal(expected.SummationRiskScore, actual.SummationRiskScore);
        }

    }

    class InMemoryApplicationPropertyService : IApplicationPropertyService
    {
        public Dictionary<string, object> _dict = new Dictionary<string, object>();

        public bool ContainsKey(string key) => _dict.ContainsKey(key);

        public object GetProperties(string key) => _dict.GetValueOrDefault(key);

        public Task Remove(string key) => Task.Run(() => RemoveValue(key));

        public void RemoveValue(string key) => _dict.Remove(key);

        public Task SavePropertiesAsync(string key, object property) => Task.Run(() => SetValue(key, property));

        public void SetValue<T>(string key, T value) => _dict[key] = value;
    }

    class InMemoryPreferencesService : IPreferencesService
    {
        public Dictionary<string, object> _dict = new Dictionary<string, object>();

        public bool ContainsKey(string key) => _dict.ContainsKey(key);

        public T GetValue<T>(string key, T defaultValue = default) => (T)_dict.GetValueOrDefault(key, defaultValue);

        public void RemoveValue(string key) => _dict.Remove(key);

        public void SetValue<T>(string key, T value) => _dict[key] = value;
    }

    class InMemorySecureStorageService : ISecureStorageService
    {
        public Dictionary<string, object> _dict = new Dictionary<string, object>();

        public bool ContainsKey(string key) => _dict.ContainsKey(key);

        public T GetValue<T>(string key, T defaultValue = default) => (T)_dict.GetValueOrDefault(key, defaultValue);

        public void RemoveValue(string key) => _dict.Remove(key);

        public void SetValue<T>(string key, T value) => _dict[key] = value;
    }
}