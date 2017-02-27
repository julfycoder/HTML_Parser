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
        private readonly IHTMLDocumentManager _docManager;
        private readonly IURLManager _urlManager;
        private readonly IThreadsManager _threadsManager;
        private readonly IParsingStorage _parsingStorage;

        private readonly HashSet<string> _visitedPages = new HashSet<string>();
        private readonly Dictionary<string, string> _links = new Dictionary<string, string>();
        private readonly List<WebSite> _sites = new List<WebSite>();

        private readonly List<WebPage> _currentPages = new List<WebPage>();
        private readonly Dictionary<ImageFile, string> _currentPagesImages = new Dictionary<ImageFile, string>();
        private readonly Dictionary<CssFile, string> _currentPagesCssFiles = new Dictionary<CssFile, string>();

        private bool _useForeignLinks = true;

        private readonly Stopwatch _watch = new Stopwatch();
        public Parser(IHTMLDocumentManager docManager, IURLManager urlManager, IParsingStorage parsingStorage, IThreadsManager threadsManager)//, ISiteTreeRepository siteTreeRepository)
        {
            _docManager = docManager;
            _urlManager = urlManager;
            _parsingStorage = parsingStorage;
            _threadsManager = threadsManager;
        }

        public void ChangeWorkerThreadsCount(int workerThreads = 10)
        {
            _threadsManager.SetMaxThreads(workerThreads, 10);
        }

        public void Start(string url, int workerThreads = 10, int depth = -2, bool useForeignLinks = true)
        {
            _parsingStorage.Initialize();
            _useForeignLinks = useForeignLinks;

            _threadsManager.SetMaxThreads(workerThreads, 10);

            Initialize();
            _links.Add(url, null);

            Parse(url, depth);
        }

        void Initialize()
        {
            _links.Clear();
            _visitedPages.Clear();
        }
        void ParsePage(object state)
        {
            var parsingPage = (WebPage)state;

            _visitedPages.Add(parsingPage.URL);
            var doc = GetHtmlDocument(parsingPage.URL);
            var nodes = _docManager.GetAllNodes(doc);
            var currentLinks = _docManager.GetLinks(nodes);
            foreach (string link in currentLinks)
            {
                if (_useForeignLinks || (!_useForeignLinks && !_urlManager.IsForeignURL(_urlManager.GetCorrectURL(link,parsingPage.URL), parsingPage.URL)))
                {
                    lock (_links)
                    {
                        if (!_links.ContainsKey(link))
                        {
                            _links.Add(link, parsingPage.URL);
                        }
                    }
                }
            }
            if (_parsingStorage.GetWebPage(parsingPage.URL) == null)
            {
                SavePage(parsingPage, doc);
                SaveImages(parsingPage.URL, _docManager.GetImagesUrls(nodes));
                SaveCssFiles(parsingPage.URL, _docManager.GetCssFilesUrls(nodes));
            }

            Console.WriteLine(parsingPage.URL);
            Console.WriteLine(GetTimeToLoad());
            Console.WriteLine();

        }

        void Parse(string url, int depth)
        {
            int childsCount = -1;
            while (_links.Count != childsCount || _visitedPages.Count == 0)
            {
                childsCount = _links.Count;
                depth--;

                var keys = new List<string>(_links.Keys);
                foreach (string link in keys)
                {
                    var parent = GetParentPage(url, link);
                    if ((!_visitedPages.Contains(_urlManager.GetCorrectURL(link, parent.URL)) && (depth > -2) &&
                        ((_urlManager.IsForeignURL(link, parent.URL) == false && _useForeignLinks == false) || _useForeignLinks == true)))
                    {
                        if ((_urlManager.GetCorrectURL(link, parent.URL) != parent.URL) || (_visitedPages.Count == 0))
                        {
                            Uri u;
                            if (Uri.TryCreate(_urlManager.GetCorrectURL(link, parent.URL), UriKind.RelativeOrAbsolute, out u))
                            {
                                SaveWebSite(_urlManager.GetCorrectURL(link), parent.URL);
                                _threadsManager.QueueWorkItem(ParsePage, CreatePage(link, parent));
                            }
                        }
                    }
                    _threadsManager.WaitAllThreads();
                    if (_currentPages.Count >= 5)
                    {
                        SaveCurrentPages();
                        SaveCurrentPagesCssFiles();
                        SaveCurrentPagesImages();
                    }
                }
                _threadsManager.WaitAllThreads();
                SaveCurrentPages();
                SaveCurrentPagesCssFiles();
                SaveCurrentPagesImages();
            }
        }
        void SaveWebSite(string url, string parentUrl)
        {
            if (!_sites.Any(s => _urlManager.IsBelongTo(new Uri(s.URL), new Uri(_urlManager.GetCorrectURL(url, parentUrl)))))
            {
                var site = new WebSite { URL = _urlManager.GetHostWithScheme(_urlManager.GetCorrectURL(url, parentUrl)) };
                lock (_parsingStorage)
                {
                    _parsingStorage.SaveWebSite(site);
                    site = _parsingStorage.GetWebSite(_urlManager.GetHostWithScheme(_urlManager.GetCorrectURL(url, parentUrl)));
                }
                lock (_sites) if (_sites.All(s => s.Id != site.Id)) _sites.Add(site);
            }
        }
        WebPage CreatePage(string url, WebPage parentPage)//////////////////////////////??????????????????????
        {
            int? parentPageId = null;
            int? referralPageId = null;
            if (url != parentPage.URL)
            {
                referralPageId = parentPage.Id;
                if (!_urlManager.IsForeignURL(url, parentPage.URL)) parentPageId = parentPage.Id;
            }
            return new WebPage
            {
                URL = _urlManager.GetCorrectURL(url, parentPage.URL),
                ParentPageId = parentPageId,
                ReferralPageId = referralPageId,
                WebSiteId = _sites.First(s => _urlManager.IsBelongTo(new Uri(s.URL), new Uri(_urlManager.GetCorrectURL(url, parentPage.URL)))).Id
            };
        }
        void SavePage(WebPage currentPage, HtmlDocument doc)
        {
            lock(_parsingStorage) if (_parsingStorage.GetWebPage(currentPage.URL) != null) return;
            int id = _currentPages.Count() + 1;
            lock (_currentPages)
            {
                lock (_parsingStorage)
                    if (_parsingStorage.GetWebPages().ToList().Any())
                    {
                        id += _parsingStorage.GetWebPages().ToList().Last().Id;
                    }

                _currentPages.Add(new WebPage
                {
                    Id = id,
                    URL = currentPage.URL,
                    TimeToLoad = int.Parse(GetTimeToLoad().ToString()),
                    HTML_Size = _docManager.GetHtmlSize(doc),
                    ParentPageId = currentPage.ParentPageId,
                    ReferralPageId = currentPage.ReferralPageId,
                    WebSiteId = currentPage.WebSiteId
                });
            }
        }

        void SaveCurrentPages()
        {
            lock (_currentPages)
            {
                _parsingStorage.SaveWebPages(_currentPages);
                _currentPages.Clear();
            }
        }

        void SaveImages(string pageUrl, List<string> imagesLinks)
        {
            foreach (string link in imagesLinks)
            {
                int id = _currentPagesImages.Count() + 1;
                lock (_currentPagesImages)
                {
                    lock (_parsingStorage)
                        if (_parsingStorage.GetImages().ToList().Count() > 0)
                        {
                            id += _parsingStorage.GetImages().ToList().Last().Id;
                        }
                    _currentPagesImages.Add(new ImageFile
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
            foreach (var key in _currentPagesImages.Keys)
                lock (_currentPagesImages)
                    key.WebPageId = _parsingStorage.GetWebPage(_currentPagesImages[key]).Id;//_storagePages[_currentPagesImages[key]].Id;
            _parsingStorage.SaveImageFiles(_currentPagesImages.Keys);
            _currentPagesImages.Clear();
        }
        void SaveCssFiles(string pageUrl, List<string> cssFiles)
        {
            foreach (string link in cssFiles)
            {
                lock (_currentPagesCssFiles)
                {
                    int id = _currentPagesCssFiles.Count() + 1;
                    lock (_parsingStorage) if (_parsingStorage.GetCssFiles().ToList().Any())
                        {
                            id += _parsingStorage.GetCssFiles().ToList().Last().Id;
                        }
                    _currentPagesCssFiles.Add(new CssFile
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
            foreach (CssFile key in _currentPagesCssFiles.Keys)
                lock (_currentPagesCssFiles)
                    key.WebPageId = _parsingStorage.GetWebPage(_currentPagesCssFiles[key]).Id;
            _parsingStorage.SaveCssFiles(_currentPagesCssFiles.Keys);
            _currentPagesCssFiles.Clear();
        }
        WebPage GetParentPage(string startUrl, string currentUrl)//////////////////////////??????????
        {
            try
            {
                if (_visitedPages.Count != 0 && _links[currentUrl] != null&& _parsingStorage.GetWebPage(_links[currentUrl])!=null)
                    return _parsingStorage.GetWebPage(_links[currentUrl]);
                if (_links[startUrl] != null && _parsingStorage.GetWebPage(_links[startUrl]) != null&& _parsingStorage.GetWebPage(_links[startUrl])!=null)
                    return _parsingStorage.GetWebPage(_links[startUrl]);
            }
            catch (Exception)
            {
                // ignored
            }
            return new WebPage { URL = startUrl };
        }

        HtmlDocument GetHtmlDocument(string url)
        {
            _watch.Restart();
            HtmlDocument doc = _docManager.GetHTMLDocument(url);
            _watch.Stop();
            return doc;
        }
        long GetTimeToLoad()
        {
            return _watch.ElapsedMilliseconds;
        }
    }
}
