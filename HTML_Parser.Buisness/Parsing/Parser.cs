using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Threading;
using System.Diagnostics;
using HTML_Parser.DAL.Data.Entities;
using HTML_Parser.DAL.Data;
using StructureMap;
using HTML_Parser.Business.Web;

namespace HTML_Parser.Business.Parsing
{

    public class Parser
    {
        IHTMLDocumentManager docManager;
        IURLManager urlManager;
        IContainer container;
        IParsingStorage storage;
        ISiteTreeRepository siteTreeRepository;

        HashSet<string> visitedPages = new HashSet<string>();

        int workerThreads = 10;
        bool useForeignLinks = true;

        Stopwatch watch = new Stopwatch();

        Dictionary<string, string> currentLinks = new Dictionary<string, string>();
        Dictionary<string, WebPage> storagePages = new Dictionary<string, WebPage>();

        int availableThreads = 0;
        public Parser(IContainer container)
        {
            this.container = container;
            docManager = container.GetInstance<IHTMLDocumentManager>();
            urlManager = container.GetInstance<IURLManager>();
            storage = container.GetInstance<IParsingStorage>();
            siteTreeRepository = container.GetInstance<ISiteTreeRepository>();
        }

        public void ChangeWorkerThreadsCount(int workerThreads = 10)
        {
            this.workerThreads = workerThreads;
        }

        public void Start(string url, int workerThreads = 10, int depth = -2, bool useForeignLinks = true)
        {
            storage.Initialize(container);
            foreach (WebPage page in storage.GetWebPages()) if (!storagePages.ContainsKey(page.URL)) storagePages.Add(page.URL, page);
            ThreadPool.SetMaxThreads(workerThreads, workerThreads);
            int avIOThreads = 0;
            ThreadPool.GetAvailableThreads(out availableThreads, out avIOThreads);

            this.useForeignLinks = useForeignLinks;
            this.workerThreads = workerThreads;

            currentLinks.Add(url, null);

            Parse(url, depth);
        }
        void ParsePage(object state)
        {
            WebPage parsingPage = (WebPage)state;

            string url = parsingPage.URL;
            visitedPages.Add(url);
            HtmlDocument doc = GetHtmlDocument(url);
            List<HtmlNode> nodes = docManager.GetAllNodes(doc);
            foreach (string link in docManager.GetLinks(nodes))
            {
                lock (currentLinks)
                {
                    if (!currentLinks.ContainsKey(link))
                    {
                        currentLinks.Add(link, url);
                    }
                }
            }
            lock (storage)
            {
                storage.SaveWebPages(new WebPage
                {
                    URL = url,
                    TimeToLoad = int.Parse(GetTimeToLoad().ToString()),
                    HTML_Size = docManager.GetHtmlSize(doc),
                    ParentPageId = parsingPage.ParentPageId,
                    ReferralPageId = parsingPage.ReferralPageId
                });
                WebPage lastPage = storage.GetWebPages().Last();
                if (!storagePages.ContainsKey(lastPage.URL)) storagePages.Add(lastPage.URL, lastPage);
            }

            foreach (string link in docManager.GetImagesUrls(nodes))
            {
                lock (storage)
                {
                    storage.SaveImageFiles(new ImageFile
                    {
                        URL = link,
                        WebPageId = storage.GetWebPages().Last(p => p.URL == url).Id,
                        Name = link
                    });
                }
            }
            foreach (string link in docManager.GetCssFilesUrls(nodes))
            {
                lock (storage)
                {
                    storage.SaveCssFiles(new CssFile
                    {
                        URL = link,
                        WebPageId = storage.GetWebPages().Last(p => p.URL == url).Id,
                        Name = link
                    });
                }
            }

            Console.WriteLine(url);
            Console.WriteLine("Size: " + docManager.GetHtmlSize(doc));
            Console.WriteLine("Time to load: " + GetTimeToLoad());
            Console.WriteLine();

        }

        void Parse(string url, int depth)
        {
            int childsCount = -1;
            while (currentLinks.Count != childsCount || visitedPages.Count == 0)
            {
                childsCount = currentLinks.Count;
                depth--;

                WaitAllThreads();
                List<string> keys = new List<string>(currentLinks.Keys);
                foreach (string link in keys)
                {
                    WebPage parent;
                    lock (currentLinks) parent = GetParentPage(url, link);
                    if ((urlManager.IsForeignURL(link, parent.URL) == false && useForeignLinks == false) || useForeignLinks == true)
                    {
                        if ((!visitedPages.Contains(urlManager.GetCorrectURL(link, parent.URL)) &&
                            urlManager.GetCorrectURL(link, parent.URL) != parent.URL && (depth > -2)) || (visitedPages.Count == 0))
                        {
                            int? parentPageId = null;
                            int? referralPageId = null;
                            if (link != parent.URL)
                            {
                                referralPageId = parent.Id;
                                if (!urlManager.IsForeignURL(link, parent.URL)) parentPageId = parent.Id;
                            }
                            ThreadPool.QueueUserWorkItem(ParsePage, new WebPage { URL = urlManager.GetCorrectURL(link, parent.URL), ParentPageId = parentPageId, ReferralPageId = referralPageId });
                        }
                    }
                }
            }
        }
        void WaitAllThreads()
        {
            int currentAvThreads = 0;
            while (currentAvThreads != availableThreads)                  //Waiting while all threads are working
            {
                int avIOThreads = 0;
                ThreadPool.GetAvailableThreads(out currentAvThreads, out avIOThreads);
            }
        }
        WebPage GetParentPage(string startUrl,string currentUrl)
        {
            WebPage parent;
            if (visitedPages.Count != 0 && currentLinks[currentUrl] != null) parent = storagePages[currentLinks[currentUrl]];
            else if (currentLinks[startUrl] != null && storagePages[currentLinks[startUrl]] != null)
                parent = storagePages[currentLinks[startUrl]];
            else parent = new WebPage { URL = startUrl };
            return parent;
        }
        public void CreateSiteTree(string url)
        {
            storage.Initialize(container);
            List<WebPage> webPages = storage.GetWebPages().ToList();
            siteTreeRepository.Initialize(container);
            siteTreeRepository.SaveSiteTree(url, webPages);
        }
        HtmlDocument GetHtmlDocument(string url)
        {
            watch.Restart();
            HtmlDocument doc = docManager.GetHTMLDocument(url);
            watch.Stop();
            return doc;
        }
        public long GetTimeToLoad()
        {
            return watch.ElapsedMilliseconds;
        }
    }
}
