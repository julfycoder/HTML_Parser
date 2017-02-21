using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Commands.Handlers
{
    public abstract class CommandHandler
    {
        protected CommandHandler successor;
        public void SetSuccessor(CommandHandler successor)
        {
            this.successor = successor;
        }
        public abstract ICommand HandleCommand(string[] commandSegments);
    }
}
