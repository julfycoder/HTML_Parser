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
            List<string> siteTree = new List<string>();
            siteTree.Add(root.URL + "\r\n");

            List<WebPage> children = new List<WebPage>();
            foreach (WebPage page in webPages)
            {
                if (page.ParentPageId == root.Id) children.Add(page);
            }

            if (children.Count > 0)
            {
                foreach (WebPage page in children)
                {
                    List<string> tree = CreateSiteTree(page, webPages);
                    foreach (string t in tree)
                    {
                        siteTree.Add('-' + t);
                    }
                }
            }
            return siteTree;
        }
    }
}
