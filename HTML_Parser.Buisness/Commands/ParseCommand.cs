using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.Business.Parsing;
using NLog;

namespace HTML_Parser.Business.Commands
{
    public class ParseCommand : ICommand
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        IParser parser;
        string url;
        int depth, workerThreadsCount;
        bool useForeignLinks;
        public ParseCommand(IParser parser)
        {
            this.parser = parser;
        }
        public void Initialize(CommandInfoBase commandInfo)
        {
            ParseCommandInfo parseCommand = (ParseCommandInfo)commandInfo;
            url = parseCommand.Url;
            depth = parseCommand.Depth;
            workerThreadsCount = parseCommand.WorkerThreadsCount;
            useForeignLinks = parseCommand.UseForeignLinks;
        }
        public void Execute()
        {
            //try
            //{
                logger.Info("Start to parse '{0}', with {1} threads, with depth = {2}, foreign links = {3}", url, workerThreadsCount, depth, useForeignLinks);
                parser.Start(url, workerThreadsCount, depth, useForeignLinks);
                logger.Info("End of parsing");
            //}
            //catch (Exception e) { logger.Error(e.Message); }
        }
    }
}
