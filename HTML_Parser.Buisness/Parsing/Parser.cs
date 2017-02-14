using System;
using System.Collections.Generic;
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

        HashSet<string> currentLinks = new HashSet<string>();

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
            ThreadPool.SetMaxThreads(workerThreads, workerThreads);
            this.useForeignLinks = useForeignLinks;

            currentLinks.Add(url);
            Parse(new HTMLParsingState(url, null, null, depth));
        }
        void ParsePage(object state)
        {
            HTMLParsingState parsingState = (HTMLParsingState)state;

            string url = parsingState.GetUrl();
            visitedPages.Add(url);
            HtmlDocument doc = GetHtmlDocument(url);

            lock (storage) storage.SaveWebPages(new WebPage
            {
                URL = url,
                TimeToLoad = int.Parse(GetTimeToLoad().ToString()),
                HTML_Size = docManager.GetHtmlSize(doc),
                ParentPageId = parsingState.GetParentPageId(),
                ReferralPageId = parsingState.GetReferralPageId()
            });
            if (parsingState.GetDepth() > -2)
            {
                List<HtmlNode> nodes = docManager.GetAllNodes(doc);

                //HashSet<string> currentLinks = new HashSet<string>();
                foreach (string link in docManager.GetLinks(nodes)) lock (currentLinks) currentLinks.Add(link);

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
        }

        void Parse(HTMLParsingState parsingState)
        {
            int childsCount = -1;
            string url = parsingState.GetUrl();
            int depth = parsingState.GetDepth();
            while (currentLinks.Count != childsCount || visitedPages.Count == 0)
            {
                depth--;
                int avThreads = 0;
                while (avThreads != workerThreads)                  //Waiting while all threads are working
                {
                    int avIOThreads = 0;
                    ThreadPool.GetAvailableThreads(out avThreads, out avIOThreads);
                }
                childsCount = currentLinks.Count;
                foreach (string link in currentLinks)
                {
                    if ((urlManager.IsForeignURL(link, url) == false && useForeignLinks == false) || useForeignLinks == true)
                    {
                        if ((!visitedPages.Contains(urlManager.GetCorrectURL(link, url)) &&
                            urlManager.GetCorrectURL(link, url) != url && (parsingState.GetDepth() != -2)) || (visitedPages.Count == 0))
                        {
                            int? parentPageId = null;

                            if (!urlManager.IsForeignURL(link, url) && (link != url))
                            {
                                lock (storage) parentPageId = storage.GetWebPages().Last(p => p.URL == url).Id;
                            }
                            ThreadPool.QueueUserWorkItem(ParsePage, new HTMLParsingState(urlManager.GetCorrectURL(link, url), parentPageId, storage.GetWebPages().Last(p => p.URL == url).Id, depth));
                        }
                    }
                }
            }
        }
        //void ParseCurrentLinks(HashSet<string> currentLinks, HTMLParsingState parsingState, string url)
        //{
        //    foreach (string link in currentLinks)
        //    {
        //        if ((urlManager.IsForeignURL(link, url) == false && useForeignLinks == false) || useForeignLinks == true)
        //        {
        //            if (!visitedPages.Contains(urlManager.GetCorrectURL(link, url)) &&
        //                urlManager.GetCorrectURL(link, url)!=url && (parsingState.GetDepth() > -2))
        //            {
        //                int? parentPageId = null;
        //                lock (storage)
        //                    if (!urlManager.IsForeignURL(link, url)) parentPageId = storage.GetWebPages().Last(p => p.URL == url).Id;
        //                ThreadPool.QueueUserWorkItem(Parse, new HTMLParsingState(urlManager.GetCorrectURL(link, url), parentPageId, storage.GetWebPages().Last(p => p.URL == url).Id, parsingState.GetDepth()));
        //            }
        //        }
        //    }
        //}
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
