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
        // openssl ecparam -name prime256v1 -genkey -noout -out eckey.pem
        // openssl ec -in eckey.pem -pubout -out public.pem
        // openssl pkcs8 -topk8 -nocrypt -in eckey.pem -out pkcs8.pem
        // cat pkcs8.pem
        private const string PrivateKey = "MIGHAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBG0wawIBAQQgH4+cFTXcU8NXlKUI1mTrtK4zYu0Somn4ih5Jf2j4JAGhRANCAASqLOB9nSvWxRq3bWVqfh34eV7GFxJmgcQSDxSwmcFbADLEr4AHBC2yQF/hoRj007vdtSNX9wKqDnDsU5SZOy9n";

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
        }
    }
}
