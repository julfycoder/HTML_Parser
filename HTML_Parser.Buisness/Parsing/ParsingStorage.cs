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
        IParseRepository repository;

        List<WebPage> dbPages = new List<WebPage>();
        List<CssFile> dbCss = new List<CssFile>();
        List<ImageFile> dbImages = new List<ImageFile>();

        public ParsingStorage(IParseRepository parseRepository)
        {
            this.repository = parseRepository;

        }
        public void Initialize()
        {
            dbPages = repository.GetWebPages().ToList();
            dbImages = repository.GetImageFiles().ToList();
            dbCss = repository.GetCssFiles().ToList();

        }
        public void SaveWebPages(object state)
        {
            WebPage page = (WebPage)state;
            if (dbPages != null && (!dbPages.Any() || dbPages.All(p => p.URL != page.URL)))
            {
                lock (repository)
                {
                    repository.AddEntity(page);
                    dbPages.Add(repository.GetWebPages().Last());
                }
            }
        }
        public void SaveCssFiles(object state)
        {
            CssFile css = (CssFile)state;
            if (dbCss != null && dbCss.All(c => c.URL != css.URL && c.WebPageId != css.WebPageId))
            {
                lock (repository) repository.AddEntity(css);
            }
        }
        public void SaveImageFiles(object state)
        {
            ImageFile image = (ImageFile)state;
            if (dbImages != null && dbImages.All(i => i.URL != image.URL && i.WebPageId != image.WebPageId))
            {
                lock (repository) repository.AddEntity(image);
            }
        }
        public WebPage GetWebPage(int id)
        {
            return dbPages.First(p => p.Id == id);
        }
        public WebPage GetWebPage(string url)
        {
            return repository.GetWebPage(url);
        }
        public IEnumerable<WebPage> GetWebPages()
        {
            return dbPages;
        }
    }
}
