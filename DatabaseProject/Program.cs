using System;
using System.Data;
using System.Threading;

namespace DatabaseProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Operations operations = new Operations(IsolationLevel.ReadUncommitted);
            int numOfThreads = 15;
            threadFunc(numOfThreads, operations);
        }

        public static void threadFunc(int numOfThreads,Operations operations)
        {
            WaitHandle[] waitHandles = new WaitHandle[numOfThreads];
            for (int i = 0; i < numOfThreads; i++)
            {
                var j = i;
                if(j % 2 == 0)
                {
                    var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                    var thread = new Thread(() =>
                    {
                        
                        Console.WriteLine("Thread{0} exits", j);
                        operations.ThreadTypeA(j/2+1).AfterRunReport();
                        handle.Set();
                    });
                    waitHandles[j] = handle;
                    thread.Start();
                }
                else
                {
                    var handle = new EventWaitHandle(false, EventResetMode.ManualReset);
                    var thread = new Thread(() =>
                    {
                        
                        Console.WriteLine("Thread{0} exits", j);
                        operations.ThreadTypeB(j/2+1).AfterRunReport();
                        handle.Set();
                    });
                    waitHandles[j] = handle;
                    thread.Start();
                }
            }
            WaitHandle.WaitAll(waitHandles);
            Console.WriteLine("Main thread exits");
            Console.Read();
        }

        public void ThreadARuntime(int runCount, Operations operations)
        {
            for(int i=0;i<runCount; i++)
            {
                operations.ThreadTypeA(runCount);
            }
        }
      
    }

    
}