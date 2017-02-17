using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Parsing
{
    public interface IParser
    {
        void Start(string url, int workerThreads = 10, int depth = -2, bool useForeignLinks = true);
        void ChangeWorkerThreadsCount(int workerThreads = 10);
    }
}
