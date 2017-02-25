using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Commands
{
    public class ParseCommandInfo : CommandInfoBase
    {
        public int Depth { get; set; }
        public bool UseForeignLinks { get; set; }
        public int WorkerThreadsCount { get; set; }
    }
}
