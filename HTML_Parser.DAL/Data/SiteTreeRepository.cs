using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data.Entities;
using HTML_Parser.DAL;
using HTML_Parser.DAL.IO;
using NLog;

namespace HTML_Parser.DAL.Data
{
    public class SiteTreeRepository : ISiteTreeRepository
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IFileManager _fileManager;
        public SiteTreeRepository(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }
        public void SaveSiteTree(string url, List<string> siteTree)
        {
            _logger.Info("Saving site tree...");
            try
            {
                var fileName = CreateFileName(url);
                _logger.Info("File name: " + fileName);
                _fileManager.CreateFile(fileName);
                _fileManager.SaveString(DateTime.Now.ToString(), fileName);
                foreach (var link in siteTree)
                {
                    _fileManager.SaveString(link, fileName);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }

        private string CreateFileName(string url)
        {
            const string fileExtension = ".txt";
            var fileName = AppDomain.CurrentDomain.BaseDirectory + "( " + new Uri(url).Host + " )__SiteTree";
            var count = 0;
            while (_fileManager.IsFileExists(fileName + count.ToString() + fileExtension))
            {
                count++;
            }
            fileName += count.ToString() + fileExtension;
            return fileName;
        }
    }
}
