using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.Data;

namespace HTML_Parser.Business.Commands
{
    public interface ICommandsManager
    {
        void WaitCommands(object sender,ParsingCommandsEventArgs e);
        void ExecuteNextCommand();
    }
}
