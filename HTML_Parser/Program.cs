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
using HTML_Parser.Business.Commands;
using StructureMap;

namespace HTML_Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            Container c = new Container(x =>
            {
                x.Scan(s =>
                {
                    s.WithDefaultConventions();
                    s.AssembliesFromApplicationBaseDirectory();
                });
            });

            //string url = "https://www.wikipedia.org";
            //Parser p = new Parser(c.GetInstance<IHTMLDocumentManager>(), c.GetInstance<IURLManager>(), c.GetInstance<IParsingStorage>(), c.GetInstance<IThreadsManager>());//, c.GetInstance<ISiteTreeRepository>());
            ////p.Start(url, 10, 1, false);

            //ISiteTreeBuilder builder = new SiteTreeBuilder(c.GetInstance<ISiteTreeRepository>(), c.GetInstance<IParsingStorage>());
            //builder.CreateSiteTree(url);
            //Console.WriteLine("OK!");

            ICommandsManager manager = c.GetInstance<ICommandsManager>();

            manager.ExecuteNextCommand();
            Console.ReadLine();
        }
    }
}
