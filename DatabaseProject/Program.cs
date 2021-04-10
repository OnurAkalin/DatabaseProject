using System.Data;

namespace DatabaseProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Operations operations = new Operations(IsolationLevel.ReadUncommitted);
            operations.ThreadTypeA().AfterRunReport();
        }
    }
}