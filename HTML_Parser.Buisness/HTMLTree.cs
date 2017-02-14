using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business
{
    public class HTMLTree
    {
        public string URL { get; set; }
        public HTMLTree Parent { get; set; }
        public HashSet<HTMLTree> Children { get; set; }
        public void Insert(string URL)
        {
            
        }
    }
}
