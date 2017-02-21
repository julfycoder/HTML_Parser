using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.Business.Commands.Handlers;

namespace HTML_Parser.Business.Commands
{
    public class CommandsInterpreter : ICommandsInterpreter
    {
        ParseCommandHandler parseHandler;
        public CommandsInterpreter(Parsing.IParser parser,Parsing.ISiteTreeBuilder builder)
        {
            parseHandler = new ParseCommandHandler(parser);
            CreateSiteTreeCommandHandler cstHandler = new Handlers.CreateSiteTreeCommandHandler(builder);
            parseHandler.SetSuccessor(cstHandler);
        }
        public ICommand Interpret(string commandString)
        {
            string[] commandSegments = commandString.Split('|');
            return parseHandler.HandleCommand(commandSegments);
        }
    }
}
