using System;
using Covid19Radar.Model;
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
        private readonly Mock<IApplicationPropertyService> mockApplicationPropertyService;

        public TermsUpdateServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockApplicationPropertyService = mockRepository.Create<IApplicationPropertyService>();
        }

        private TermsUpdateService CreateService()
        {
            return new TermsUpdateService(mockLoggerService.Object, mockApplicationPropertyService.Object);
        }

        [Theory]
        [InlineData(TermsType.TermsOfService)]
        [InlineData(TermsType.PrivacyPolicy)]
        public void IsReAgreeTest_InfoIsEmpty(TermsType termsType)
        {
            var termsUpdateService = CreateService();
            var result = termsUpdateService.IsReAgree(termsType, new TermsUpdateInfoModel());
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

            var termsUpdateService = CreateService();
            mockApplicationPropertyService.Setup(x => x.ContainsKey(key)).Returns(false);
            var result = termsUpdateService.IsReAgree(termsType, info);
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

            var termsUpdateService = CreateService();
            mockApplicationPropertyService.Setup(x => x.ContainsKey(key)).Returns(true);
            mockApplicationPropertyService.Setup(x => x.GetProperties(key)).Returns(new DateTime(2020, 11, 1));
            var result = termsUpdateService.IsReAgree(termsType, info);
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

            var termsUpdateService = CreateService();
            mockApplicationPropertyService.Setup(x => x.ContainsKey(key)).Returns(true);
            mockApplicationPropertyService.Setup(x => x.GetProperties(key)).Returns(new DateTime(2020, 11, 3));
            var result = termsUpdateService.IsReAgree(termsType, info);
            Assert.False(result);
        }
    }
}
