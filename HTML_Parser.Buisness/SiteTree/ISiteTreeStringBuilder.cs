using System.Collections.Generic;
using HTML_Parser.DAL.Data.Entities;

namespace HTML_Parser.Business.SiteTree
{
    public interface ISiteTreeStringBuilder
    {
        List<string> CreateSiteTree(WebPage root, List<WebPage> webPages);
    }
}
