using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace HTML_Parser.Business.Commands.Handlers
{
    public abstract class CommandHandler
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected string _commandKey;
        private CommandHandler _successor;
        public void SetSuccessor(CommandHandler/*<T>*/ successor)
        {
            _successor = successor;
        }
       
        public ICommand HandleCommand(string[] commandSegments)
        {
            if (commandSegments[0] == _commandKey)
            {
                return HandleConcreteCommand(commandSegments);
            }
            if (_successor != null) return _successor.HandleCommand(commandSegments);
            _logger.Info("Unknown command");
            return null;
        }

        protected abstract ICommand HandleConcreteCommand(string[] commandSegments);
    }
}
