using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data.Entities;

namespace HTML_Parser.Business.Parsing
{
    public interface IParsingStorage
    {
        void Initialize();
        void SaveCssFile(object state);
        void SaveCssFiles(IEnumerable<CssFile> cssFiles);
        void SaveImageFile(object state);
        void SaveImageFiles(IEnumerable<ImageFile> imageFIles);
        void SaveWebPage(object state);
        void SaveWebPages(IEnumerable<WebPage> pages);
        WebPage GetWebPage(string url);
        IEnumerable<WebPage> GetWebPages();
        IEnumerable<WebSite> GetWebSites();
        WebSite GetWebSite(string url);
        void SaveWebSite(object state);
        IEnumerable<ImageFile> GetImages();
        IEnumerable<CssFile> GetCssFiles();
    }
}
