using Castle.Core.Configuration;
using Covid19Radar.Api;
using Covid19Radar.Api.Common;
using Covid19Radar.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.Common.Services
{
    [TestClass]
    [TestCategory("Common")]
    public class CryptionServiceTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            config.Setup(_ => _.GetSection("CRYPTION_KEY").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            config.Setup(_ => _.GetSection("CRYPTION_HASH").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_KEY2").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV2").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            var logger = new Mock<ILogger<CryptionService>>();
            // action
            var instance = new CryptionService(config.Object, logger.Object);
        }

        [DataTestMethod]
        [DataRow('X', 1, true)]
        [DataRow('X', 2, true)]
        [DataRow('X', 4, true)]
        [DataRow('X', 8, true)]
        [DataRow('X', 16, true)]
        [DataRow('X', 32, true)]
        [DataRow('X', 64, true)]
        [DataRow('X', 128, true)]
        [DataRow('X', 256, true)]
        [DataRow('X', 257, false)]
        [DataRow('X', 512, false)]
        [DataRow('X', 1024, false)]
        public void CreateSecretMethod(char u, int length, bool expected)
        {
            // preparation
            var userUuid = new string(u, length);
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            config.Setup(_ => _.GetSection("CRYPTION_KEY").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            config.Setup(_ => _.GetSection("CRYPTION_HASH").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_KEY2").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV2").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            var logger = new Mock<ILogger<CryptionService>>();
            var instance = new CryptionService(config.Object, logger.Object);
            // action
            var secret = instance.CreateSecret(userUuid);
            var actual = instance.ValidateSecret(userUuid, secret);
            // assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CreateSecretMethodLoad()
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            config.Setup(_ => _.GetSection("CRYPTION_KEY").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            config.Setup(_ => _.GetSection("CRYPTION_HASH").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_KEY2").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV2").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            var logger = new Mock<ILogger<CryptionService>>();
            var instance = new CryptionService(config.Object, logger.Object);
            // action
            for (var i = 0; i < 13000; i++)
            {
                var userUuid = new string('x', i % 255) + "Z";
                var secret = instance.CreateSecret(userUuid);
                var actual = instance.ValidateSecret(userUuid, secret);
                // assert
                Assert.AreEqual(true, actual);
            }
        }

        [TestMethod]
        public void CreateSecretMethodParallelLoad()
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            config.Setup(_ => _.GetSection("CRYPTION_KEY").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            config.Setup(_ => _.GetSection("CRYPTION_HASH").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_KEY2").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV2").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            var logger = new Mock<ILogger<CryptionService>>();
            var instance = new CryptionService(config.Object, logger.Object);
            var option = new ParallelOptions() { MaxDegreeOfParallelism = 512 };
            // action
            Parallel.For(0, 13000, option, i =>
            {
                var userUuid = new string('x', i % 255) + "Z";
                var secret = instance.CreateSecret(userUuid);
                var actual = instance.ValidateSecret(userUuid, secret);
                // assert
                Assert.AreEqual(true, actual);
            });
        }

        [DataTestMethod]
        [DataRow("1", "2")]
        [DataRow("2", "3")]
        [DataRow("11111", "22222")]
        public void CreateSecretMethodError(string userUuid1, string userUuid2)
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            config.Setup(_ => _.GetSection("CRYPTION_KEY").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            config.Setup(_ => _.GetSection("CRYPTION_HASH").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_KEY2").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV2").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            var logger = new Mock<ILogger<CryptionService>>();
            var instance = new CryptionService(config.Object, logger.Object);
            // action
            var secret1 = instance.CreateSecret(userUuid1);
            var secret2 = instance.CreateSecret(userUuid2);
            var actual1 = instance.ValidateSecret(userUuid1, secret1);
            var actual2 = instance.ValidateSecret(userUuid2, secret2);
            var actual3 = instance.ValidateSecret(userUuid2, secret1);
            var actual4 = instance.ValidateSecret(userUuid1, secret2);
            // assert
            Assert.AreEqual(true, actual1);
            Assert.AreEqual(true, actual2);
            Assert.AreEqual(false, actual3);
            Assert.AreEqual(false, actual4);
        }

        [DataTestMethod]
        [DataRow("1", "2", false)]
        [DataRow("", "", false)]
        [DataRow("", null, false)]
        [DataRow(null, "", false)]
        [DataRow(null, null, false)]
        public void ValidateSecretMethodError(string userUuid, string secret, bool expected)
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            config.Setup(_ => _.GetSection("CRYPTION_KEY").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            config.Setup(_ => _.GetSection("CRYPTION_HASH").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_KEY2").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV2").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            var logger = new Mock<ILogger<CryptionService>>();
            var instance = new CryptionService(config.Object, logger.Object);
            // action
            var actual = instance.ValidateSecret(userUuid, secret);
            // assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ValidateSecretMethodParallelLoad()
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            config.Setup(_ => _.GetSection("CRYPTION_KEY").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            config.Setup(_ => _.GetSection("CRYPTION_HASH").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_KEY2").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV2").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            var logger = new Mock<ILogger<CryptionService>>();
            var instance = new CryptionService(config.Object, logger.Object);
            var userUuid = "XXXXXXXXXXXXXXXXXXXXXXXXXX";
            var secret = instance.CreateSecret(userUuid);
            var option = new ParallelOptions() { MaxDegreeOfParallelism = 512 };
            // action
            Parallel.For(0, 13000, option, i =>
            {
                var actual = instance.ValidateSecret(userUuid, secret);
                // assert
                Assert.AreEqual(true, actual);
            });
        }

        [DataTestMethod]
        [DataRow("XXXXXXXXXX")]
        [DataRow("")]
        public void ProtectMethod(string secret)
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            config.Setup(_ => _.GetSection("CRYPTION_KEY").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            config.Setup(_ => _.GetSection("CRYPTION_HASH").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_KEY2").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV2").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            var logger = new Mock<ILogger<CryptionService>>();
            var instance = new CryptionService(config.Object, logger.Object);
            var enc = System.Text.Encoding.UTF8;
            var secretValue = Convert.ToBase64String(enc.GetBytes(secret));
            // action
            var protectSecret = instance.Protect(secretValue);
            var actual = instance.Unprotect(protectSecret);
            // assert
            Assert.AreEqual(secretValue, actual);
        }

        [TestMethod]
        public void ProtectMethodParallelLoad()
        {
            // preparation
            var config = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            config.Setup(_ => _.GetSection("CRYPTION_KEY").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            config.Setup(_ => _.GetSection("CRYPTION_HASH").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_KEY2").Value).Returns("2GH3X8zK8xeJBbwx18yxCB9T7t2xzqqVH9LzJ1cmchI=");
            config.Setup(_ => _.GetSection("CRYPTION_IV2").Value).Returns("o4Pm6LJ+/q3UxwZArVLdkQ==");
            var logger = new Mock<ILogger<CryptionService>>();
            var instance = new CryptionService(config.Object, logger.Object);
            var enc = System.Text.Encoding.UTF8;
            var secretValue = Convert.ToBase64String(enc.GetBytes("Secret"));
            var option = new ParallelOptions() { MaxDegreeOfParallelism = 512 };
            // action
            Parallel.For(0, 13000, option, i =>
            {
                // action
                var protectSecret = instance.Protect(secretValue);
                var actual = instance.Unprotect(protectSecret);
                // assert
                Assert.AreEqual(secretValue, actual);
            });
        }
    }
}
