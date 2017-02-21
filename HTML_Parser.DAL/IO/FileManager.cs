using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HTML_Parser.DAL.IO
{
    public class FileManager : IFileManager
    {
        
        public event FileSystemEventHandler Changed;

        public void CreateFile(string path)
        {
            FileStream fstream = File.Create(path);
            fstream.Close();
        }

        public void CreateFile(string path, string name)
        {
            FileStream fstream = File.Create(path + name);
            fstream.Close();
        }

        public string GetAllContent(string filePath)
        {
            FileStream fstream = File.Open(filePath, FileMode.OpenOrCreate);
            StreamReader streamReader = new StreamReader(fstream);
            string allContent = streamReader.ReadToEnd();
            streamReader.Close();
            fstream.Close();
            return allContent;
        }

        public string GetLastString(string filePath)
        {
            string allContent = GetAllContent(filePath);
            return allContent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        public void SaveString(string savingString, string filePath)
        {
            FileStream fstream = File.Open(filePath, FileMode.Append | FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fstream);
            writer.WriteLine(savingString);
            writer.Close();
            fstream.Close();
        }
        public bool IsFileExists(string path)
        {
            return File.Exists(path);
        }

        public void Watch(string path)
        {
            FileWatcher watcher = new FileWatcher(path);
            watcher.Changed += Changed;
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }

        public void Copy(string oldPath, string newPath)
        {
            File.Copy(oldPath, newPath);
        }
    }
}
