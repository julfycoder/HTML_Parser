using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Commands
{
    public class CommandsInterpreter : ICommandsInterpreter
    {
        Parsing.IParser parser;
        public CommandsInterpreter(Parsing.IParser parser)
        {
            this.parser = parser;
        }
        public ICommand Interpret(string commandString)
        {
            string[] commandParams = commandString.Split('|');
            switch (commandParams.Count())
            {
                case 4:
                    {
                        ParseCommand parseCommand = new Commands.ParseCommand(parser);
                        parseCommand.Initialize(new ParseCommandInfo
                        {
                            WorkerThreadsCount = int.Parse(commandParams[0]),
                            Depth = int.Parse(commandParams[1]),
                            UseForeignLinks = bool.Parse(commandParams[2]),
                            Url = commandParams[3]
                        });
                        return parseCommand;
                    }
                default: return null;
            }
        }
    }
}
