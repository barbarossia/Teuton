using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility.Common;
using Utility.Web;

namespace Utility.UnitTest.Web
{
    [TestClass]
    public class BookmarkUnitTest
    {
        [TestMethod]
        public void Test()
        {
            var content = Bookmark.GetIEFavoritesUrl();
            //Assert.AreEqual(1048576, converter.Parse("1M"));
        }

        [TestMethod]
        public void TestBtTianTang()
        {
            var content = Bookmark.GetIEFavoritesUrl("BtTianTang");
            //Assert.AreEqual(1048576, converter.Parse("1M"));
        }
    }
}
