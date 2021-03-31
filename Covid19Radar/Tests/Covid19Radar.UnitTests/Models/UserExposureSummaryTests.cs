/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Model;
using Newtonsoft.Json;
using Xunit;

namespace Covid19Radar.UnitTests.Models
{
    public class UserExposureSummaryTests
    {
        public UserExposureSummaryTests()
        {
        }

        [Fact]
        public void JsonSerializeTests()
        {
            var testDaysSinceLastExposure = 1;
            var testMatchedKeyCount = 2UL;
            var testHighestRiskScore = 30;
            var testAttenuationDurations = new TimeSpan[] { new TimeSpan(1, 2, 3, 4, 5), new TimeSpan(2, 3, 4, 5, 6) };
            var testSummationRiskScore = 10;

            var testExposureSummary = new UserExposureSummary(testDaysSinceLastExposure, testMatchedKeyCount, testHighestRiskScore, testAttenuationDurations, testSummationRiskScore);
            var serializedJson = JsonConvert.SerializeObject(testExposureSummary);

            Assert.NotEmpty(serializedJson);
            Assert.Contains($"\"DaysSinceLastExposure\":{testDaysSinceLastExposure}", serializedJson);
            Assert.Contains($"\"MatchedKeyCount\":{testMatchedKeyCount}", serializedJson);
            Assert.Contains($"\"HighestRiskScore\":{testHighestRiskScore}", serializedJson);
            Assert.Contains("\"AttenuationDurations\":[\"1.02:03:04.0050000\",\"2.03:04:05.0060000\"]", serializedJson);
            Assert.Contains($"\"SummationRiskScore\":{testSummationRiskScore}", serializedJson);
        }

        [Fact]
        public void JsonDeserializeTests()
        {
            var testJson = "{\"DaysSinceLastExposure\":1,\"MatchedKeyCount\":2,\"HighestRiskScore\":30,\"AttenuationDurations\":[\"1.02:03:04.0050000\",\"2.03:04:05.0060000\"],\"SummationRiskScore\":10}";
            var deserializedExposureSummary = JsonConvert.DeserializeObject<UserExposureSummary>(testJson);

            Assert.Equal(1, deserializedExposureSummary.DaysSinceLastExposure);
            Assert.Equal(2UL, deserializedExposureSummary.MatchedKeyCount);
            Assert.Equal(30, deserializedExposureSummary.HighestRiskScore);
            Assert.Equal(new TimeSpan[] { new TimeSpan(1, 2, 3, 4, 5), new TimeSpan(2, 3, 4, 5, 6) }, deserializedExposureSummary.AttenuationDurations);
            Assert.Equal(10, deserializedExposureSummary.SummationRiskScore);
        }
    }
}
