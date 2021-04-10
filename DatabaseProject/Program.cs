using System;
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

            Task task1 = Task.Factory.StartNew(() => operations.ThreadTypeA());
            Task task2 = Task.Factory.StartNew(() => operations.ThreadTypeA());
            Task task3 = Task.Factory.StartNew(() => operations.ThreadTypeA());
            Task task4 = Task.Factory.StartNew(() => operations.ThreadTypeA());

            Task.WaitAll(task1, task2, task3, task4);

            
        }
    }
}