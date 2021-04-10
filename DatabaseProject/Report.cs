using System;

namespace DatabaseProject
{
    public class Report
    {
        private readonly TimeSpan _timeSpan;
        private readonly int _numberOfDeadlocks;

        public Report(TimeSpan timeSpan, int numberOfDeadlocks)
        {
            _timeSpan = timeSpan;
            _numberOfDeadlocks = numberOfDeadlocks;
        }

        public void AfterRunReport()
        {
            Console.WriteLine("Total time : " + _timeSpan + "\nNumber of Deadlocks : " + _numberOfDeadlocks);
        }
    }
}