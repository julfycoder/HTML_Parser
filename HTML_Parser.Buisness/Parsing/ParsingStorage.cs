using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data.Entities;
using HTML_Parser.DAL.Data;
using StructureMap;

namespace HTML_Parser.Business.Parsing
{
    public class ParsingStorage : IParsingStorage
    {
        readonly IParseRepository _repository;

        private readonly Dictionary<string, WebPage> _dbPages = new Dictionary<string, WebPage>();
        private List<CssFile> _dbCss = new List<CssFile>();
        private List<ImageFile> _dbImages = new List<ImageFile>();
        private List<WebSite> _dbSites = new List<WebSite>();

        public ParsingStorage(IParseRepository parseRepository)
        {
            this._repository = parseRepository;

        }
        public void Initialize()
        {
            foreach(var page in _repository.GetEntities<WebPage>())_dbPages.Add(page.URL,page);
            _dbImages = _repository.GetEntities<ImageFile>().ToList();
            _dbCss = _repository.GetEntities<CssFile>().ToList();
            _dbSites = _repository.GetEntities<WebSite>().ToList();
        }
        public void SaveWebPage(object state)
        {
            var page = (WebPage)state;
            if (_dbPages != null && (!_dbPages.Any() || _dbPages.ContainsKey(page.URL)))
            {
                lock (_repository)
                {
                    _repository.AddEntity(page);
                    var dbPage = _repository.GetEntities<WebPage>().ToList().Last();
                    _dbPages.Add(dbPage.URL,dbPage);
                }
            }
        }
        public void SaveWebPages(IEnumerable<WebPage> pages)
        {
            lock (_repository) _repository.AddEntities(pages);
            foreach (var page in pages) _dbPages.Add(page.URL,_repository.GetWebPage(page.URL));
        }
        public void SaveCssFile(object state)
        {
            var css = (CssFile)state;
            if (_dbCss != null && _dbCss.All(c => c.URL != css.URL && c.WebPageId != css.WebPageId))
            {
                lock (_repository) _repository.AddEntity(css);
            }
        }
        public void SaveCssFiles(IEnumerable<CssFile> cssFiles)
        {
            foreach (var css in cssFiles)
            {
                if (_dbCss != null && _dbCss.All(c => c.URL != css.URL && c.WebPageId != css.WebPageId))
                {
                    lock (_repository) _repository.AddEntity(css);
                }
            }
        }
        public void SaveImageFile(object state)
        {
            var image = (ImageFile)state;
            if (_dbImages != null && _dbImages.All(i => i.URL != image.URL && i.WebPageId != image.WebPageId))
            {
                lock (_repository) _repository.AddEntity(image);
            }
        }
        public void SaveImageFiles(IEnumerable<ImageFile> imageFiles)
        {
            foreach (var image in imageFiles)
            {
                if (_dbImages != null && _dbImages.All(i => i.URL != image.URL && i.WebPageId != image.WebPageId))
                {
                    lock (_repository) _repository.AddEntity(image);
                }
            }
        }

        public WebPage GetWebPage(string url)
        {
            if (_dbPages.ContainsKey(url))
                return _dbPages[url];
            return null;
        }

        public WebSite GetWebSite(string url)
        {
            return _dbSites.First(s => s.URL == url);
        }

        public void SaveWebSite(object state)
        {
            var site = (WebSite)state;
            if (_dbSites != null && (!_dbSites.Any() || _dbSites.All(p => p.URL != site.URL)))
            {
                lock (_repository)
                {
                    _repository.AddEntity(site);
                    _dbSites.Add(_repository.GetEntities<WebSite>().ToList().Last());
                }
            }
        }
        public IEnumerable<WebPage> GetWebPages()
        {
            return _dbPages.Values;
        }

        public IEnumerable<WebSite> GetWebSites()
        {
            return _dbSites;
        }

        public IEnumerable<ImageFile> GetImages()
        {
            return _dbImages;
        }

        public IEnumerable<CssFile> GetCssFiles()
        {
            return _dbCss;
        }
    }
}
