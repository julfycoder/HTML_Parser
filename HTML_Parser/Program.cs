using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;
using System.Threading;
using HTML_Parser.Business.Parsing;
using HTML_Parser.Business.Web;
using HTML_Parser.Business.Threading;
using HTML_Parser.DAL.Data;
using HTML_Parser.DAL.IO;
using HTML_Parser.DAL;
using StructureMap;

namespace HTML_Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            Container c = new Container(x => {
                x.For<IParseRepository>().Use<ParseRepository>();
                x.For<IHTMLDocumentManager>().Use<HTMLDocumentManager>();
                x.For<IURLManager>().Use<URLManager>();
                x.For<IParsingStorage>().Use<ParsingStorage>();
                x.For<ISiteTreeRepository>().Use<SiteTreeRepository>();
                x.For<IFileManager>().Use<FileManager>();
                x.For<ISiteTreeStringBuilder>().Use<SiteTreeStringBuilder>();
                x.For<IThreadsManager>().Use<ThreadsManager>();
            });

            string url = "https://www.wikipedia.org";
            Parser p = new Parser(c.GetInstance<IHTMLDocumentManager>(),c.GetInstance<IURLManager>(),c.GetInstance<IParsingStorage>(),c.GetInstance<ISiteTreeRepository>(),c.GetInstance<IThreadsManager>());
            p.Start(url, 10, 2,false);
            //p.CreateSiteTree(url);
            //Console.WriteLine("OK!");

            Console.ReadLine();
        }
    }
}
