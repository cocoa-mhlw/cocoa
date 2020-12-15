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
