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
        private CommandHandler _handler;

        public CommandsInterpreter(IHandlersChainFactory factory)
        {
            _handler = factory.CreateHandlersChain();
        }

        public ICommand Interpret(string commandString)
        {
            var commandSegments = commandString.Split('|');
            return _handler.HandleCommand(commandSegments);
        }
    }
}
