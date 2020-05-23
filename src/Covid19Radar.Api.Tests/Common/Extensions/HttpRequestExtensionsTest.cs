using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Covid19Radar.Api.Extensions;
using Covid19Radar.Api.Models;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.Common.Extensions
{
    [TestClass]
    [TestCategory("Common")]
    public class HttpRequestExtensionsTest
    {
        public class TestModel : IPayload
        {
            public bool IsValid()
            {
                return !string.IsNullOrWhiteSpace(TestProperty);
            }

            public string TestProperty { get; set; }
        }

        [DataTestMethod]
        [DataRow("1")]
        [DataRow("XXX")]
        public async Task ParseAndThrowMethod(string propertyValue)
        {
            // preparation
            var request = new Mock<HttpRequest>();
            var model = new TestModel();
            model.TestProperty = propertyValue;
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            writer.Write(JsonConvert.SerializeObject(model));
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            request.SetupGet(_ => _.Body).Returns(stream);
            // action
            var actual = await request.Object.ParseAndThrowAsync<TestModel>();
            // assert
            Assert.AreEqual(model.TestProperty, actual.TestProperty);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task ParseAndThrowMethodException(string propertyValue)
        {
            // preparation
            var request = new Mock<HttpRequest>();
            var model = new TestModel();
            model.TestProperty = propertyValue;
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            writer.Write(JsonConvert.SerializeObject(model));
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            request.SetupGet(_ => _.Body).Returns(stream);
            // action
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await request.Object.ParseAndThrowAsync<TestModel>());
        }

    }
}
