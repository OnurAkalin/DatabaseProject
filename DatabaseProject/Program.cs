using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace DatabaseProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Operations operations = new Operations(IsolationLevel.ReadUncommitted);
            
            int typeAUserCount = 10;
            int typeBUserCount = 10;
            
            List<Thread> threadListA = new List<Thread>();
            List<Thread> threadListB = new List<Thread>();
            
            for (int i = 0; i < typeAUserCount; i++)
            {
                threadListA.Add(new Thread(new ThreadStart(operations.ThreadTypeA)));
            }

            foreach (var thread in threadListA)
            {
                thread.Start();
            }

            foreach (var thread in threadListA)
            {
                thread.Join();
            }

            for (int i = 0; i < typeBUserCount; i++)
            {
                threadListB.Add(new Thread(new ThreadStart(operations.ThreadTypeB)));
            }

            foreach (var thread in threadListB)
            {
                thread.Start();
            }

            foreach (var thread in threadListB)
            {
                thread.Join();
            }
            
            operations.WriteThreadReportsToFile("ReadUncommitted.txt");
        }
    }
}