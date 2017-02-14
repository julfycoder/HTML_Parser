using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.Business;

namespace HTML_Parser.Business.Entities
{
    public class HTMLTree : Entity
    {
        public string URL { get; set; }
        public int TimeToLoad { get; set; }
        public int HTML_Size { get; set; }
        public HTMLTree Parent { get; set; }
        public SortedList<string, HTMLTree> Pages { get; set; }
        public SortedList<string, CssFile> CssFiles { get; set; }
        public SortedList<string, ImageFile> ImageFiles { get; set; }

        //public void Insert(HTMLTree parent, HTMLTree child)
        //{
        //    if (!Pages.Contains(parent))
        //    {

        //    }
        //}
    }
}
