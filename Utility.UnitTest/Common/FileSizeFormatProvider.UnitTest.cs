using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.Common;

namespace Utility.UnitTest.Common
{
    [TestClass]
    public class FileSizeFormatProviderUnitTest
    {
        [TestMethod]
        public void Test100()
        {
            string result = string.Format(new FileSizeFormatProvider(), "File size: {0:fs}", 100);
            Assert.AreEqual("File size: 100.00 B", result);
        }

        [TestMethod]
        public void Test100000()
        {
            string result = string.Format(new FileSizeFormatProvider(), "File size: {0:fs}", 100000);
            Assert.AreEqual("File size: 97.66kB", result);
        }

        [TestMethod]
        public void TestSizeConverter()
        {
            object value = 123456789;
            string result = string.Format(new FileSizeFormatProvider(), "{0:fs}", value);
            Assert.AreEqual("117.74MB", result);
        }

        [TestMethod]
        public void TestSize08T()
        {
            object value = 879609302220;
            string result = string.Format(new FileSizeFormatProvider(), "{0:fs}", value);
            Assert.AreEqual("819.20GB", result);
        }

        [TestMethod]
        public void TestSize15G()
        {
            object value = 16106127360;
            string result = string.Format(new FileSizeFormatProvider(), "{0:fs}", value);
            Assert.AreEqual("15.00GB", result);
        }
    }
}
