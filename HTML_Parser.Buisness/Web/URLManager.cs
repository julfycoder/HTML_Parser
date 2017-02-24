using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Web
{
    public class URLManager : IURLManager
    {
        public string GetCorrectURL(string currentUrl)                                  //Not tested
        {
            if (currentUrl.Length > 1 && currentUrl.Substring(0, 2) == "//") currentUrl = "https:" + currentUrl;
            return currentUrl;
        }
        public string GetCorrectURL(string currentUrl, string parentUrl)                //Not tested
        {
            if (currentUrl.Count() > 1 && currentUrl.Substring(0, 2) == "//")
            {
                return "https:" + currentUrl;                                                                 //Starts on '//'
            }
            else if (currentUrl.Count() > 5 && currentUrl.Substring(currentUrl.Count() - 5, 5).ToLower() == ".html" &&
               parentUrl.Substring(parentUrl.Count() - 5, 5).ToLower() == ".html")
            {
                return parentUrl.Substring(0, (parentUrl.Length - parentUrl.Split('/').Last().Length - 1)) +
                    "/" + currentUrl.Split('/')[currentUrl.Split('/').Count() - 1];            //Ends on '.html'
            }
            else if ((currentUrl.Length == 1 && currentUrl[0] == '/') ||                                //Single '/'
                    (currentUrl.Count() > 2 && (currentUrl.Substring(0, 2) == ".." || currentUrl.Substring(0, 1) == ".")) ||//Starts on '..' 
                    (currentUrl.Count() > 5 && currentUrl.Substring(currentUrl.Count() - 5, 5).ToLower() == ".html" && !IsAbsoluteURL(currentUrl)))//Ends on '.html'
            {
                return parentUrl + "/" + currentUrl.Split('/')[currentUrl.Split('/').Count() - 1];
            }
            else if (currentUrl.Count() > 1 && currentUrl[0] == '/' && currentUrl.Split('/')[0] == parentUrl.Split('/').Last())
            {
                if (parentUrl.Last() == '/' && currentUrl.First() == '/')
                {
                    return parentUrl.Substring(0, parentUrl.Length - parentUrl.Split('/').Last().Length - 1) + currentUrl;
                }
                return parentUrl.Substring(0, parentUrl.Length - parentUrl.Split('/').Last().Length) + currentUrl;
            }
            else if (currentUrl.Count() > 1 && currentUrl[0] == '/') //Starts on '/'
            {
                return parentUrl.Substring(0, parentUrl.Length - parentUrl.Split('/').Last().Length-1) + currentUrl;
            }
            else if ((currentUrl.Count() < 7) || ((currentUrl.Count() >= 7 && currentUrl.Substring(0, 7) != "http://")
                && (currentUrl.Count() >= 8 && currentUrl.Substring(0, 8) != "https://")))
            {
                return parentUrl + "/" + currentUrl;                                                          //Starts on 'http://' or 'https://'
            }
            return currentUrl;
        }
        public bool IsAbsoluteURL(string urlString)
        {
            Uri result;
            if (Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out result))
            {
                return result.IsAbsoluteUri;
            }
            return false;
        }
        public bool IsBelongTo(Uri uri, Uri childUri)                       //Not tested
        {
            if (childUri.Host == uri.Host) return true;
            if (childUri.Scheme != "")
            {
                string[] uriHostFragments = uri.Host.Split('.');
                string[] childHostFragments = childUri.Host.Split('.');

                int coincidence = 0;
                for (int i = 0; i < uriHostFragments.Length; i++)
                {
                    if (childHostFragments.Any(c => c == uriHostFragments[i])) coincidence++;
                    else if (coincidence > 0) coincidence--;
                    if (coincidence > 1) return true;
                }
            }
            return false;
        }
        public bool IsForeignURL(string url, string parentUrl)
        {
            url = GetCorrectURL(url);
            Uri parentUri = new Uri(parentUrl);
            Uri childUri;
            try
            {
                childUri = new Uri(GetCorrectURL(url, parentUrl));
            }
            catch (Exception) { childUri = new Uri(parentUrl); }
            return (((url.Count() >= 7 && url.Substring(0, 7) == "http://") || (url.Count() >= 8 && url.Substring(0, 8) == "https://")) &&
                (!IsBelongTo(parentUri, childUri)));
        }
        public string GetHostWithScheme(string url)
        {
            Uri uri = new Uri(url);
            return uri.Scheme + "://" + uri.Host;
        }
    }
}
