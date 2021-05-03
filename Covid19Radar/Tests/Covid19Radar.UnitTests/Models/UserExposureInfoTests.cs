/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Model;
using Newtonsoft.Json;
using Xunit;

namespace Covid19Radar.UnitTests.Models
{
    public class UserExposureInfoTests
    {
        public UserExposureInfoTests()
        {
        }

        [Fact]
        public void JsonSerializeTests()
        {
            var testTimestamp = new DateTime(2020, 12, 21, 9, 3, 30);
            var testDuration = new TimeSpan(1, 2, 3, 4, 5);
            var testAttenuationValue = 1;
            var testTotalRiskScore = 20;
            var testRiskLevel = UserRiskLevel.Medium;

            var testExposureInfo = new UserExposureInfo(testTimestamp, testDuration, testAttenuationValue, testTotalRiskScore, testRiskLevel);
            var serializedJson = JsonConvert.SerializeObject(testExposureInfo);

            Assert.NotEmpty(serializedJson);
            Assert.Contains("\"Timestamp\":\"2020-12-21T09:03:30\"", serializedJson);
            Assert.Contains("\"Duration\":\"1.02:03:04.0050000\"", serializedJson);
            Assert.Contains($"\"AttenuationValue\":{testAttenuationValue}", serializedJson);
            Assert.Contains($"\"TotalRiskScore\":{testTotalRiskScore}", serializedJson);
            Assert.Contains($"\"TransmissionRiskLevel\":{(int)testRiskLevel}", serializedJson);
        }

        [Fact]
        public void JsonDeserializeTests()
        {
            var testJson = "{\"Timestamp\":\"2020-12-21T10:15:30\",\"Duration\":\"1.02:03:04.005\",\"AttenuationValue\":2,\"TotalRiskScore\":19,\"TransmissionRiskLevel\":4}";
            var deserializedExposureInfo = JsonConvert.DeserializeObject<UserExposureInfo>(testJson);

            Assert.Equal(new DateTime(2020, 12, 21, 10, 15, 30), deserializedExposureInfo.Timestamp);
            Assert.Equal(new TimeSpan(1, 2, 3, 4, 5), deserializedExposureInfo.Duration);
            Assert.Equal(2, deserializedExposureInfo.AttenuationValue);
            Assert.Equal(19, deserializedExposureInfo.TotalRiskScore);
            Assert.Equal(UserRiskLevel.Medium, deserializedExposureInfo.TransmissionRiskLevel);
        }
    }
}
