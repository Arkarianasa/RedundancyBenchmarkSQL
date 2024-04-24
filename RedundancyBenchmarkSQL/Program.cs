﻿using Microsoft.Data.SqlClient;
using RedundancyBenchmarkSQL;
using System;
using System.Data.Common;
using System.IO;
using Microsoft.Data.SqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;

namespace SQLRedundancyBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            RegisterDbProviders();

            string filePath = "../../../Resources/queries.sql";

            // Example connection strings for different databases
            string sqlServerConnectionString = "Server=dbsys.cs.vsb.cz\\STUDENT;Database=myDataBase;User Id=myUsername;Password=myPassword;Encrypt=True;TrustServerCertificate=True;";
            string oracleConnectionString = "Data Source=dbsys.cs.vsb.cz\\oracle;User Id=myUsername;Password=myPassword;";
            string mySqlConnectionString = "Server=myServerAddress;Port=myPort;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";
            string postgreConnectionString = "Host=dbsys.cs.vsb.cz;Port=5432;Username=myUsername;Password=myPassword;Database=myDataBase;";

            Benchmark benchmark = new Benchmark(filePath);
            
            benchmark.SetDatabaseSystem("Microsoft.Data.SqlClient", sqlServerConnectionString);
            benchmark.RunBenchmark();
            
            benchmark.SetDatabaseSystem("MySql", mySqlConnectionString);
            benchmark.RunBenchmark();

            
            benchmark.SetDatabaseSystem("Npgsql", postgreConnectionString);
            benchmark.RunBenchmark();
            
            benchmark.SetDatabaseSystem("Oracle.ManagedDataAccess.Client", oracleConnectionString);
            benchmark.RunBenchmark();
            

            // Print Results
            benchmark.PrintBenchmark();
            benchmark.PrintQueries();

            /*
            // Save results to .txt file
            using (StreamWriter writer = new StreamWriter("../../Output/test.txt"))
            {
                Console.SetOut(writer);

                benchmark.PrintBenchmark();
                benchmark.PrintQueries();
            }
            */

            // Wait for user input before closing the console window
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }

        private static void RegisterDbProviders()
        {
            // Register the SQL Client Factory
            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
            DbProviderFactories.RegisterFactory("Oracle.ManagedDataAccess.Client", OracleClientFactory.Instance);
            Console.WriteLine("Database providers registered successfully.");
        }
    }
}