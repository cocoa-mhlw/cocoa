using Covid19Radar.Background.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.Background.Services
{
    [TestClass]
    [TestCategory("Services")]
    public class TemporaryExposureKeySignServiceTest
    {
        // openssl ecparam -name prime256v1 -genkey -noout -out private.pem
        // openssl ec -in private.pem -pubout -out public.pem
        // cat private.pem
        private const string PrivateKey = "MHcCAQEEIFdaIjZgfoIi+Tvck6OkJNrUL7BL3vLOeHAwno/mdh49oAoGCCqGSM49AwEHoUQDQgAEQwqCWDLkl+g+4bwTQgoRMbR7Z+Dz3wfNbAQhB2ja07q7UN7Bwa45HYJXkZlXqEQVb9c+SPuW4fDZnlMuQeIw+Q==";
        // cat public.pem
        private const string PublicKey = "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEQwqCWDLkl+g+4bwTQgoRMbR7Z+Dz3wfNbAQhB2ja07q7UN7Bwa45HYJXkZlXqEQVb9c+SPuW4fDZnlMuQeIw+Q==";

        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["VerificationKeySecret"]).Returns(PrivateKey);
            var logger = new Mock.LoggerMock<TemporaryExposureKeySignService>();
            // action
            var service = new TemporaryExposureKeySignService(config.Object, logger);
        }

        [TestMethod]
        public async Task SignAsyncMethod()
        {
            // preparation
            var config = new Mock<IConfiguration>();
            config.Setup(_ => _["VerificationKeySecret"]).Returns(PrivateKey);
            var logger = new Mock.LoggerMock<TemporaryExposureKeySignService>();
            var service = new TemporaryExposureKeySignService(config.Object, logger);
            using var memory = new MemoryStream();
            using var writer = new StreamWriter(memory);
            await writer.WriteAsync("TEST");
            await writer.FlushAsync();
            memory.Position = 0;
            // action
            var result = await service.SignAsync(memory);
            // Assert
            memory.Position = 0;
            var verify = service.VerificationKey.VerifyData(memory, result, HashAlgorithmName.SHA256);
            Assert.AreEqual(true, verify);
            // Assert public key
            memory.Position = 0;
            var verifyKey = ECDsa.Create();
            int read;
            verifyKey.ImportSubjectPublicKeyInfo(Convert.FromBase64String(PublicKey), out read);
            var verify2 = verifyKey.VerifyData(memory, result, HashAlgorithmName.SHA256);
            Assert.AreEqual(true, verify2);
        }
    }
}
