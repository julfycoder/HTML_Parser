using HTML_Parser.Business.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HTML_Parser.Business.Commands.Handlers
{
    public class ParseCommandHandler : CommandHandler
    {
        private readonly IParser _parser;

        public ParseCommandHandler(IParser parser)
        {
            _parser = parser;
            _commandKey = "P";
        }

        protected override ICommand HandleConcreteCommand(string[] commandSegments)
        {
            return new ParseCommand(_parser,
                new ParseCommandInfo
                {
                    WorkerThreadsCount = int.Parse(commandSegments[1]),
                    Depth = int.Parse(commandSegments[2]),
                    UseForeignLinks = bool.Parse(commandSegments[3]),
                    Url = commandSegments[4]
                });
        }
    }
}
