using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HTML_Parser.DAL.Data.Entities
{
    public class WebFile:Entity
    {
        public string Name { get; set; }
        [ForeignKey("Page")]
        public int WebPageId { get; set; }
        public WebPage Page { get; set; }
    }
}
