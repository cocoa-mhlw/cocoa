/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class TermsUpdateServiceTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IPreferencesService> mockPreferencesService;
        private readonly Mock<IUserDataRepository> mockUserDataRepository;

        public TermsUpdateServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockPreferencesService = mockRepository.Create<IPreferencesService>();
            mockUserDataRepository = mockRepository.Create<IUserDataRepository>();
        }

        private TermsUpdateService CreateService()
        {
            return new TermsUpdateService(
                mockLoggerService.Object,
                mockUserDataRepository.Object
                );
        }

        [Theory]
        [InlineData(TermsType.TermsOfService)]
        [InlineData(TermsType.PrivacyPolicy)]
        public void IsUpdatedTest_InfoIsEmpty(TermsType termsType)
        {
            var termsUpdateService = CreateService();
            var result = termsUpdateService.IsUpdated(termsType, new TermsUpdateInfoModel());
            Assert.False(result);
        }

        [Theory]
        [InlineData(TermsType.TermsOfService, "TermsOfServiceLastUpdateDateTimeEpoch")]
        [InlineData(TermsType.PrivacyPolicy, "PrivacyPolicyLastUpdateDateTimeEpoch")]
        public void IsUpdatedTest_NotSaveLastUpdateDateTime(TermsType termsType, string key)
        {
            var info = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { UpdateDateTimeJst = DateTime.UtcNow },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { UpdateDateTimeJst = DateTime.UtcNow }
            };

            var termsUpdateService = CreateService();
            mockPreferencesService.Setup(x => x.ContainsKey(key)).Returns(false);
            var result = termsUpdateService.IsUpdated(termsType, info);
            Assert.True(result);
        }

        [Theory]
        [InlineData(TermsType.TermsOfService, "TermsOfServiceLastUpdateDateTimeEpoch")]
        [InlineData(TermsType.PrivacyPolicy, "PrivacyPolicyLastUpdateDateTimeEpoch")]
        public void IsUpdatedTest_RequiredReAgree(TermsType termsType, string key)
        {
            var info = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { UpdateDateTimeJst = new DateTime(2020, 11, 2) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { UpdateDateTimeJst = new DateTime(2020, 11, 2) }
            };

            var termsUpdateService = CreateService();
            mockPreferencesService.Setup(x => x.ContainsKey(key)).Returns(true);
            mockUserDataRepository.Setup(x => x.GetLastUpdateDate(termsType)).Returns(new DateTime(2020, 11, 1));
            var result = termsUpdateService.IsUpdated(termsType, info);
            Assert.True(result);
        }

        [Theory]
        [InlineData(TermsType.TermsOfService, "TermsOfServiceLastUpdateDateTimeEpoch")]
        [InlineData(TermsType.PrivacyPolicy, "PrivacyPolicyLastUpdateDateTimeEpoch")]
        public void IsUpdatedTest_NoNeedReAgree(TermsType termsType, string key)
        {
            var info = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { UpdateDateTimeJst = new DateTime(2020, 11, 2) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { UpdateDateTimeJst = new DateTime(2020, 11, 2) }
            };

            var termsUpdateService = CreateService();
            mockPreferencesService.Setup(x => x.ContainsKey(key)).Returns(true);
            mockUserDataRepository.Setup(x => x.GetLastUpdateDate(termsType)).Returns(new DateTime(2020, 11, 3));
            var result = termsUpdateService.IsUpdated(termsType, info);
            Assert.False(result);
        }
    }
}
