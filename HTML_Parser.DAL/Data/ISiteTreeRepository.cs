using HTML_Parser.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;

namespace HTML_Parser.DAL.Data
{
    public interface ISiteTreeRepository
    {
        void Initialize(IContainer container);
        void SaveSiteTree(string url, List<WebPage> webPages);
    }
}
