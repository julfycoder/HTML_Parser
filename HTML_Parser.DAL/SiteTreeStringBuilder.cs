using HTML_Parser.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.DAL
{
    public class SiteTreeStringBuilder : ISiteTreeStringBuilder
    {
        public List<string> CreateSiteTree(WebPage root, List<WebPage> webPages)
        {
            var siteTree = new List<string> {root.URL + "\r\n"};

            var children = webPages.Where(page => page.ParentPageId == root.Id).ToList();

            if (children.Count <= 0) return siteTree;

            siteTree.AddRange(from page in children from t in CreateSiteTree(page, webPages) select '-' + t);

            return siteTree;
        }
    }
}
