using HTML_Parser.Business.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace HTML_Parser.Business.Commands.Handlers
{
    public class ParseCommandHandler : CommandHandler
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        IParser parser;
        public ParseCommandHandler(IParser parser)
        {
            this.parser = parser;
        }
        public override ICommand HandleCommand(string[] commandSegments)
        {
            if (commandSegments[0] == "P")
            {
                ParseCommand command = new ParseCommand(parser);
                command.Initialize(new ParseCommandInfo
                {
                    WorkerThreadsCount = int.Parse(commandSegments[1]),
                    Depth = int.Parse(commandSegments[2]),
                    UseForeignLinks = bool.Parse(commandSegments[3]),
                    Url = commandSegments[4]
                });
                return command;
            }
            if (successor != null) return successor.HandleCommand(commandSegments);
            logger.Info("Unknown command");
            return null;
        }
    }
}
