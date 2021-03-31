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
        public void GetValueTest_Default()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testDefault = 100;
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(false);
            ;
            var result = unitUnderTest.GetValue(testKey, testDefault);

            Assert.Equal(testDefault, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Never());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetValueTest_Int32()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x04, 0x03, 0x02, 0x01 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetValue<int>(testKey);

            var expectedResult = 16909060;
            Assert.Equal(expectedResult, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetValueTest_Int64()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetValue<long>(testKey);

            var expectedResult = 72623859790382856L;
            Assert.Equal(expectedResult, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetValueTest_Boolean()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x01 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetValue<bool>(testKey);

            Assert.True(result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetValueTest_Float()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x00, 0x00, 0x28, 0x41 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetValue<float>(testKey);

            var expectedResult = 10.5f;
            Assert.Equal(expectedResult, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetValueTest_Double()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x9A, 0x99, 0x99, 0x99, 0x99, 0x19, 0x25, 0x40 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetValue<double>(testKey);

            var expectedResult = 10.55d;
            Assert.Equal(expectedResult, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetValueTest_String()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0xE3, 0x83, 0x86, 0xE3, 0x82, 0xB9, 0xE3, 0x83, 0x88 };
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);

            var result = unitUnderTest.GetValue<string>(testKey);

            var expectedResult = "テスト";
            Assert.Equal(expectedResult, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void GetValueTest_UnknownType()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            byte[] testBytes = null;
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Returns(testBytes);
            ;
            var result = unitUnderTest.GetValue<ulong>(testKey);

            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void GetValueTest_Exception()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.GetBytes(testKey)).Throws(new InvalidOperationException("test"));

            var result = unitUnderTest.GetValue<int>(testKey);

            Assert.Equal(default, result);
            mockSecureStorageDependencyService.Verify(x => x.GetBytes(testKey), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void SetValueTests_Int32()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x04, 0x03, 0x02, 0x01 };
            var testValue = 16909060;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetValueTests_Int64()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };
            var testValue = 72623859790382856L;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetValueTests_Boolean()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x01 };
            var testValue = true;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetValueTests_Float()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x00, 0x00, 0x28, 0x41 };
            var testValue = 10.5f;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetValueTests_Double()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0x9A, 0x99, 0x99, 0x99, 0x99, 0x19, 0x25, 0x40 };
            var testValue = 10.55d;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetValueTests_String()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testBytes = new byte[] { 0xE3, 0x83, 0x86, 0xE3, 0x82, 0xB9, 0xE3, 0x83, 0x88 };
            var testValue = "テスト";

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, testBytes));

            unitUnderTest.SetValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, testBytes), Times.Once());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never());
        }

        [Fact]
        public void SetValueTests_UnknownType()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testValue = 100ul;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);

            unitUnderTest.SetValue(testKey, testValue);

            mockSecureStorageDependencyService.Verify(x => x.SetBytes(testKey, It.IsAny<byte[]>()), Times.Never());

            mockLoggerService.Verify(x => x.StartMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.EndMethod(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
            mockLoggerService.Verify(x => x.Exception(It.IsAny<string>(), It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void SetValueTests_Exception()
        {
            var unitUnderTest = CreateService();

            var testKey = "key";
            var testValue = 1;

            mockSecureStorageDependencyService.Setup(x => x.ContainsKey(testKey)).Returns(true);
            mockSecureStorageDependencyService.Setup(x => x.SetBytes(testKey, It.IsAny<byte[]>())).Throws(new InvalidOperationException("test"));

            unitUnderTest.SetValue(testKey, testValue);

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
