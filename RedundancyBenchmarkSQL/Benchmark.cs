using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Azure;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace RedundancyBenchmarkSQL
{
    internal class Benchmark
    {
        Queries queries;

        private DbProviderFactory factory;
        private string connectionString;
        string providerName;

        int SqlServerPoints = -1;
        int OraclePoints = -1;
        int MySqlPoints = -1;
        int PostgreSqlPoints = -1;
        public Benchmark(string queriesFilePath)
        {
            queries = new Queries();
            queries.ReadQueriesFromFile(queriesFilePath);
        }

        public void ResetBenchmark()
        {
            SqlServerPoints = -1;
            OraclePoints = -1;
            MySqlPoints = -1;
            PostgreSqlPoints = -1;

            queries.ResetQueriesResults();
        }
        public void SetDatabaseSystem(string providerName, string connectionString)
        {
            this.providerName = providerName;
            
            if (providerName != "MySql")
                factory = DbProviderFactories.GetFactory(providerName);

            this.connectionString = connectionString;
        }

        public void RunBenchmark()
        {
            switch (providerName)
            {
                case "Microsoft.Data.SqlClient":
                    SqlServerPoints = 0;
                    break;
                case "Oracle.ManagedDataAccess.Client":
                    OraclePoints = 0;
                    break;
                case "MySql":
                    MySqlPoints = 0;
                    break;
                case "Npgsql":
                    PostgreSqlPoints = 0;
                    break;
            }
            for (int i = 0; i < queries.Count(); i++)
            {
                List<string> correctPlan = GetQueryExecutionPlan(queries.queryList[i].GetCorrectQuery(providerName));
                List<string> redundantPlan = GetQueryExecutionPlan(queries.queryList[i].GetRedundantQuery(providerName));

                switch (providerName)
                {
                    case "Microsoft.Data.SqlClient":
                        queries.queryList[i].SetSqlServerPlan("correct", correctPlan);
                        queries.queryList[i].SetSqlServerPlan("redundant", redundantPlan);
                        if (correctPlan.OrderBy(x => x).SequenceEqual(redundantPlan.OrderBy(x => x)))
                        {
                            SqlServerPoints++;
                            queries.queryList[i].SqlServerComparison = true;
                        }
                        break;
                    case "Oracle.ManagedDataAccess.Client":
                        queries.queryList[i].SetOraclePlan("correct", correctPlan);
                        queries.queryList[i].SetOraclePlan("redundant", redundantPlan);
                        if (correctPlan.OrderBy(x => x).SequenceEqual(redundantPlan.OrderBy(x => x)))
                        {
                            OraclePoints++;
                            queries.queryList[i].OracleComparison = true;
                        }
                        break;
                    case "MySql":
                        queries.queryList[i].SetMySqlPlan("correct", correctPlan);
                        queries.queryList[i].SetMySqlPlan("redundant", redundantPlan);
                        if (correctPlan.OrderBy(x => x).SequenceEqual(redundantPlan.OrderBy(x => x)))
                        {
                            MySqlPoints++;
                            queries.queryList[i].MySqlComparison = true;
                        }
                        break;
                    case "Npgsql":
                        queries.queryList[i].SetPostgreSqlPlan("correct", correctPlan);
                        queries.queryList[i].SetPostgreSqlPlan("redundant", redundantPlan);
                        if (correctPlan.OrderBy(x => x).SequenceEqual(redundantPlan.OrderBy(x => x)))
                        {
                            PostgreSqlPoints++;
                            queries.queryList[i].PostgreSqlComparison = true;
                        }
                        break;

                }
            }

            Console.WriteLine(providerName + " benchamark finished.");
        }

        public void PrintBenchmark()
        {
            Console.WriteLine("Benchmark Results:");
            if (SqlServerPoints > -1)
                Console.WriteLine("Microsoft SQL Server: " + SqlServerPoints.ToString() + " / " + queries.Count());

            if (OraclePoints > -1)
                Console.WriteLine("Oracle: " + OraclePoints.ToString() + " / " + queries.Count());

            if (MySqlPoints > -1)
                Console.WriteLine("My SQL: " + MySqlPoints.ToString() + " / " + queries.Count());

            if (PostgreSqlPoints > -1)
                Console.WriteLine("Postgre SQL: " + PostgreSqlPoints.ToString() + " / " + queries.Count());
        }
        public void PrintQueries()
        {
            Console.WriteLine("----------------------------------------------------------------------------------------------------");
            Console.WriteLine("Redundancy Benchmark for " + queries.Count() + " queries:");
            Console.WriteLine("----------------------------------------------------------------------------------------------------");
            queries.PrintQueries();
        }

        public List<string> GetQueryExecutionPlan(string query)
        {
            List<string> executionPlan = new List<string>();
            switch (providerName)
            {
                case "Microsoft.Data.SqlClient":
                    executionPlan = GetSqlServerExecutionPlan(query);
                    return executionPlan;
                case "Oracle.ManagedDataAccess.Client":
                    executionPlan = GetOracleExecutionPlan(query);
                    return executionPlan;
                case "MySql":
                    executionPlan = GetMySqlExecutionPlan(query);
                    return executionPlan;
                case "Npgsql":
                    executionPlan = GetPostgreExecutionPlan(query);
                    return executionPlan;
                default:
                    executionPlan.Add("Execution plan not supported for this provider.");
                    return executionPlan;
            }
        }

        private List<string> GetSqlServerExecutionPlan(string query)
        {
            List<string> planLines = new List<string>();

            // Create and open the connection
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();

                // Enable the execution plan output
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SET SHOWPLAN_ALL ON";
                    command.ExecuteNonQuery();
                }

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    // Execute the command and fetch the execution plan
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        int counter = 0;
                        while (reader.Read())
                        {
                            if (counter++ > 0)
                            {
                                string planLine = reader.GetString(0)
                                                + " (Est Rows: " + reader.GetFloat(8)
                                                + ", Est IO Cost: " + reader.GetFloat(9)
                                                + ", Est CPU Time: " + reader.GetFloat(10)
                                                + ", Avg Row Size: " + reader.GetInt32(11) + ")";
                                planLines.Add(planLine);
                            }
                        }
                    }
                }

                // Disable the execution plan output
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SET SHOWPLAN_ALL OFF";
                    command.ExecuteNonQuery();
                }

                return planLines;
            }
        }
        
        private List<string> GetMySqlExecutionPlan(string query)
        {
            List<string> plan = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string explainQuery = $"EXPLAIN {query}";
                MySqlCommand command = new MySqlCommand(explainQuery, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string selectType = reader["select_type"] as string ?? "Unknown";
                        string table = reader["table"] as string ?? "Unknown";
                        string type = reader["type"] as string ?? "Unknown";
                        string key = reader["key"] as string ?? "None";
                        string refColumn = reader["ref"] as string ?? "None";
                        string rows = reader["rows"]?.ToString() ?? "Unknown"; // Safe conversion to string
                        string filtered = reader["filtered"]?.ToString() ?? "Unknown"; // Safe conversion to string
                        string extra = reader["Extra"] as string ?? "None";

                        string detail = $"{selectType} on {table} ({type}), key: {key} comparing to {refColumn}, rows: {rows} (filtered {filtered}%), {extra}";
                        plan.Add(detail);
                    }
                }
            }

            return plan;
        }

        
        private List<string> GetPostgreExecutionPlan(string query)
        {
            List<string> operations = new List<string>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Use EXPLAIN to get the query execution plan
                string explainQuery = $"EXPLAIN {query}";
                using (NpgsqlCommand command = new NpgsqlCommand(explainQuery, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string planPart = reader.GetString(0);
                            operations.Add(planPart);
                        }
                    }
                }
            }

            return operations;
        }

        public List<string> GetOracleExecutionPlan(string query)
        {
            List<string> planLines = new List<string>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Set the current schema to LAS0084_SP
                string setSchemaQuery = "ALTER SESSION SET CURRENT_SCHEMA = LAS0084_SP";
                using (OracleCommand schemaCmd = new OracleCommand(setSchemaQuery, connection))
                {
                    schemaCmd.ExecuteNonQuery();
                }

                // Insert the execution plan of your query into PLAN_TABLE
                string explainPlanQuery = $"EXPLAIN PLAN FOR {query}";
                using (OracleCommand cmd = new OracleCommand(explainPlanQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                // Query the PLAN_TABLE to retrieve the execution plan
                string planTableQuery = @"
                SELECT PLAN_TABLE_OUTPUT 
                FROM TABLE(DBMS_XPLAN.DISPLAY('PLAN_TABLE', null, 'ALL'))";

                using (OracleCommand cmd = new OracleCommand(planTableQuery, connection))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        int counter = 0;
                        while (reader.Read())
                        {
                            if (counter++ > 1)
                            {
                                string planLine = reader.GetString(0);
                                planLines.Add(planLine);
                            }
                        }
                    }
                }
            }

            return planLines;
        }

        public void RunScript(string script)
        {
            switch (providerName)
            {
                case "Microsoft.Data.SqlClient":
                    SqlServerRunScript(script); break;
                case "Oracle.ManagedDataAccess.Client":
                    OracleRunScript(script); break;
                case "MySql":
                    MySqlRunScript(script); break;
                case "Npgsql":
                    PostgreRunScript(script); break;
            }

            Console.WriteLine(Path.GetFileName(script) + " script finished.");
        }
        private void SqlServerRunScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException("File not found", scriptPath);
            }

            using (var connection = new SqlConnection(connectionString)) // Ensure that `SqlConnection` is properly instantiated.
            {
                connection.Open();
                var command = connection.CreateCommand();
                SqlTransaction transaction = connection.BeginTransaction();
                command.Transaction = transaction;

                try
                {
                    string scriptContent = File.ReadAllText(scriptPath);
                    command.CommandText = scriptContent;
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log the error and rethrow or handle it as needed
                    throw new Exception("An error occurred while executing the script.", ex);
                }
            }
        }

        private void PostgreRunScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException("File not found", scriptPath);
            }

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;

                        try
                        {
                            string scriptContent = File.ReadAllText(scriptPath);
                            command.CommandText = scriptContent;
                            command.ExecuteNonQuery();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("An error occurred while executing the script.", ex);
                        }
                    }
                }
            }
        }

        private void MySqlRunScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException("File not found", scriptPath);
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand
                {
                    Connection = connection
                };
                var transaction = connection.BeginTransaction();
                command.Transaction = transaction;

                try
                {
                    string scriptContent = File.ReadAllText(scriptPath);
                    command.CommandText = scriptContent;
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("An error occurred while executing the script.", ex);
                }
            }
        }


        private void OracleRunScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                throw new FileNotFoundException("File not found", scriptPath);
            }

            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;

                        string[] scriptLines = File.ReadAllLines(scriptPath);
                        foreach (var line in scriptLines)
                        {
                            try
                            {
                                command.CommandText = line;
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                Console.WriteLine("Failed to execute script line: " + ex.Message);
                                throw; // This will allow for further upstack handling of the exception if necessary
                            }
                        }

                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Failed to execute script: " + ex.Message);
                    throw; // This will allow for further upstack handling of the exception if necessary
                }
            }
        }
    }
}
