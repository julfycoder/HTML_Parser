using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using HTML_Parser.DAL.Data.Entities;

namespace HTML_Parser.DAL.Data.Entities
{
    public class HTML_ParserDbContext : DbContext
    {
        public HTML_ParserDbContext() : base("HTML_ParserConnectionString")
        {

        }
        public DbSet<WebPage> WebPages { get; set; }
        public DbSet<CssFile> CssFiles { get; set; }
        public DbSet<ImageFile> ImageFiles { get; set; }
    }
}
