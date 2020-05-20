using Covid19Radar.Api;
using Covid19Radar.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Covid19Radar.Api.Tests.Common
{
    [TestClass]
    [TestCategory("Common")]
    public class QueryCacheTest
    {
        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(int.MaxValue)]
        [DataRow(int.MinValue)]
        public void CreateMethod(int cacheTimeout)
        {
            // preparation
            // action
            var instance = new QueryCache<string>(cacheTimeout);
        }

        [DataTestMethod]
        [DataRow(60, "", "", 1)]
        [DataRow(60, "1", "1", 1)]
        [DataRow(60, null, null, 1)]
        [DataRow(0, "", "", 100)]
        [DataRow(0, "1", "1", 100)]
        [DataRow(0, null, null, 100)]
        [DataRow(-1, "", "", 100)]
        [DataRow(-1, "1", "1", 100)]
        [DataRow(-1, null, null, 100)]
        public async Task QueryWithCacheAsyncMethod(int cacheTimeout, string value, string expected, int expectedCallCount)
        {
            // preparation
            var instance = new QueryCache<string>(cacheTimeout);
            // action
            int callCount = 0;
            for (var i = 1; i <= 100; i++)
            {
                var actual = await instance.QueryWithCacheAsync(() =>
                {
                    callCount++;
                    return Task.FromResult(value);
                });
                Assert.AreEqual(expected, actual);
            }
            Assert.AreEqual(expectedCallCount, callCount);
        }

        [DataTestMethod]
        [DataRow(60, "", "", 1)]
        [DataRow(60, "1", "1", 1)]
        [DataRow(60, null, null, 1)]
        [DataRow(0, "", "", 500)]
        [DataRow(0, "1", "1", 500)]
        [DataRow(0, null, null, 500)]
        [DataRow(-1, "", "", 500)]
        [DataRow(-1, "1", "1", 500)]
        [DataRow(-1, null, null, 500)]
        public void QueryWithCacheAsyncMultipleMethod(int cacheTimeout, string value, string expected, int expectedCallCount)
        {
            // preparation
            var instance = new QueryCache<string>(cacheTimeout);
            var option = new ParallelOptions();
            option.MaxDegreeOfParallelism = 50;
            // action
            int callCount = 0;
            Parallel.For(0, 500, option, async _ =>
            {
                var actual = await instance.QueryWithCacheAsync(() =>
                {
                    callCount++;
                    return Task.FromResult(value);
                });
                Assert.AreEqual(expected, actual);
            });

            Assert.AreEqual(expectedCallCount, callCount);
        }
    }
}
