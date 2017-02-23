using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HTML_Parser.Business.Web;

namespace HTML_Parser.Business.Web.Tests
{
    [TestClass]
    public class URLManagerTests
    {
        [TestMethod]
        public void GetHostWithScheme_SchemePlusHostOfUrlReturns()
        {
            string url = "https://github.com/julfycoder/HTML_Parser";
            string expected = "https://github.com";

            URLManager u = new URLManager();
            string actual = u.GetHostWithScheme(url);

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void IsForeignUrl_OnForeignUrlTrueReturns()
        {
            string url = "https://msdn.microsoft.com/uk-ua/library/ms182532.aspx";
            string parentUrl = "https://www.wikipedia.org/";

            Assert.IsTrue(IsForeignUrlCall(url, parentUrl));
        }
        [TestMethod]
        public void IsForeignUrl_OnInnerUrlFalseReturns()
        {
            string url = "https://en.wikipedia.org/wiki/Main_Page";
            string parentUrl = "https://www.wikipedia.org/";

            Assert.IsFalse(IsForeignUrlCall(url, parentUrl));
        }
        [TestMethod]
        public void IsAbsoluteUrl_OnAbsoluteUrlTrueReturns()
        {
            string url = "https://www.wikipedia.org/";

            Assert.IsTrue(IsAbsoluteUrlCall(url));
        }
        [TestMethod]
        public void IsAbsoluteUrl_OnLocalPathFalseReturns()
        {
            string url = "/wiki/Wikidata:Main_Page";

            Assert.IsFalse(IsAbsoluteUrlCall(url));
        }

        public bool IsAbsoluteUrlCall(string url)
        {
            URLManager u = new URLManager();

            return u.IsAbsoluteURL(url);
        }
        public bool IsForeignUrlCall(string url, string parentUrl)
        {
            URLManager u = new URLManager();

            return u.IsForeignURL(url, parentUrl);
        }
    }
}
