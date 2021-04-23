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
            Operations operations = new Operations(IsolationLevel.Serializable);

            int typeBUserCount = 2;
            int typeAUserCount = 2;

            ThreadManager threadManager = new ThreadManager(operations);
            var userAList = threadManager.CreateTypeAThreads(typeAUserCount);
            var userBList = threadManager.CreateTypeBThreads(typeBUserCount);
            threadManager.StartAndJoinThreads(userAList, userBList);
            operations.WriteThreadReportsToFile("/Users/onur/Desktop/ReadUncommitted2.txt", typeAUserCount, typeBUserCount);
            
            
        }
    }
}