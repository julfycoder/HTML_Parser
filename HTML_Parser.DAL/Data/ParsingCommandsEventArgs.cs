using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.DAL.Data
{
    public class ParsingCommandsEventArgs:EventArgs
    {
        public ParsingCommandsEventArgs(string commandString)
        {
            CommandsString = commandString;
        }
        public string CommandsString { get; set; }
    }
}
