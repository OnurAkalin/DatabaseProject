using System;
using System.IO;

namespace DatabaseProject
{
    public class Report
    {
        private TimeSpan _typeATotalTime;
        private TimeSpan _typeBTotalTime;
        private int _typeADeadlockCount; 
        private int _typeBDeadlockCount; 
        private int _otherExceptionsCount;

        public Report()
        {
            _typeATotalTime = TimeSpan.Zero;
            _typeBTotalTime = TimeSpan.Zero;
            _typeADeadlockCount = 0;
            _typeBDeadlockCount = 0;
            _otherExceptionsCount = 0;
        }
        public void IncreaseTypeADeadlockCount()
        {
            _typeADeadlockCount++;
        }

        public void IncreaseTypeBDeadlockCount()
        {
            _typeBDeadlockCount++;
        }

        public void IncreaseOtherExdeptionCount()
        {
            _otherExceptionsCount++;
        }
        public void IncreaseTypeATotalTime(TimeSpan totalTime)
        {
            _typeATotalTime += totalTime;
        }
        
        public void IncreaseTypeBTotalTime(TimeSpan totalTime)
        {
            _typeBTotalTime += totalTime;
        }
    }
}