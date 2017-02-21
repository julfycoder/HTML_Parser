using System;
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
            foreach (string link in docManager.GetLinks(nodes))
            {
                if (useForeignLinks || (!useForeignLinks && !urlManager.IsForeignURL(link, parsingPage.URL)))
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

            SavePage(parsingPage);
            SaveImages(parsingPage.URL, docManager.GetImagesUrls(nodes));
            SaveCssFiles(parsingPage.URL, docManager.GetCssFilesUrls(nodes));

            Console.WriteLine(parsingPage.URL);
            Console.WriteLine();

        }

        void Parse(string url, int depth)
        {
            int childsCount = -1;
            while (links.Count != childsCount || visitedPages.Count == 0)
            {
                childsCount = links.Count;
                depth--;

                foreach (string link in links.Keys)
                {
                    WebPage parent = GetParentPage(url, link);
                    if ((!visitedPages.Contains(urlManager.GetCorrectURL(link, parent.URL)) && (depth > -2) &&
                        ((urlManager.IsForeignURL(link, parent.URL) == false && useForeignLinks == false) || useForeignLinks == true)))
                    {
                        if ((urlManager.GetCorrectURL(link, parent.URL) != parent.URL) || (visitedPages.Count == 0))
                        {
                            threadsManager.QueueWorkItem(ParsePage, CreatePage(link, parent));
                        }
                    }
                }
                threadsManager.WaitAllThreads();
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
                ReferralPageId = referralPageId
            };
        }
        void SavePage(WebPage currentPage)
        {
            lock (storagePages)
            {
                if (storagePages.ContainsKey(currentPage.URL)) return;
            }
            WebPage lastPage;
            lock (parsingStorage)
            {
                parsingStorage.SaveWebPage(new WebPage
                {
                    URL = currentPage.URL,
                    TimeToLoad = int.Parse(GetTimeToLoad().ToString()),
                    HTML_Size = currentPage.HTML_Size,
                    ParentPageId = currentPage.ParentPageId,
                    ReferralPageId = currentPage.ReferralPageId
                });
                lastPage = parsingStorage.GetWebPages().Last();
            }
            lock (storagePages) if (!storagePages.ContainsKey(lastPage.URL)) storagePages.Add(lastPage.URL, lastPage);
        }
        void SaveImages(string pageUrl, List<string> imagesLinks)
        {
            WebPage page;
            lock (parsingStorage) page = parsingStorage.GetWebPage(pageUrl);
            foreach (string link in imagesLinks)
            {
                lock (parsingStorage)
                {
                    parsingStorage.SaveImageFile(new ImageFile
                    {
                        URL = link,
                        WebPageId = page.Id,
                        Name = link
                    });
                }
            }
        }
        void SaveCssFiles(string pageUrl, List<string> cssFiles)
        {
            WebPage page;
            lock (parsingStorage) page = parsingStorage.GetWebPage(pageUrl);
            foreach (string link in cssFiles)
            {
                lock (parsingStorage)
                {
                    parsingStorage.SaveCssFile(new CssFile
                    {
                        URL = link,
                        WebPageId = page.Id,
                        Name = link
                    });
                }
            }
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
            catch (Exception) { }
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
