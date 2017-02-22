﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data.Entities;

namespace HTML_Parser.DAL.Data
{
    public interface IParseRepository
    {
        IEnumerable<WebPage> GetWebPages();
        WebPage GetWebPage(int id);
        WebPage GetWebPage(string url);
        IEnumerable<WebSite> GetWebSites();
        WebSite GetWebSite(string url);
        WebSite GetWebSite(int id);
        IEnumerable<CssFile> GetCssFiles();
        IEnumerable<ImageFile> GetImageFiles();

        void AddEntity<T>(T entity) where T : Entity;
        void AddEntities<T>(IEnumerable<T> entities) where T : Entity;
        void RemoveEntity<T>(T entity) where T : Entity;
    }
}
