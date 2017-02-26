using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data;
using HTML_Parser.DAL.Data.Entities;
using HTML_Parser.Business.Parsing;

namespace HTML_Parser.Business.SiteTree
{
    public class SiteTreeBuilder:ISiteTreeBuilder
    {
        ISiteTreeRepository siteTreeRepository;
        IParsingStorage parsingStorage;

        public SiteTreeBuilder(ISiteTreeRepository siteTreeRepository, IParsingStorage parsingStorage)
        {
            this.siteTreeRepository = siteTreeRepository;
            this.parsingStorage = parsingStorage;
        }
        public void CreateSiteTree(string url)
        {
            parsingStorage.Initialize();
            List<WebPage> webPages = parsingStorage.GetWebPages().ToList();
            siteTreeRepository.SaveSiteTree(url, webPages);
        }
    }
}
