using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Entities
{
    public class WebFile:Entity
    {
        public string URL { get; set; }
        public string Name { get; set; }
        public HTMLTree Page { get; set; }
    }
}
