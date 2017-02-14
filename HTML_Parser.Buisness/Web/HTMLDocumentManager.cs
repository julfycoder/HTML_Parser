using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace HTML_Parser.Business.Web
{
    public class HTMLDocumentManager:IHTMLDocumentManager
    {
        public HtmlDocument GetHTMLDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            if (url != null && url != "")
            {
                try
                {
                    return web.Load(url);
                }
                catch (Exception e)
                {
                    Console.WriteLine();
                    Console.WriteLine("On {0}, exception: {1}", url, e.Message);
                    Console.WriteLine();
                }
            }
            return null;
        }
        public List<string> GetImagesUrls(List<HtmlNode> nodes)
        {
            List<string> images = new List<string>();
            foreach (HtmlNode node in nodes)
            {
                if (node.Name == "img" && node.Attributes["src"] != null) images.Add(node.Attributes["src"].Value);
                else if (node.Name == "img" && node.Attributes["srcset"] != null) images.Add(node.Attributes["srcset"].Value);
            }
            return images;
        }
        public List<string> GetCssFilesUrls(List<HtmlNode> nodes)
        {
            List<string> cssFiles = new List<string>();
            foreach (HtmlNode node in nodes)
            {
                if (node.Name == "link" && node.Attributes["type"] != null &&
                    node.Attributes["type"].Value == "text/css" && node.Attributes["href"] != null)
                    cssFiles.Add(node.Attributes["href"].Value);
            }
            return cssFiles;
        }
        public List<string> GetLinks(List<HtmlNode> nodes)
        {
            List<string> links = new List<string>();
            foreach (HtmlNode node in nodes)
            {
                if (node.Name == "a" && node.Attributes["href"] != null && node.Attributes["href"].Value != "" && node.Attributes["href"].Value[0] != '#')
                {
                    if (node.Attributes["href"].Value.Count() >= ("javascript").Count())
                    {
                        if (node.Attributes["href"].Value.Substring(0, ("javascript").Count()) != "javascript")
                        {
                            links.Add(node.Attributes["href"].Value);
                        }
                    }
                    else links.Add(node.Attributes["href"].Value);
                }
            }
            return links;
        }
        
        public List<HtmlNode> GetAllNodes(HtmlDocument doc)
        {
            if (doc != null)
            {
                List<HtmlNode> nodes = doc.DocumentNode.ChildNodes.ToList();
                List<HtmlNode> children = new List<HtmlNode>();

                foreach (HtmlNode node in nodes)
                {
                    children.AddRange(node.ChildNodes);
                }
                while (children.Count != 0)
                {
                    foreach (HtmlNode node in children) nodes.AddRange(node.ChildNodes);
                    List<HtmlNode> temp = new List<HtmlNode>();
                    temp.AddRange(children);
                    children.Clear();

                    foreach (HtmlNode node in temp)
                    {
                        if (node.ChildNodes.Count != 0) children.AddRange(node.ChildNodes);
                    }
                }
                return nodes;
            }
            return new List<HtmlNode>();
        }
        public int GetHtmlSize(HtmlDocument doc)
        {
            if (doc != null && doc.DocumentNode != null && doc.DocumentNode.HasChildNodes)
            {
                string innerHTML = doc.DocumentNode.InnerHtml;
                return Encoding.GetEncoding(1251).GetByteCount(innerHTML);
            }
            return 0;
        }
    }
}
