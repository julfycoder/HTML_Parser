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
        readonly string _path;
        public FileWatcher(string path)
        {
            this._path = path;
            var watchForChangesThread = new Thread(WatchForChanges);
            watchForChangesThread.Start();
        }
        private void WatchForChanges()
        {
            var lastModified = File.GetLastWriteTime(_path);
            while (true)
            {
                if (File.GetLastWriteTime(_path) != lastModified && Changed != null)
                {
                    Changed(null, new FileSystemEventArgs(WatcherChangeTypes.Changed, AppDomain.CurrentDomain.BaseDirectory, _path));
                    lastModified = File.GetLastWriteTime(_path);
                }
                else Thread.Sleep(1000);
            }
        }
    }
}
