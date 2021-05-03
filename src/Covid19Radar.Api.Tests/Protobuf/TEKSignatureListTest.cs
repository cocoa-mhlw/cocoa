﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Covid19Radar.Background.Protobuf;
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
    public class TEKSignatureListTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // preparation
            var instance = new TEKSignatureList();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new TEKSignatureList();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

        [TestMethod]
        public void CalculateSizeMethod()
        {
            // preparation
            var instance = new TEKSignatureList();
            // action
            var actual = instance.CalculateSize();
            // assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void EqualsMethod()
        {
            // preparation
            var instance = new TEKSignatureList();
            // action
            var actual1 = instance.Clone();
            var actual2 = new TEKSignatureList(instance);
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
            var instance = new TEKSignatureList();
            instance.Signatures.Add(new TEKSignature());
            // action
            var actual1 = instance.Clone();
            var actual2 = new TEKSignatureList(instance);
            var actual3 = new TEKSignatureList();
            var actual4 = new TEKSignatureList();
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
            var instance = new TEKSignatureList();
            instance.Signatures.Add(new TEKSignature());
            var actual = instance.Clone();
            // action assert
            actual.Signatures.Clear();
            Assert.AreNotEqual(instance, actual);
        }

        [TestMethod]
        public void EqualsMethodWithNull()
        {
            // preparation
            var instance = new TEKSignatureList();
            // action assert
            Assert.IsFalse(instance.Equals(null));
        }

        [TestMethod]
        public void EqualsMethodWithSameReference()
        {
            // preparation
            var instance = new TEKSignatureList();
            // action assert
            Assert.IsTrue(instance.Equals(instance));
        }
    }
}
