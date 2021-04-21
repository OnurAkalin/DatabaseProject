using System.Collections.Generic;
using System.Threading;

namespace DatabaseProject
{
    public class ThreadManager
    {
        private readonly Operations _operations;

        public ThreadManager(Operations operations)
        {
            _operations = operations;
        }

        public List<Thread> CreateTypeAThreads(int threadNumber)
        {
            List<Thread> threadListA = new List<Thread>();
            for (int i = 0; i < threadNumber; i++)
            {
                threadListA.Add(new Thread(new ThreadStart(_operations.ThreadTypeA)));
            }
            return threadListA;
        }
        
        public List<Thread> CreateTypeBThreads(int threadNumber)
        {
            List<Thread> threadListB = new List<Thread>();
            for (int i = 0; i < threadNumber; i++)
            {
                threadListB.Add(new Thread(new ThreadStart(_operations.ThreadTypeB)));
            }
            return threadListB;
        }

        public void StartAndJoinThreads(List<Thread> threadListTypeA, List<Thread> threadListTypeB)
        {
            foreach (var thread in threadListTypeA)
            {
                thread.Start();
            }

            foreach (var thread in threadListTypeB)
            {
                thread.Start();
            }
            
            foreach (var thread in threadListTypeA)
            {
                thread.Join();
            }

            foreach (var thread in threadListTypeB)
            {
                thread.Join();
            }
        }
    }
}