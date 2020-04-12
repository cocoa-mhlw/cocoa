using Covid19Radar.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Covid19Radar.Tests.Models
{
    [TestClass]
    public class SequenceDataModelTest
    {
        [TestMethod]
        public void CreateMethod()
        {
            // action
            var model = new SequenceDataModel();
        }

        [TestMethod]
        public void PropertiesTest()
        {
            // preparation
            var model = new SequenceDataModel();
            // model property access
            Helper.ModelTestHelper.PropetiesTest(model);
        }

        [DataTestMethod]
        [DataRow(0, 0, 0, 1)]
        [DataRow(0, 1, 0, 2)]
        [DataRow(0, 65535, 0, 65536)]
        [DataRow(0, 65536, 1, 0)]
        [DataRow(1, 0, 1, 1)]
        public void IncrementTest(int major, int minor,
                                  int expectedMajor, int expectedMinor)
        {
            // preparation
            var model = new SequenceDataModel();
            model.Major = major;
            model.Minor = minor;
            Assert.AreEqual(major, model.Major);
            Assert.AreEqual(minor, model.Minor);
            // action
            model.Increment();
            // assert
            Assert.AreEqual(expectedMajor, model.Major);
            Assert.AreEqual(expectedMinor, model.Minor);

        }

        [DataTestMethod]
        [DataRow(0, false)]
        [DataRow(65535, false)]
        [DataRow(65536, true)]
        [DataRow(65537, true)]
        public void IsMajorMaxTest(int major, bool expected)
        {
            // preparation
            var model = new SequenceDataModel();
            model.Major = major;
            Assert.AreEqual(major, model.Major);
            // action
            var result = model.IsMajorMax();
            // assert
            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow(0, false)]
        [DataRow(65535, false)]
        [DataRow(65536, true)]
        [DataRow(65537, true)]
        public void IsMinorMaxTest(int minor, bool expected)
        {
            // preparation
            var model = new SequenceDataModel();
            model.Minor = minor;
            Assert.AreEqual(minor, model.Minor);
            // action
            var result = model.IsMinorMax();
            // assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void IdTest()
        {
            // preparation
            var model = new SequenceDataModel();
            // action
            var result = model.id;
            // assert
            Assert.IsNotNull(result);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        }
    }
}
