using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.IO;

namespace HTML_Parser.DAL.Data
{
    public class ParsingCommandsRepository : IParsingCommandsRepository
    {
        IFileManager fileManager;
        string commandsFileName = "Commands.txt";
        public ParsingCommandsRepository(IFileManager fileManager)
        {
            this.fileManager = fileManager;
        }
        public string GetLastCommand()
        {
            return fileManager.GetLastString(commandsFileName);
        }
    }
}
