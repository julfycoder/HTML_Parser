using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HTML_Parser.DAL.IO
{
    public interface IFileManager
    {
        event FileSystemEventHandler Changed;
        void CreateFile(string path, string name);
        void CreateFile(string path);
        void SaveString(string savingString, string filePath);
        string GetLastString(string filePath);
        string GetAllContent(string filePath);
        bool IsFileExists(string path);
        void Watch(string path);
        void Delete(string path);
        void Copy(string oldPath, string newPath);
    }
}
