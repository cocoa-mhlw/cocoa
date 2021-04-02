/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Net.Http;
using Covid19Radar.Services;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class HttpClientServiceTests
    {
        public HttpClientServiceTests()
        {
        }

        public HttpClientService CreateService()
        {
            return new HttpClientService();
        }

        [Fact]
        public void CreateTest()
        {
            var unitUnderTest = CreateService();
            var httpClient = unitUnderTest.Create();
            Assert.IsType<HttpClient>(httpClient);
        }
    }
}
