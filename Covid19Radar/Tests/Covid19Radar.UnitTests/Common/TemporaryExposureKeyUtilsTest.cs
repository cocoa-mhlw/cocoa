using System;
using System.Collections.Generic;
using System.Linq;
using Chino;
using Covid19Radar.Common;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Common
{
    public class TemporaryExposureKeyUtilsTest
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;

        public TemporaryExposureKeyUtilsTest()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
        }

        [Theory]
        [InlineData("2020-11-26T00:00:00+09:00", 0, "", "")]
        [InlineData("2020-11-21T00:00:00+09:00", 3, "2020-11-20T00:00:00+00:00", "2020-11-18T00:00:00+00:00")]
        [InlineData("2020-11-20T00:00:00+09:00", 4, "2020-11-20T00:00:00+00:00", "2020-11-17T00:00:00+00:00")]
        [InlineData("2020-11-19T00:00:00+09:00", 5, "2020-11-20T00:00:00+00:00", "2020-11-16T00:00:00+00:00")]
        [InlineData("2020-11-18T00:00:00+09:00", 6, "2020-11-20T00:00:00+00:00", "2020-11-15T00:00:00+00:00")]
        [InlineData("2020-11-17T00:00:00+09:00", 8, "2020-11-20T00:00:00+00:00", "2020-11-13T15:00:00+00:00")]
        [InlineData("2020-11-16T00:00:00+09:00", 10, "2020-11-20T00:00:00+00:00", "2020-11-13T00:00:00+00:00")]
        [InlineData("2020-11-15T00:00:00+09:00", 11, "2020-11-20T00:00:00+00:00", "2020-11-12T00:00:00+00:00")]
        [InlineData("2020-11-14T00:00:00+09:00", 12, "2020-11-20T00:00:00+00:00", "2020-11-11T00:00:00+00:00")]
        [InlineData("2020-11-13T00:00:00+09:00", 13, "2020-11-20T00:00:00+00:00", "2020-11-10T00:00:00+00:00")]
        [InlineData("2020-11-12T00:00:00+09:00", 14, "2020-11-20T00:00:00+00:00", "2020-11-09T00:00:00+00:00")]
        [InlineData("2020-11-11T00:00:00+09:00", 15, "2020-11-20T00:00:00+00:00", "2020-11-08T00:00:00+00:00")]
        [InlineData("2020-11-10T00:00:00+09:00", 16, "2020-11-20T00:00:00+00:00", "2020-11-07T00:00:00+00:00")]
        [InlineData("2020-11-09T00:00:00+09:00", 17, "2020-11-20T00:00:00+00:00", "2020-11-06T00:00:00+00:00")]
        [InlineData("2020-11-08T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-07T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-06T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-05T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-04T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        public void FliterTemporaryExposureKeysTests_Success_V1_V2_DiagnosisApi(string diagnosisDateTest, int expectedCount, string expectedFirst, string expedtedLast)
        {
            var temporaryExposureKeys = new List<TemporaryExposureKey>() {
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-20T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-19T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-18T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-17T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-16T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-15T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-14T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-13T15:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-13T14:59:59+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-13T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-12T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-11T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-10T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-09T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-08T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-07T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-06T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-05T00:00:00+00:00").ToEnInterval() },
            };

            var diagnosisDate = DateTime.Parse(diagnosisDateTest);

            IList<TemporaryExposureKey> filteredTemporaryExposureKeys = TemporaryExposureKeyUtils.FiilterTemporaryExposureKeys(
                temporaryExposureKeys,
                diagnosisDate,
                -3,
                mockLoggerService.Object
                );

            var actualCount = filteredTemporaryExposureKeys.Count();
            Assert.Equal(expectedCount, actualCount);
            if (actualCount > 0)
            {
                Assert.Equal(DateTime.Parse(expectedFirst).ToEnInterval(), filteredTemporaryExposureKeys.First().RollingStartIntervalNumber);
                Assert.Equal(DateTime.Parse(expedtedLast).ToEnInterval(), filteredTemporaryExposureKeys.Last().RollingStartIntervalNumber);
            }
        }

        [Theory]
        [InlineData("2020-12-31T00:00:00+09:00", 0, "", "")]
        [InlineData("2020-11-21T00:00:00+09:00", 17, "2020-11-20T00:00:00+00:00", "2020-11-06T00:00:00+00:00")]
        [InlineData("2020-11-20T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-19T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-18T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-17T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-16T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-15T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-14T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-13T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-12T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-11T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-10T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-09T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-08T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-07T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-06T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-05T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        [InlineData("2020-11-04T00:00:00+09:00", 18, "2020-11-20T00:00:00+00:00", "2020-11-05T00:00:00+00:00")]
        public void FliterTemporaryExposureKeysTests_Success_V3DiagnosisApi(string diagnosisDateTest, int expectedCount, string expectedFirst, string expedtedLast)
        {
            var temporaryExposureKeys = new List<TemporaryExposureKey>() {
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-20T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-19T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-18T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-17T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-16T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-15T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-14T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-13T15:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-13T14:59:59+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-13T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-12T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-11T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-10T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-09T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-08T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-07T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-06T00:00:00+00:00").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-05T00:00:00+00:00").ToEnInterval() },
            };

            var diagnosisDate = DateTime.Parse(diagnosisDateTest);

            IList<TemporaryExposureKey> filteredTemporaryExposureKeys = TemporaryExposureKeyUtils.FiilterTemporaryExposureKeys(
                temporaryExposureKeys,
                diagnosisDate,
                -15,
                mockLoggerService.Object
                );

            var actualCount = filteredTemporaryExposureKeys.Count();
            Assert.Equal(expectedCount, actualCount);
            if (actualCount > 0)
            {
                Assert.Equal(DateTime.Parse(expectedFirst).ToEnInterval(), filteredTemporaryExposureKeys.First().RollingStartIntervalNumber);
                Assert.Equal(DateTime.Parse(expedtedLast).ToEnInterval(), filteredTemporaryExposureKeys.Last().RollingStartIntervalNumber);
            }
        }

    }
}
