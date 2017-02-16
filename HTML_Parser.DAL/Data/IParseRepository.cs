using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data.Entities;

namespace HTML_Parser.DAL.Data
{
    public interface IParseRepository
    {
        IEnumerable<WebPage> GetWebPages();
        WebPage GetWebPage(int id);
        WebPage GetWebPage(string url);
        IEnumerable<CssFile> GetCssFiles();
        IEnumerable<ImageFile> GetImageFiles();

        void AddEntity<T>(T entity) where T : Entity;
        void RemoveEntity<T>(T entity) where T : Entity;
    }
}
