using System;
using System.Collections.Generic;
using System.Linq;
using Chino;
using Covid19Radar.Common;
using Xunit;

namespace Covid19Radar.UnitTests.Common
{
    public class TemporaryExposureKeyUtilsTest
    {
        [Theory]
        [InlineData("2020-11-22T00:00:00+09:00", 0, "", "")]
        [InlineData("2020-11-21T00:00:00+09:00", 1, "2020-11-18T00:00:00Z", "2020-11-18T00:00:00Z")]
        [InlineData("2020-11-20T00:00:00+09:00", 2, "2020-11-18T00:00:00Z", "2020-11-17T00:00:00Z")]
        [InlineData("2020-11-19T00:00:00+09:00", 3, "2020-11-18T00:00:00Z", "2020-11-16T00:00:00Z")]
        [InlineData("2020-11-18T00:00:00+09:00", 4, "2020-11-18T00:00:00Z", "2020-11-15T00:00:00Z")]
        [InlineData("2020-11-17T00:00:00+09:00", 6, "2020-11-18T00:00:00Z", "2020-11-13T15:00:00Z")]
        [InlineData("2020-11-16T00:00:00+09:00", 8, "2020-11-18T00:00:00Z", "2020-11-13T00:00:00Z")]
        [InlineData("2020-11-15T00:00:00+09:00", 9, "2020-11-18T00:00:00Z", "2020-11-12T00:00:00Z")]
        [InlineData("2020-11-14T00:00:00+09:00", 9, "2020-11-18T00:00:00Z", "2020-11-12T00:00:00Z")]
        public void FliterTemporaryExposureKeysTests_Success(string diagnosisDateTest, int expectedCount, string expectedFirst, string expedtedLast)
        {
            var temporaryExposureKeys = new List<TemporaryExposureKey>() {
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-18T00:00:00Z").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-17T00:00:00Z").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-16T00:00:00Z").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-15T00:00:00Z").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-14T00:00:00Z").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-13T15:00:00Z").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-13T14:59:59Z").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-13T00:00:00Z").ToEnInterval() },
                new TemporaryExposureKey { RollingStartIntervalNumber = DateTime.Parse("2020-11-12T00:00:00Z").ToEnInterval() },
            };

            var diagnosisDate = DateTime.Parse(diagnosisDateTest);

            IList<TemporaryExposureKey> filteredTemporaryExposureKeys = TemporaryExposureKeyUtils.FiilterTemporaryExposureKeys(
                temporaryExposureKeys,
                diagnosisDate,
                AppConstants.DaysToSendTek
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
