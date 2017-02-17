using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data.Entities;
using HTML_Parser.DAL;
using HTML_Parser.DAL.IO;

namespace HTML_Parser.DAL.Data
{
    public class SiteTreeRepository : ISiteTreeRepository
    {
        IFileManager fileManager;
        ISiteTreeStringBuilder siteTreeStringBuilder;
        public SiteTreeRepository(IFileManager fileManager, ISiteTreeStringBuilder siteTreeStringBuilder)
        {
            this.fileManager = fileManager;
            this.siteTreeStringBuilder = siteTreeStringBuilder;
        }
        public void SaveSiteTree(string url, List<WebPage> webPages)
        {
            DateTime dt = DateTime.Now;
            if (webPages.Any(p => p.URL == url))
            {
                WebPage page = webPages.First(p => p.URL == url);
                List<string> siteTree = siteTreeStringBuilder.CreateSiteTree(page, webPages);
                string fileName = CreateFileName(url);
                fileManager.CreateFile(fileName);
                fileManager.SaveString(DateTime.Now.ToString(), fileName);
                foreach (string link in siteTree)
                {
                    fileManager.SaveString(link, fileName);
                }
            }
        }
        string CreateFileName(string url)
        {
            string fileExtension = ".txt";
            string fileName = "( " + new Uri(url).Host + " )__SiteTree";
            int count = 0;
            while (fileManager.IsFileExists(fileName + count.ToString() + fileExtension))
            {
                count++;
            }
            fileName += count.ToString() + fileExtension;
            return fileName;
        }
    }
}
