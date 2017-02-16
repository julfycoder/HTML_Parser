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
        void SaveCssFiles(object state);
        void SaveImageFiles(object state);
        void SaveWebPages(object state);
        WebPage GetWebPage(int id);
        WebPage GetWebPage(string url);
        IEnumerable<WebPage> GetWebPages();
    }
}
