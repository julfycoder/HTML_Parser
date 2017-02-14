using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using HTML_Parser.DAL.Data.Entities;

namespace HTML_Parser.Business.Parsing
{
    public interface IParsingStorage
    {
        void Initialize(IContainer container);
        void SaveCssFiles(object state);
        void SaveImageFiles(object state);
        void SaveWebPages(object state);
        WebPage GetWebPage(int Id);
        IEnumerable<WebPage> GetWebPages();
    }
}
