﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Diagnostics;
using HTML_Parser.DAL.Data.Entities;
using HTML_Parser.DAL.Data;
using HTML_Parser.Business.Web;
using HTML_Parser.Business.Threading;

namespace HTML_Parser.Business.Parsing
{

    public class Parser : IParser
    {
        IHTMLDocumentManager docManager;
        IURLManager urlManager;
        IThreadsManager threadsManager;
        IParsingStorage parsingStorage;

        HashSet<string> visitedPages = new HashSet<string>();
        Dictionary<string, string> links = new Dictionary<string, string>();
        Dictionary<string, WebPage> storagePages = new Dictionary<string, WebPage>();
        List<WebSite> sites = new List<WebSite>();

        List<WebPage> currentPages = new List<WebPage>();
        Dictionary<ImageFile, string> currentPagesImages = new Dictionary<ImageFile, string>();
        Dictionary<CssFile, string> currentPagesCssFiles = new Dictionary<CssFile, string>();

        bool useForeignLinks = true;

        Stopwatch watch = new Stopwatch();
        public Parser(IHTMLDocumentManager docManager, IURLManager urlManager, IParsingStorage parsingStorage, IThreadsManager threadsManager)//, ISiteTreeRepository siteTreeRepository)
        {
            this.docManager = docManager;
            this.urlManager = urlManager;
            this.parsingStorage = parsingStorage;
            this.threadsManager = threadsManager;
        }

        public void ChangeWorkerThreadsCount(int workerThreads = 10)
        {
            threadsManager.SetMaxThreads(workerThreads, 10);
        }

        public void Start(string url, int workerThreads = 10, int depth = -2, bool useForeignLinks = true)
        {
            parsingStorage.Initialize();
            foreach (WebPage page in parsingStorage.GetWebPages()) if (!storagePages.ContainsKey(page.URL)) storagePages.Add(page.URL, page);
            this.useForeignLinks = useForeignLinks;

            threadsManager.SetMaxThreads(workerThreads, 10);

            Initialize();
            links.Add(url, null);

            Parse(url, depth);
        }
        void Initialize()
        {
            links.Clear();
            visitedPages.Clear();
        }
        void ParsePage(object state)
        {
            WebPage parsingPage = (WebPage)state;

            visitedPages.Add(parsingPage.URL);
            HtmlDocument doc = GetHtmlDocument(parsingPage.URL);
            List<HtmlNode> nodes = docManager.GetAllNodes(doc);
            List<string> currentLinks = docManager.GetLinks(nodes);
            foreach (string link in currentLinks)
            {
                if (useForeignLinks || (!useForeignLinks && !urlManager.IsForeignURL(urlManager.GetCorrectURL(link,parsingPage.URL), parsingPage.URL)))
                {
                    lock (links)
                    {
                        if (!links.ContainsKey(link))
                        {
                            links.Add(link, parsingPage.URL);
                        }
                    }
                }
            }
            if (!storagePages.ContainsKey(parsingPage.URL))
            {
                SavePage(parsingPage, doc);
                SaveImages(parsingPage.URL, docManager.GetImagesUrls(nodes));
                SaveCssFiles(parsingPage.URL, docManager.GetCssFilesUrls(nodes));
            }

            Console.WriteLine(parsingPage.URL);
            Console.WriteLine(GetTimeToLoad());
            Console.WriteLine();

        }

