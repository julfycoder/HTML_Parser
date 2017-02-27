using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Parsing
{
    public interface ISiteTreeBuilder
    {
        void CreateSiteTree(string url);
    }
}
