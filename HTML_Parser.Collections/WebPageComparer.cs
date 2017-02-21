using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data.Entities;

namespace HTML_Parser.Collections
{
    public class WebPageComparer : IEqualityComparer<WebPage>
    {
        public bool Equals(WebPage x, WebPage y)
        {
            return x != null && y != null && x.URL == y.URL && x.ParentPageId == y.ParentPageId;
        }

        public int GetHashCode(WebPage obj)
        {
            return obj.GetHashCode();
        }
    }
}
