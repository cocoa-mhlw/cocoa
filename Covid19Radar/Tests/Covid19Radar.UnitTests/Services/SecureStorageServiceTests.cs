/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Covid19Radar.Services;
using Covid19Radar.Services.Logs;
using Moq;
using Xunit;

namespace Covid19Radar.UnitTests.Services
{
    public class SecureStorageServiceTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<ILoggerService> mockLoggerService;
        private readonly Mock<ISecureStorageDependencyService> mockSecureStorageDependencyService;

        public SecureStorageServiceTests()
        {
            mockRepository = new MockRepository(MockBehavior.Default);
            mockLoggerService = mockRepository.Create<ILoggerService>();
            mockSecureStorageDependencyService = mockRepository.Create<ISecureStorageDependencyService>();
        }

        public SecureStorageService CreateService()
        {
            return new SecureStorageService(
                mockLoggerService.Object,
                mockSecureStorageDependencyService.Object);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ConteinsKeyTest_Success(bool testResult)
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(testResult);

            var result = unitUnderTest.ContainsKey(testKey);

            Assert.Equal(testResult, result);
            mockSecureStorageDependencyService.Verify(x => x.ContainsKey(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void ConteinsKeyTest_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Throws(new InvalidOperationException("test"));

            var result = unitUnderTest.ContainsKey(testKey);

            Assert.False(result);
            mockSecureStorageDependencyService.Verify(x => x.ContainsKey(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void GetIntValueTest_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x04, 0x03, 0x02, 0x01 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetIntValue(testKey);

            var expectedResult = 16909060;
            Assert.Equal(expectedResult, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetIntValueTest_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Throws(new InvalidOperationException("test"));

            var result = unitUnderTest.GetIntValue(testKey);

            Assert.Equal(default, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void GetLongValueTest_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetLongValue(testKey);

            var expectedResult = 72623859790382856L;
            Assert.Equal(expectedResult, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetLongValueTest_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Throws(new InvalidOperationException("test"));

            var result = unitUnderTest.GetLongValue(testKey);

            Assert.Equal(default, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void GetBoolValueTest_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x01 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetBoolValue(testKey);

            Assert.True(result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetBoolValueTest_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Throws(new InvalidOperationException("test"));

            var result = unitUnderTest.GetBoolValue(testKey);

            Assert.Equal(default, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void GetFloatValueTest_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x00, 0x00, 0x28, 0x41 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetFloatValue(testKey);

            var expectedResult = 10.5f;
            Assert.Equal(expectedResult, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetFloatValueTest_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Throws(new InvalidOperationException("test"));

            var result = unitUnderTest.GetFloatValue(testKey);

            Assert.Equal(default, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void GetDoubleValueTest_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x9A, 0x99, 0x99, 0x99, 0x99, 0x19, 0x25, 0x40 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetDoubleValue(testKey);

            var expectedResult = 10.55d;
            Assert.Equal(expectedResult, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetDoubleValueTest_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Throws(new InvalidOperationException("test"));

            var result = unitUnderTest.GetDoubleValue(testKey);

            Assert.Equal(default, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void GetStringValueTest_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0xE3, 0x83, 0x86, 0xE3, 0x82, 0xB9, 0xE3, 0x83, 0x88 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetStringValue(testKey);

            var expectedResult = "テスト";
            Assert.Equal(expectedResult, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetStringValueTest_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Throws(new InvalidOperationException("test"));

            var result = unitUnderTest.GetStringValue(testKey);

            Assert.Equal(default, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void SetIntValueTests_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x04, 0x03, 0x02, 0x01 };
            var testValue = 16909060;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetIntValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetIntValueTests_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testValue = 1;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, It.IsAny<byte[]>())).Throws(new InvalidOperationException("test"));

            unitUnderTest.SetIntValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, It.IsAny<byte[]>()), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void SetLongValueTests_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };
            var testValue = 72623859790382856L;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetLongValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetLongValueTests_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testValue = 72623859790382856L;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, It.IsAny<byte[]>())).Throws(new InvalidOperationException("test"));

            unitUnderTest.SetLongValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, It.IsAny<byte[]>()), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void SetBoolValueTests_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x01 };
            var testValue = true;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetBoolValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetBoolValueTests_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testValue = true;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, It.IsAny<byte[]>())).Throws(new InvalidOperationException("test"));

            unitUnderTest.SetBoolValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, It.IsAny<byte[]>()), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void SetFloatValueTests_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x00, 0x00, 0x28, 0x41 };
            var testValue = 10.5f;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetFloatValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetFloatValueTests_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testValue = 10.5f;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, It.IsAny<byte[]>())).Throws(new InvalidOperationException("test"));

            unitUnderTest.SetFloatValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, It.IsAny<byte[]>()), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void SetDoubleValueTests_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x9A, 0x99, 0x99, 0x99, 0x99, 0x19, 0x25, 0x40 };
            var testValue = 10.55d;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetDoubleValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetDoubleValueTests_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testValue = 10.55d;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, It.IsAny<byte[]>())).Throws(new InvalidOperationException("test"));

            unitUnderTest.SetDoubleValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, It.IsAny<byte[]>()), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void SetStringValueTests_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0xE3, 0x83, 0x86, 0xE3, 0x82, 0xB9, 0xE3, 0x83, 0x88 };
            var testValue = "テスト";

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetStringValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetStringValueTests_Failure()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testValue = "テスト";

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, It.IsAny<byte[]>())).Throws(new InvalidOperationException("test"));

            unitUnderTest.SetStringValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, It.IsAny<byte[]>()), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void RemoveValueTests_Success()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";

            unitUnderTest.RemoveValue(testKey);

            mockSecureStorageDependencyService.Verify(x => x.Remove(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void RemoveValueTests_Exception()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";

            mockSecureStorageDependencyService.Setup(x => x.Remove(testKey)).Throws(new InvalidOperationException("test"));

            unitUnderTest.RemoveValue(testKey);

            mockSecureStorageDependencyService.Verify(x => x.Remove(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }
    }
}
