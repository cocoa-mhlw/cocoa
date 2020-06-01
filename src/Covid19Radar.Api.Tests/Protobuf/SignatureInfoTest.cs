using Covid19Radar.Api.Protobuf;
using Google.Protobuf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Covid19Radar.Api.Tests.Protobuf
{
    [TestClass]
    [TestCategory("Protobuf")]
    public class SignatureInfoTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var instance = new SignatureInfo();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new SignatureInfo();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

        [TestMethod]
        public void CalculateSizeMethod()
        {
            // preparation
            var instance = new SignatureInfo();
            // action
            var actual = instance.CalculateSize();
            // assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void EqualsMethod()
        {
            // preparation
            var instance = new SignatureInfo();
            // action
            var actual1 = instance.Clone();
            var actual2 = new SignatureInfo(instance);
            // assert
            Assert.AreEqual(instance, actual1);
            Assert.AreEqual(instance, actual2);
            Assert.AreEqual(instance.GetHashCode(), actual1.GetHashCode());
            Assert.AreEqual(instance.GetHashCode(), actual2.GetHashCode());
        }

        [TestMethod]
        public void EqualsMethodWithSetValue()
        {
            // preparation
            var instance = new SignatureInfo();
            instance.AndroidPackage = "AndroidPackage";
            instance.AppBundleId = "AppBundleId";
            instance.SignatureAlgorithm = "SignatureAlgorithm";
            instance.VerificationKeyId = "VerificationKeyId";
            instance.VerificationKeyVersion = "VerificationKeyVersion";
            // action
            var actual1 = instance.Clone();
            var actual2 = new SignatureInfo(instance);
            var actual3 = new SignatureInfo();
            var actual4 = new SignatureInfo();
            using var memory = new MemoryStream();
            using var codedOut = new CodedOutputStream(memory, true);
            instance.WriteTo(codedOut);
            codedOut.Flush();
            memory.Position = 0;
            using var codedIn = new CodedInputStream(memory, true);
            actual3.MergeFrom(codedIn);
            actual4.MergeFrom(actual3);
            // assert
            Assert.AreEqual(instance, actual1);
            Assert.AreEqual(instance, actual2);
            Assert.AreEqual(instance, actual3);
            Assert.AreEqual(instance, actual4);
            Assert.AreEqual(instance.GetHashCode(), actual1.GetHashCode());
            Assert.AreEqual(instance.GetHashCode(), actual2.GetHashCode());
            Assert.AreEqual(instance.GetHashCode(), actual3.GetHashCode());
            Assert.AreEqual(instance.GetHashCode(), actual4.GetHashCode());
            Assert.AreEqual(instance.CalculateSize(), actual1.CalculateSize());
            Assert.AreEqual(instance.CalculateSize(), actual2.CalculateSize());
            Assert.AreEqual(instance.CalculateSize(), actual3.CalculateSize());
            Assert.AreEqual(instance.CalculateSize(), actual4.CalculateSize());
            Assert.AreEqual(instance.ToString(), actual1.ToString());
            Assert.AreEqual(instance.ToString(), actual2.ToString());
            Assert.AreEqual(instance.ToString(), actual3.ToString());
            Assert.AreEqual(instance.ToString(), actual4.ToString());
        }


        [TestMethod]
        public void EqualsMethodWithClear()
        {
            // preparation
            var instance = new SignatureInfo();
            instance.AndroidPackage = "AndroidPackage";
            instance.AppBundleId = "AppBundleId";
            instance.SignatureAlgorithm = "SignatureAlgorithm";
            instance.VerificationKeyId = "VerificationKeyId";
            instance.VerificationKeyVersion = "VerificationKeyVersion";
            var actual = instance.Clone();
            // action assert
            actual.ClearAndroidPackage();
            Assert.AreNotEqual(instance, actual);
            actual = instance.Clone();
            actual.ClearAppBundleId();
            Assert.AreNotEqual(instance, actual);
            actual = instance.Clone();
            actual.ClearSignatureAlgorithm();
            Assert.AreNotEqual(instance, actual);
            actual = instance.Clone();
            actual.ClearVerificationKeyId();
            Assert.AreNotEqual(instance, actual);
            actual = instance.Clone();
            actual.ClearVerificationKeyVersion();
            Assert.AreNotEqual(instance, actual);
        }

        [TestMethod]
        public void EqualsMethodWithNull()
        {
            // preparation
            var instance = new SignatureInfo();
            // action assert
            Assert.IsFalse(instance.Equals(null));
        }

        [TestMethod]
        public void EqualsMethodWithSameReference()
        {
            // preparation
            var instance = new SignatureInfo();
            // action assert
            Assert.IsTrue(instance.Equals(instance));
        }
    }
}
