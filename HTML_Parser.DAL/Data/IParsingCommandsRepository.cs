using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.DAL.Data
{
    public delegate void ParsingCommandsHandler(object sender, ParsingCommandsEventArgs e);
    public interface IParsingCommandsRepository
    {
        event ParsingCommandsHandler CommandAdded;
        string GetLastCommand();
    }
}
