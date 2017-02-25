using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Commands.Handlers
{
    public interface IHandlersChainFactory
    {
        CommandHandler CreateHandlersChain();
    }
}
