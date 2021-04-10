using System;
using System.Data;
using System.Data.SqlClient;

namespace DatabaseProject
{
    public class Operations 
    {
        private readonly IsolationLevel _isolationLevel;
        private int _typeADeadlockCount;
        private int _typeBDeadlockCount;
        private int _otherExceptionsCount;

        private string _connectionString =
            @"Data Source=localhost;Initial Catalog=AdventureWorks2012;User ID=sa;Password=Onur-1234";

        public Operations(IsolationLevel isolationLevel)
        {
            _isolationLevel = isolationLevel;
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
            DateTime beginTime = DateTime.Now;

            for (int i = 0; i < 100; i++)
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction(_isolationLevel);

                Random random = new Random();
                double randomNumber = random.NextDouble();
                try
                {
                    if (randomNumber < 0.5)
                        UpdateQuery("20110101", "20111231", conn, transaction).ExecuteNonQuery();
                    if (randomNumber < 0.5)
                        UpdateQuery("20120101", "20121231", conn, transaction).ExecuteNonQuery();
                    if (randomNumber < 0.5)
                        UpdateQuery("20130101", "20131231", conn, transaction).ExecuteNonQuery();
                    if (randomNumber < 0.5)
                        UpdateQuery("20140101", "20141231", conn, transaction).ExecuteNonQuery();
                    if (randomNumber < 0.5)
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
                            _typeADeadlockCount++;
                            Console.WriteLine("Deadlock has been catched in ThreadTypeA, Total Deadlock is {0}", _typeADeadlockCount);
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine("ThreadTypeA ,Rollback Exception Type: {0}", ex2.GetType());
                            Console.WriteLine("Message: {0}", ex2.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _otherExceptionsCount++;
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
        }

        public void ThreadTypeB()
        {
            DateTime beginTime = DateTime.Now;
            
            for (int i = 0; i < 20; i++)
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction(_isolationLevel);

                Random random = new Random();
                double randomNumber = random.NextDouble();
                try
                {
                    if (randomNumber < 0.5)
                        SelectQuery("20110101", "20111231", conn, transaction).ExecuteNonQuery();
                    if (randomNumber < 0.5)
                        SelectQuery("20120101", "20121231", conn, transaction).ExecuteNonQuery();
                    if (randomNumber < 0.5)
                        SelectQuery("20130101", "20131231", conn, transaction).ExecuteNonQuery();
                    if (randomNumber < 0.5)
                        SelectQuery("20140101", "20141231", conn, transaction).ExecuteNonQuery();
                    if (randomNumber < 0.5)
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
                            _typeBDeadlockCount++;
                            Console.WriteLine("Deadlock has been catched in ThreadTypeB, Total Deadlock is {0}", _typeBDeadlockCount);
                            transaction.Rollback();
                        }
                        catch (Exception ex2)
                        {
                            Console.WriteLine("ThreadTypeB ,Rollback Exception Type: {0}", ex2.GetType());
                            Console.WriteLine("Message: {0}", ex2.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _otherExceptionsCount++;
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
        }
    }
}