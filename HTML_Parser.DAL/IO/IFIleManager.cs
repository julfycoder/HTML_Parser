using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.DAL.IO
{
    public interface IFileManager
    {
        void CreateFile(string path,string name);
        void CreateFile(string name);
        void SaveString(string savingString, string filePath);
        void SaveText(string savingString, string filePath);
    }
}
