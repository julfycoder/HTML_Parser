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
using HTML_Parser.Business.Commands.Handlers;
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
                x.For<IHandlersChainFactory>().Use<HTML_ParserHandlersChainFactory>();
                x.For<CommandHandler>().Use<ParseCommandHandler>();
                x.For<CommandHandler>().Use<CreateSiteTreeCommandHandler>();
            });

            ICommandsManager manager = c.GetInstance<ICommandsManager>();

            manager.Start();
            Console.ReadLine();
        }
    }
}
