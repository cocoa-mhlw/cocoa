using System;
using Covid19Radar.Model;
using Covid19Radar.Repository;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Repository
{
    public class UserDataRepositoryTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<IPreferencesService> mockPreferencesService;

        public UserDataRepositoryTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockPreferencesService = mockRepository.Create<IPreferencesService>();
        }

        private IUserDataRepository CreateRepository()
            => new UserDataRepository(mockPreferencesService.Object, mockLoggerService.Object);

        [Theory]
        [InlineData(TermsType.TermsOfService)]
        [InlineData(TermsType.PrivacyPolicy)]
        public void IsReAgreeTest_InfoIsEmpty(TermsType termsType)
        {
            var userDataRepository = CreateRepository();
            var result = userDataRepository.IsReAgree(termsType, new TermsUpdateInfoModel());
            Assert.False(result);
        }

        [Theory]
        [InlineData(TermsType.TermsOfService, "TermsOfServiceLastUpdateDateTime")]
        [InlineData(TermsType.PrivacyPolicy, "PrivacyPolicyLastUpdateDateTime")]
        public void IsReAgreeTest_NotSaveLastUpdateDateTime(TermsType termsType, string key)
        {
            var info = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { UpdateDateTime = DateTime.Now },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { UpdateDateTime = DateTime.Now }
            };

            var userDataRepository = CreateRepository();
            mockPreferencesService.Setup(x => x.ContainsKey(key)).Returns(false);
            var result = userDataRepository.IsReAgree(termsType, info);
            Assert.True(result);
        }

        [Theory]
        [InlineData(TermsType.TermsOfService, "TermsOfServiceLastUpdateDateTime")]
        [InlineData(TermsType.PrivacyPolicy, "PrivacyPolicyLastUpdateDateTime")]
        public void IsReAgreeTest_RequiredReAgree(TermsType termsType, string key)
        {
            var info = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { UpdateDateTime = new DateTime(2020, 11, 2) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { UpdateDateTime = new DateTime(2020, 11, 2) }
            };

            var userDataRepository = CreateRepository();
            mockPreferencesService.Setup(x => x.ContainsKey(key)).Returns(true);
            mockPreferencesService.Setup(x => x.GetValue(key, new DateTime())).Returns(new DateTime(2020, 11, 1));
            var result = userDataRepository.IsReAgree(termsType, info);
            Assert.True(result);
        }

        [Theory]
        [InlineData(TermsType.TermsOfService, "TermsOfServiceLastUpdateDateTime")]
        [InlineData(TermsType.PrivacyPolicy, "PrivacyPolicyLastUpdateDateTime")]
        public void IsReAgreeTest_NoNeedReAgree(TermsType termsType, string key)
        {
            var info = new TermsUpdateInfoModel
            {
                TermsOfService = new TermsUpdateInfoModel.Detail { UpdateDateTime = new DateTime(2020, 11, 2) },
                PrivacyPolicy = new TermsUpdateInfoModel.Detail { UpdateDateTime = new DateTime(2020, 11, 2) }
            };

            var userDataRepository = CreateRepository();
            mockPreferencesService.Setup(x => x.ContainsKey(key)).Returns(true);
            mockPreferencesService.Setup(x => x.GetValue(key, new DateTime())).Returns(new DateTime(2020, 11, 3));
            var result = userDataRepository.IsReAgree(termsType, info);
            Assert.False(result);
        }
    }
}
