using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Web
{
    public interface IURLManager
    {
        string GetCorrectURL(string currentUrl);
        string GetCorrectURL(string currentUrl, string ParentUrl);
        bool IsAbsoluteURL(string urlString);
        bool IsBelongTo(Uri uri, Uri childUri);
        bool IsForeignURL(string url, string parentUrl);
        bool IsDomain(string url);
        string GetHostWithScheme(string url);
    }
}
