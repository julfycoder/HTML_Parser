using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HTML_Parser.Business.Threading
{
    public class ThreadsManager : IThreadsManager
    {
        int availableThreads;
        public void QueueWorkItem(WaitCallback workItem, object state)
        {
            ThreadPool.QueueUserWorkItem(workItem, state);
        }

        public void SetMaxThreads(int maxWorkerThreads,int maxIOThreads)
        {
            ThreadPool.SetMaxThreads(maxWorkerThreads, maxIOThreads);
            int avIOThreads = 0;
            ThreadPool.GetAvailableThreads(out availableThreads, out avIOThreads);
        }

        public void WaitAllThreads()
        {
            int currentAvThreads = 0;
            while (currentAvThreads != availableThreads)                  //Waiting while all threads are working
            {
                int avIOThreads = 0;
                ThreadPool.GetAvailableThreads(out currentAvThreads, out avIOThreads);
            }
        }
    }
}
