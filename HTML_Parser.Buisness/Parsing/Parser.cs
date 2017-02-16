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

    public class Parser
    {
        IHTMLDocumentManager docManager;
        IURLManager urlManager;
        IThreadsManager threadsManager;
        IParsingStorage parsingStorage;
        ISiteTreeRepository siteTreeRepository;

        HashSet<string> visitedPages = new HashSet<string>();

        bool useForeignLinks = true;

        Stopwatch watch = new Stopwatch();

        Dictionary<string, string> links = new Dictionary<string, string>();
        Dictionary<string, WebPage> storagePages = new Dictionary<string, WebPage>();

        public Parser(IHTMLDocumentManager docManager,IURLManager urlManager,IParsingStorage parsingStorage,ISiteTreeRepository siteTreeRepository,IThreadsManager threadsManager)
        {
            this.docManager = docManager;
            this.urlManager = urlManager;
            this.parsingStorage = parsingStorage;
            this.siteTreeRepository = siteTreeRepository;
            this.threadsManager = threadsManager;
        }

        public void ChangeWorkerThreadsCount(int workerThreads = 10)
        {
            threadsManager.SetMaxThreads(workerThreads, 10);
        }

        public void Start(string url, int workerThreads = 10, int depth = -2, bool useForeignLinks = true)
        {
            foreach (WebPage page in parsingStorage.GetWebPages()) if (!storagePages.ContainsKey(page.URL)) storagePages.Add(page.URL, page);
            this.useForeignLinks = useForeignLinks;

            threadsManager.SetMaxThreads(workerThreads, 10);

            links.Add(url, null);

            Parse(url, depth);
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

            SavePage(int.Parse(GetTimeToLoad().ToString()), parsingPage);
            SaveImages(parsingPage.URL, docManager.GetImagesUrls(nodes));
            SaveCssFiles(parsingPage.URL, docManager.GetCssFilesUrls(nodes));

            Console.WriteLine(parsingPage.URL);
            //Console.WriteLine("Size: " + docManager.GetHtmlSize(doc));
            //Console.WriteLine("Time to load: " + GetTimeToLoad());
            Console.WriteLine();

        }

        void Parse(string url, int depth)
        {
            int childsCount = -1;
            while (links.Count != childsCount || visitedPages.Count == 0)
            {
                childsCount = links.Count;
                depth--;

                threadsManager.WaitAllThreads();

                List<string> keys = new List<string>(links.Keys);
                foreach (string link in keys)
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
        void SavePage(int htmlSize, WebPage currentPage)
        {
            WebPage lastPage;
            lock (parsingStorage)
            {
                parsingStorage.SaveWebPages(new WebPage
                {
                    URL = currentPage.URL,
                    TimeToLoad = int.Parse(GetTimeToLoad().ToString()),
                    HTML_Size = htmlSize,
                    ParentPageId = currentPage.ParentPageId,
                    ReferralPageId = currentPage.ReferralPageId
                });
                lastPage = parsingStorage.GetWebPages().Last();
                
            }
            lock (storagePages)
            {
                if (!storagePages.ContainsKey(lastPage.URL))
                {
                    storagePages.Add(lastPage.URL, lastPage);
                }
            }
        }
        void SaveImages(string url, List<string> imagesLinks)
        {
            foreach (string link in imagesLinks)
            {
                lock (parsingStorage)
                {
                    parsingStorage.SaveImageFiles(new ImageFile
                    {
                        URL = link,
                        WebPageId = parsingStorage.GetWebPage(url).Id,
                        Name = link
                    });
                }
            }
        }
        void SaveCssFiles(string url, List<string> cssFiles)
        {
            foreach (string link in cssFiles)
            {
                lock (parsingStorage)
                {
                    parsingStorage.SaveCssFiles(new CssFile
                    {
                        URL = link,
                        WebPageId = parsingStorage.GetWebPage(url).Id,
                        Name = link
                    });
                }
            }
        }
        WebPage GetParentPage(string startUrl,string currentUrl)//////////////////////////??????????
        {
            WebPage parent;
            if (visitedPages.Count != 0 && links[currentUrl] != null) parent = storagePages[links[currentUrl]];
            else if (links[startUrl] != null && storagePages[links[startUrl]] != null)
                parent = storagePages[links[startUrl]];
            else parent = new WebPage { URL = startUrl };
            return parent;
        }
        public void CreateSiteTree(string url)////////////////////////////??????????????????????
        {
            parsingStorage.Initialize();
            List<WebPage> webPages = parsingStorage.GetWebPages().ToList();
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
