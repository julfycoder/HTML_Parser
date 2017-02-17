using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.Business.Parsing;

namespace HTML_Parser.Business.Commands
{
    public class ParseCommand : ICommand
    {
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
            parser.Start(url, workerThreadsCount, depth, useForeignLinks);
        }
    }
}
