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
        WebPage GetWebPage(string url);
        WebSite GetWebSite(string url);
        WebSite GetWebSite(int id);
        void AddEntity<T>(T entity) where T : Entity;
        void AddEntities<T>(IEnumerable<T> entities) where T : Entity;
        void RemoveEntity<T>(T entity) where T : Entity;
        IEnumerable<T> GetEntities<T>() where T : Entity;
    }
}