        void Parse(string url, int depth)
        {
            int childsCount = -1;
            while (links.Count != childsCount || visitedPages.Count == 0)
            {
                childsCount = links.Count;
                depth--;

                List<string> keys = new List<string>(links.Keys);
                foreach (string link in keys)
                {
                    WebPage parent = GetParentPage(url, link);
                    if ((!visitedPages.Contains(urlManager.GetCorrectURL(link, parent.URL)) && (depth > -2) &&
                        ((urlManager.IsForeignURL(link, parent.URL) == false && useForeignLinks == false) || useForeignLinks == true)))
                    {
                        if ((urlManager.GetCorrectURL(link, parent.URL) != parent.URL) || (visitedPages.Count == 0))
                        {
                            Uri u;
                            if (Uri.TryCreate(urlManager.GetCorrectURL(link, parent.URL), UriKind.RelativeOrAbsolute, out u))
                            {
                                SaveWebSite(urlManager.GetCorrectURL(link), parent.URL);
                                threadsManager.QueueWorkItem(ParsePage, CreatePage(link, parent));
                            }
                        }
                    }
                    threadsManager.WaitAllThreads();
                    if (currentPages.Count >= 5)
                    {
                        SaveCurrentPages();
                        SaveCurrentPagesCssFiles();
                        SaveCurrentPagesImages();
                    }
                }
                threadsManager.WaitAllThreads();
                SaveCurrentPages();
                SaveCurrentPagesCssFiles();
                SaveCurrentPagesImages();
            }
        }
        void SaveWebSite(string url, string parentUrl)
        {
            if (!sites.Any(s => urlManager.IsBelongTo(new Uri(s.URL), new Uri(urlManager.GetCorrectURL(url, parentUrl)))))
            {
                WebSite site = new WebSite { URL = urlManager.GetHostWithScheme(urlManager.GetCorrectURL(url, parentUrl)) };
                lock (parsingStorage)
                {
                    parsingStorage.SaveWebSite(site);
                    site = parsingStorage.GetWebSite(urlManager.GetHostWithScheme(urlManager.GetCorrectURL(url, parentUrl)));
                }
                lock (sites) if (sites.All(s => s.Id != site.Id)) sites.Add(site);
            }
        }
        WebPage CreatePage(string url, WebPage parentPage)//////////////////////////////??????????????????????
        {
            int? parentPageId = null;
            int? referralPageId = null;
            if (url != parentPage.URL)
            {
                referralPageId = parentPage.Id;
                if (!urlManager.IsForeignURL(url, parentPage.URL)) parentPageId = parentPage.Id;
            }
            return new WebPage
            {
                URL = urlManager.GetCorrectURL(url, parentPage.URL),
                ParentPageId = parentPageId,
                ReferralPageId = referralPageId,
                WebSiteId = sites.First(s => urlManager.IsBelongTo(new Uri(s.URL), new Uri(urlManager.GetCorrectURL(url, parentPage.URL)))).Id
            };
        }
        void SavePage(WebPage currentPage, HtmlDocument doc)
        {
            lock (storagePages)
            {
                if (storagePages.ContainsKey(currentPage.URL)) return;
            }
            int id = currentPages.Count() + 1;
            lock (currentPages)
            {
                lock (parsingStorage)
                    if (parsingStorage.GetWebPages().Count() > 0)
                    {
                        id += parsingStorage.GetWebPages().Last().Id;
                    }

                currentPages.Add(new WebPage
                {
                    Id = id,
                    URL = currentPage.URL,
                    TimeToLoad = int.Parse(GetTimeToLoad().ToString()),
                    HTML_Size = docManager.GetHtmlSize(doc),
                    ParentPageId = currentPage.ParentPageId,
                    ReferralPageId = currentPage.ReferralPageId,
                    WebSiteId = currentPage.WebSiteId
                });
            }
        }

        void SaveCurrentPages()
        {
            lock (currentPages)
            {
                parsingStorage.SaveWebPages(currentPages);
                foreach (WebPage page in currentPages) storagePages.Add(page.URL, parsingStorage.GetWebPage(page.URL));
                currentPages.Clear();
            }
        }

        void SaveImages(string pageUrl, List<string> imagesLinks)
        {
            foreach (string link in imagesLinks)
            {
                int id = currentPagesImages.Count() + 1;
                lock (currentPagesImages)
                {
                    lock (parsingStorage)
                        if (parsingStorage.GetImages().Count() > 0)
                        {
                            id += parsingStorage.GetImages().Last().Id;
                        }
                    currentPagesImages.Add(new ImageFile
                    {
                        Id = id,
                        URL = link,
                        Name = link
                    }, pageUrl);
                }
            }
        }
        void SaveCurrentPagesImages()
        {
            foreach (ImageFile key in currentPagesImages.Keys) lock(currentPagesImages)key.WebPageId = storagePages[currentPagesImages[key]].Id;
            parsingStorage.SaveImageFiles(currentPagesImages.Keys);
            currentPagesImages.Clear();
        }
        void SaveCssFiles(string pageUrl, List<string> cssFiles)
        {
            foreach (string link in cssFiles)
            {
                lock (currentPagesCssFiles)
                {
                    int id = currentPagesCssFiles.Count() + 1;
                    lock (parsingStorage) if (parsingStorage.GetCssFiles().Any())
                        {
                            id += parsingStorage.GetCssFiles().Last().Id;
                        }
                    currentPagesImages.Add(new ImageFile
                    {
                        Id = id,
                        URL = link,
                        Name = link
                    }, pageUrl);
                }
            }
        }
        void SaveCurrentPagesCssFiles()
        {
            foreach (CssFile key in currentPagesCssFiles.Keys) lock(currentPagesCssFiles)key.WebPageId = storagePages[currentPagesCssFiles[key]].Id;
            parsingStorage.SaveCssFiles(currentPagesCssFiles.Keys);
            currentPagesCssFiles.Clear();
        }
        WebPage GetParentPage(string startUrl, string currentUrl)//////////////////////////??????????
        {
            try
            {
                if (visitedPages.Count != 0 && links[currentUrl] != null)
                    return storagePages[links[currentUrl]];
                if (links[startUrl] != null && storagePages[links[startUrl]] != null)
                    return storagePages[links[startUrl]];
            }
            catch (Exception)
            {
                // ignored
            }
            return new WebPage { URL = startUrl };
        }

        HtmlDocument GetHtmlDocument(string url)
        {
            watch.Restart();
            HtmlDocument doc = docManager.GetHTMLDocument(url);
            watch.Stop();
            return doc;
        }
        long GetTimeToLoad()
        {
            return watch.ElapsedMilliseconds;
        }
    }
}
