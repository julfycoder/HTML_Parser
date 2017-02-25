using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Commands.Handlers
{
    public class HTML_ParserHandlersChainFactory:IHandlersChainFactory
    {
        private CommandHandler _handler;

        public HTML_ParserHandlersChainFactory(CommandHandler[] handlers)
        {
            for (int i = 1; i < handlers.Length; i++)
            {
                handlers[i - 1].SetSuccessor(handlers[i]);
            }
            _handler = handlers[0];
        }

        public CommandHandler CreateHandlersChain()
        {
            return _handler;
        }
    }
}
