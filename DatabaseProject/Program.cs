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

            int typeAUserCount = 5;
            int typeBUserCount = 1;

            ThreadManager threadManager = new ThreadManager(operations);
            var userAList = threadManager.CreateTypeAThreads(typeAUserCount);
            var userBList = threadManager.CreateTypeBThreads(typeBUserCount);
            threadManager.StartAndJoinThreads(userAList, userBList);
            operations.WriteThreadReportsToFile("ReadUncommitted10.txt", typeAUserCount, typeBUserCount);
            
        }
    }
}