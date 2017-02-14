using HTML_Parser.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.DAL
{
    public interface ISiteTreeStringBuilder
    {
        List<string> CreateSiteTree(WebPage root, List<WebPage> webPages);
    }
}
