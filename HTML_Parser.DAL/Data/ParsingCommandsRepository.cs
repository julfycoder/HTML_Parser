using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTML_Parser.DAL.IO;
using System.IO;

namespace HTML_Parser.DAL.Data
{
    public class ParsingCommandsRepository : IParsingCommandsRepository
    {
        IFileManager fileManager;
        string commandsFileName = "Commands.txt";

        public event ParsingCommandsHandler CommandAdded;

        public ParsingCommandsRepository(IFileManager fileManager)
        {
            this.fileManager = fileManager;
            fileManager.Changed += UpdateCommandAdded;
            fileManager.Watch(AppDomain.CurrentDomain.BaseDirectory+commandsFileName);
        }
        public string GetLastCommand()
        {
            string newFileName = "CommandsCopy.txt";
            fileManager.Copy(AppDomain.CurrentDomain.BaseDirectory + commandsFileName, AppDomain.CurrentDomain.BaseDirectory + newFileName);
            string lastString = fileManager.GetLastString(AppDomain.CurrentDomain.BaseDirectory + newFileName);
            fileManager.Delete(AppDomain.CurrentDomain.BaseDirectory + newFileName);
            return lastString;
        }
        void UpdateCommandAdded(object sender, FileSystemEventArgs e)
        {
            if (CommandAdded != null) CommandAdded(this, new Data.ParsingCommandsEventArgs(GetLastCommand()));
        }
    }
}
