using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data;
using HTML_Parser.DAL.Data.Entities;
using HTML_Parser.Business.Parsing;
using HTML_Parser.DAL;

namespace HTML_Parser.Business.SiteTree
{
    public class SiteTreeBuilder:ISiteTreeBuilder
    {
        readonly ISiteTreeRepository _siteTreeRepository;
        readonly IParsingStorage _parsingStorage;
        private readonly ISiteTreeStringBuilder _siteTreeStringBuilder;

        public SiteTreeBuilder(ISiteTreeRepository siteTreeRepository, IParsingStorage parsingStorage, ISiteTreeStringBuilder siteTreeStringBuilder)
        {
            _siteTreeRepository = siteTreeRepository;
            _parsingStorage = parsingStorage;
            _siteTreeStringBuilder = siteTreeStringBuilder;
        }
        public void CreateSiteTree(string url)
        {
            _parsingStorage.Initialize();
            var webPages = _parsingStorage.GetWebPages().ToList();
            if (webPages.Any(p => p.URL == url))
            {
                var page = webPages.First(p => p.URL == url);
                var siteTree = _siteTreeStringBuilder.CreateSiteTree(page, webPages);
                _siteTreeRepository.SaveSiteTree(url, siteTree);
            }
        }
    }
}
