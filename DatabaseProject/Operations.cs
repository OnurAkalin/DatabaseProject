using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace DatabaseProject
{
    public class Operations
    {
        private readonly IsolationLevel _isolationLevel;
        private int _typeADeadlockCount;
        private int _typeBDeadlockCount;
        private int _otherExceptionsCount;
        private TimeSpan _typeATotalTime;
        private TimeSpan _typeBTotalTime;
        private readonly object _threadLock;

        private string _connectionString =
            @"Data Source=localhost;Initial Catalog=AdventureWorks2012;User ID=sa;Password=Onur-1234;Connection Timeout = 6000";

        public Operations(IsolationLevel isolationLevel)
        {
            _isolationLevel = isolationLevel;
            _typeADeadlockCount = 0;
            _typeBDeadlockCount = 0;
            _otherExceptionsCount = 0;
            _typeATotalTime = TimeSpan.Zero;
            _typeBTotalTime = TimeSpan.Zero;
            _threadLock = new object();
        }

        private SqlCommand UpdateQuery(string beginDate, string endDate, SqlConnection connection,
            SqlTransaction transaction)
        {
            SqlCommand command = new SqlCommand(
                "UPDATE Sales.SalesOrderDetail " +
                "SET UnitPrice = UnitPrice * 10.0 / 10.0 " +
                "WHERE UnitPrice > 100 " +
                "AND EXISTS (SELECT * FROM Sales.SalesOrderHeader " +
                "WHERE Sales.SalesOrderHeader.SalesOrderID = " +
                "Sales.SalesOrderDetail.SalesOrderID " +
                "AND Sales.SalesOrderHeader.OrderDate " +
                "BETWEEN @BeginDate AND @EndDate " +
                "AND Sales.SalesOrderHeader.OnlineOrderFlag = 1)",
                connection, transaction);

            command.Parameters.AddWithValue("@BeginDate", beginDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            return command;
        }

        private SqlCommand SelectQuery(string beginDate, string endDate, SqlConnection connection,
            SqlTransaction transaction)
        {
            SqlCommand command = new SqlCommand(
                "SELECT SUM(Sales.SalesOrderDetail.OrderQty) " +
                "FROM Sales.SalesOrderDetail " +
                "WHERE UnitPrice > 100 " +
                "AND EXISTS (SELECT * FROM Sales.SalesOrderHeader " +
                "WHERE Sales.SalesOrderHeader.SalesOrderID = " +
                "Sales.SalesOrderDetail.SalesOrderID " +
                "AND Sales.SalesOrderHeader.OrderDate " +
                "BETWEEN @BeginDate AND @EndDate " +
                "AND Sales.SalesOrderHeader.OnlineOrderFlag = 1)",
                connection, transaction);

            command.Parameters.AddWithValue("@BeginDate", beginDate);
            command.Parameters.AddWithValue("@EndDate", endDate);

            return command;
        }

        public void ThreadTypeA()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlTransaction transaction = null;
            DateTime beginTime = DateTime.Now;
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction(_isolationLevel);

                    Random random = new Random();
                    
                    if (random.NextDouble() < 0.5)
                        UpdateQuery("20110101", "20111231", conn, transaction).ExecuteNonQuery();
                    if (random.NextDouble() < 0.5)
                        UpdateQuery("20120101", "20121231", conn, transaction).ExecuteNonQuery();
                    if (random.NextDouble() < 0.5)
                        UpdateQuery("20130101", "20131231", conn, transaction).ExecuteNonQuery();
                    if (random.NextDouble() < 0.5)
                        UpdateQuery("20140101", "20141231", conn, transaction).ExecuteNonQuery();
                    if (random.NextDouble() < 0.5)
                        UpdateQuery("20150101", "20151231", conn, transaction).ExecuteNonQuery();

                    transaction.Commit();
                    conn.Close();
                }
                catch (SqlException ex1)
                {
                    if (ex1.Number == 1205)
                    {
                        try
                        {
                            lock (_threadLock)
                            {
                                _typeADeadlockCount++;
                            }

                            //Console.WriteLine("Deadlock has been catched in ThreadTypeA, Total Deadlock is {0}", _typeADeadlockCount);
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine("ThreadTypeA ,Rollback Exception Type: {0}", ex2.GetType());
                            lock (_threadLock)
                            {
                                _otherExceptionsCount++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    lock (_threadLock)
                    {
                        _otherExceptionsCount++;
                    }
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            DateTime endTime = DateTime.Now;
            TimeSpan elapsed = endTime - beginTime;
            //Console.WriteLine(" A - > Begin time : " +  beginTime + " End time : " + endTime  + " Total: " + elapsed);
            lock (_threadLock)
            {
                _typeATotalTime += elapsed;
            }
        }

        public void ThreadTypeB()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlTransaction transaction = null;
            DateTime beginTime = DateTime.Now;

            for (int i = 0; i < 100; i++)
            {
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction(_isolationLevel);

                    Random random = new Random();
                    
                    if (random.NextDouble() < 0.5)
                        SelectQuery("20110101", "20111231", conn, transaction).ExecuteNonQuery();
                    if (random.NextDouble() < 0.5)
                        SelectQuery("20120101", "20121231", conn, transaction).ExecuteNonQuery();
                    if (random.NextDouble() < 0.5)
                        SelectQuery("20130101", "20131231", conn, transaction).ExecuteNonQuery();
                    if (random.NextDouble() < 0.5)
                        SelectQuery("20140101", "20141231", conn, transaction).ExecuteNonQuery();
                    if (random.NextDouble() < 0.5)
                        SelectQuery("20150101", "20151231", conn, transaction).ExecuteNonQuery();

                    transaction.Commit();
                    conn.Close();
                }
                catch (SqlException ex1)
                {
                    if (ex1.Number == 1205)
                    {
                        try
                        {
                            lock (_threadLock)
                            {
                                _typeBDeadlockCount++;
                            }

                            //Console.WriteLine("Deadlock has been catched in ThreadTypeB, Total Deadlock is {0}", _typeBDeadlockCount);
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine("ThreadTypeB ,Rollback Exception Type: {0}", ex2.GetType());
                            Console.WriteLine("Message: {0}", ex2.Message);
                            lock (_threadLock)
                            {
                                _otherExceptionsCount++;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    lock (_threadLock)
                    {
                        _otherExceptionsCount++;
                    }
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            DateTime endTime = DateTime.Now;
            TimeSpan elapsed = endTime - beginTime;
            //Console.WriteLine("B -> Begin time : " +  beginTime + " End time : " + endTime  + " Total: " + elapsed);
            lock (_threadLock)
            {
                _typeBTotalTime += elapsed;
            }
        }

        public void WriteThreadReportsToFile(string fileName, int typeAUserCount, int typeBUserCount)
        {
            string[] lines =
            {
                "***************************************************************",
                "TypeA User Count : " + typeAUserCount + " TypeB User Count : " + typeBUserCount,
                "TypeA Deadlock Count : " + _typeADeadlockCount,
                "TypeB Deadlock Count : " + _typeBDeadlockCount,
                "TypeA Total Time Cost : " + (_typeATotalTime / typeAUserCount),
                "TypeB Total Time Cost : " + (_typeBTotalTime / typeBUserCount),
                "Other Exception Count : " + _otherExceptionsCount,
                "***************************************************************\n"
            };
            File.AppendAllLines(fileName, lines);
        }
    }
}