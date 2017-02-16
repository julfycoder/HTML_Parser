using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Threading
{
    public interface IThreadsManager
    {
        void SetMaxThreads(int maxWorkerThreads,int maxIOThreads);
        void WaitAllThreads();
        void QueueWorkItem(WaitCallback workItem, object state);
    }
}
