using Covid19Radar.Api;
using Covid19Radar.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Covid19Radar.Tests.Common
{
    [TestClass]
    public class UuidLockTest
    {
        static Type t = typeof(UuidLock);
        static BindingFlags staticflags = BindingFlags.NonPublic | BindingFlags.Static;
        public Dictionary<string, UuidLock.LockItem> Dictionary
        {
            get
            {
                var field = t.GetField("Locks", staticflags);
                return (Dictionary<string, UuidLock.LockItem>)field.GetValue(null);
            }
        }

        [DataTestMethod]
        [DataRow("UUID")]
        [DataRow("")]
        public void CreateMethod(string uuid)
        {
            // preparation
            var dic = Dictionary;
            dic.Clear();
            // action
            using (var l = new UuidLock(uuid)) { }
        }

        [TestMethod]
        public void CreateMethodThrowException()
        {
            // preparation
            var dic = Dictionary;
            dic.Clear();
            // action
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                using (var l = new UuidLock(null)) { }
            });
        }

        [TestMethod]
        public void CreateSingle()
        {
            // preparation
            var dic = Dictionary;
            dic.Clear();
            Assert.AreEqual(0, dic.Count);

            // action
            for (var i = 0; i < 256; i++)
            {
                // create
                using (var l = new UuidLock("UUID"))
                {
                    // Assert
                    Assert.AreEqual(1, dic.Count);
                }
                // Assert
                Assert.AreEqual(0, dic.Count);
            }
            // Assert
            Assert.AreEqual(0, dic.Count);
        }

        [TestMethod]
        public void CreateMultiple()
        {
            // preparation
            var dic = Dictionary;
            dic.Clear();
            Assert.AreEqual(0, dic.Count);

            // action
            for (var i = 0; i < 256; i++)
            {
                // create
                using (var l = new UuidLock($"UUID_{i}"))
                {
                    // Assert
                    Assert.AreEqual(1, dic.Count);
                }
                // Assert
                Assert.AreEqual(0, dic.Count);
            }
            // Assert
            Assert.AreEqual(0, dic.Count);
        }

        [TestMethod]
        public void CreateMultipleSame()
        {
            // preparation
            var dic = Dictionary;
            dic.Clear();
            Assert.AreEqual(0, dic.Count);

            var lockItems = new List<UuidLock>();
            // action
            for (var i = 1; i < 256; i++)
            {
                // create
                var l = new UuidLock($"UUID_{i}");
                lockItems.Add(l);
                // Assert
                Assert.AreEqual(i, dic.Count);
            }
            // Assert
            Assert.AreEqual(255, dic.Count);

            foreach(var lockItem in lockItems.ToArray())
            {
                // dispose
                lockItem.Dispose();
                lockItems.Remove(lockItem);
                // Assert
                Assert.AreEqual(lockItems.Count, dic.Count);
            }

            // Assert
            Assert.AreEqual(0, dic.Count);
        }

    }
}
