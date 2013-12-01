using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.Common;

namespace Utility.UnitTest.Common
{
    [TestClass]
    public class FileSizeConverterUnitTest
    {
        [TestMethod]
        public void Test1M()
        {
            var converter = new FileSizeConverter();
            Assert.AreEqual(1048576, converter.Parse("1M"));
        }

        [TestMethod]
        public void Test15G()
        {
            var converter = new FileSizeConverter();
            Assert.AreEqual(16106127360, converter.Parse("15G"));
        }

        [TestMethod]
        public void Test08T()
        {
            var converter = new FileSizeConverter();
            Assert.AreEqual(879609302220, converter.Parse("0.8T"));
        }
    }
}
