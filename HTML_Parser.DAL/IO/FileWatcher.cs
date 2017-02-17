using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace HTML_Parser.DAL.IO
{
    class FileWatcher
    {
        public event FileSystemEventHandler Changed;
        string path;
        public FileWatcher(string path)
        {
            this.path = path;
            Thread watchForChangesThread = new Thread(WatchForChanges);
            watchForChangesThread.Start();
        }
        void WatchForChanges()
        {
            DateTime lastModified = File.GetLastWriteTime(path);
            while (true)
            {
                if (File.GetLastWriteTime(path) != lastModified && Changed != null)
                {
                    Changed(null, new FileSystemEventArgs(WatcherChangeTypes.Changed, AppDomain.CurrentDomain.BaseDirectory, path));
                    lastModified = File.GetLastWriteTime(path);
                }
                else Thread.Sleep(1000);
            }
        }
    }
}
