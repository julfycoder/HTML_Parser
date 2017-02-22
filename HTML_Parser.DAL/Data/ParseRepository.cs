using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data.Entities;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace HTML_Parser.DAL.Data
{
    public class ParseRepository : IParseRepository
    {
        HTML_ParserDbContext context = new HTML_ParserDbContext();
        public void AddEntity<T>(T entity) where T : Entity
        {
            try
            {
                context.Set<T>().Add(entity);
                context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (DbEntityValidationResult result in e.EntityValidationErrors)
                {
                    Console.WriteLine(new string('-', 15));
                    foreach (DbValidationError error in result.ValidationErrors) Console.WriteLine(error.ErrorMessage);
                }
                Console.WriteLine(new string('-', 15));
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.InnerException.Message);
            }
            catch (Exception e) { Console.WriteLine(e.InnerException.Message); }
        }
        public void AddEntities<T>(IEnumerable<T> entities) where T : Entity
        {
            context.Set<T>().AddRange(entities);
            context.SaveChanges();
        }

        public IEnumerable<CssFile> GetCssFiles()
        {
            return context.CssFiles;
        }

        public IEnumerable<WebPage> GetWebPages()
        {
            return context.WebPages;
        }
        public WebPage GetWebPage(int id)
        {
            return context.WebPages.First(p => p.Id == id);
        }
        public WebPage GetWebPage(string url)
        {
            try
            {
                return context.WebPages.First(p => p.URL == url);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<ImageFile> GetImageFiles()
        {
            return context.ImageFiles;
        }

        public void RemoveEntity<T>(T entity) where T : Entity
        {
            context.Set<T>().Remove(entity);
            context.SaveChanges();
        }

        public IEnumerable<WebSite> GetWebSites()
        {
            return context.WebSites;
        }

        public WebSite GetWebSite(string url)
        {
            if (context.WebSites.Any(s => s.URL == url)) return context.WebSites.First(s => s.URL == url);
            return null;
        }

        public WebSite GetWebSite(int id)
        {
            if (context.WebSites.Any(s => s.Id == id)) return context.WebSites.First(s => s.Id == id);
            return null;
        }
    }
}
