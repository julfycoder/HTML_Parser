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
            return context.WebPages.First(p => p.URL == url);
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
    }
}
