using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace HTML_Parser.Business.Web
{
    public interface IHTMLDocumentManager
    {
        HtmlDocument GetHTMLDocument(string url);
        List<string> GetImagesUrls(List<HtmlNode> nodes);
        List<string> GetCssFilesUrls(List<HtmlNode> nodes);
        List<string> GetLinks(List<HtmlNode> nodes);
        List<HtmlNode> GetAllNodes(HtmlDocument doc);
        int GetHtmlSize(HtmlDocument doc);
    }
}
