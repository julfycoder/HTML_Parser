﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.SiteTree
{
    public interface ISiteTreeBuilder
    {
        void CreateSiteTree(string url);
    }
}
