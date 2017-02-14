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
        public void CreateFile(string name)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + name;
            FileStream fstream = File.Create(path);
            fstream.Close();
        }

        public void CreateFile(string path, string name)
        {
            FileStream fstream = File.Create(path + name);
            fstream.Close();
        }

        public void SaveString(string savingString, string filePath)
        {
            FileStream fstream = File.Open(filePath, FileMode.Append|FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(fstream);
            writer.WriteLine(savingString);
            writer.Close();
            fstream.Close();
        }
        public void SaveText(string savingText, string filePath)
        {
            FileStream fstream = File.Create(filePath);
            StreamWriter writer = new StreamWriter(fstream);
            writer.WriteLine(savingText);
            writer.Close();
            fstream.Close();
        }
    }
}
