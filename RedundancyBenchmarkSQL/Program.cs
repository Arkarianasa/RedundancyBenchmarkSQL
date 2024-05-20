using Microsoft.Data.SqlClient;
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

            string benchmarkLevel = "loose";
            //string benchmarkLevel = "strict";

            bool filterQueries = true;

            string filtered = "";

            if (filterQueries)
                filtered += "_filtered";

            // Example connection strings for different databases
            string sqlServerConnectionString = "Server=dbsys.cs.vsb.cz\\STUDENT;Database=myDataBase;User Id=myUsername;Password=myPassword;Encrypt=True;TrustServerCertificate=True;";
            string oracleConnectionString = "Data Source=dbsys.cs.vsb.cz\\oracle;User Id=myUsername;Password=myPassword;";
            string mySqlConnectionString = "Server=myServerAddress;Port=myPort;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";
            string postgreConnectionString = "Host=dbsys.cs.vsb.cz;Port=5432;Username=myUsername;Password=myPassword;Database=myDataBase;";

            Benchmark benchmark = new Benchmark(filePath, benchmarkLevel, filterQueries);
            
            benchmark.SetDatabaseSystem("Microsoft.Data.SqlClient", sqlServerConnectionString);
            benchmark.RunBenchmark();
            
            benchmark.SetDatabaseSystem("MySql", mySqlConnectionString);
            benchmark.RunBenchmark();
            
            benchmark.SetDatabaseSystem("Npgsql", postgreConnectionString);
            benchmark.RunBenchmark();
            
            benchmark.SetDatabaseSystem("Oracle.ManagedDataAccess.Client", oracleConnectionString);
            benchmark.RunBenchmark();

            // Print Results
            Console.WriteLine();
            benchmark.PrintBenchmark();
            //benchmark.PrintQueries();

            // Capture the original standard output stream
            TextWriter originalOutput = Console.Out;

            // Save results to .txt file
            using (StreamWriter writer = new StreamWriter("../../../Output/benchmark_without_indexes_" + benchmarkLevel + filtered + ".txt"))
            {
                Console.SetOut(writer);

                // Get the current date and time
                DateTime currentDateTime = DateTime.Now;

                Console.WriteLine("Benchmark (" + benchmarkLevel + ") without indexes from: " + currentDateTime + "\n");

                benchmark.PrintBenchmark();
                Console.WriteLine();
                benchmark.PrintQueries();

                writer.Flush();
            }

            // Save Results to excel file
            benchmark.GenerateExcel("../../../Output/benchmark_without_indexes_" + benchmarkLevel + filtered + ".xlsx");

            // Restore the original standard output stream
            Console.SetOut(originalOutput);

            Console.WriteLine("\nFile Output/benchmark_without_indexes_" + benchmarkLevel + filtered + ".txt created.\n");
            Console.WriteLine("File Output/benchmark_without_indexes_" + benchmarkLevel + filtered + ".xlsx created.\n");

            // Reset query results
            benchmark.ResetBenchmark();

            benchmark.SetDatabaseSystem("Microsoft.Data.SqlClient", sqlServerConnectionString);
            benchmark.RunScript("../../../Resources/create_indexes_sqlserver.sql");
            benchmark.RunBenchmark();
            benchmark.RunScript("../../../Resources/delete_indexes_sqlserver.sql");
            
            benchmark.SetDatabaseSystem("MySql", mySqlConnectionString);
            benchmark.RunScript("../../../Resources/create_indexes_mysql.sql");
            benchmark.RunBenchmark();
            benchmark.RunScript("../../../Resources/delete_indexes_mysql.sql");

            benchmark.SetDatabaseSystem("Npgsql", postgreConnectionString);
            benchmark.RunScript("../../../Resources/create_indexes_postgresql.sql");
            benchmark.RunBenchmark();
            benchmark.RunScript("../../../Resources/delete_indexes_postgresql.sql");
            
            benchmark.SetDatabaseSystem("Oracle.ManagedDataAccess.Client", oracleConnectionString);
            benchmark.RunScript("../../../Resources/create_indexes_oracle.sql");
            benchmark.RunBenchmark();
            benchmark.RunScript("../../../Resources/delete_indexes_oracle.sql");

            // Print Results
            Console.WriteLine();
            benchmark.PrintBenchmark();
            //benchmark.PrintQueries();

            // Save results to .txt file
            using (StreamWriter writer = new StreamWriter("../../../Output/benchmark_with_indexes_" + benchmarkLevel + filtered + ".txt"))
            {
                Console.SetOut(writer);

                // Get the current date and time
                DateTime currentDateTime = DateTime.Now;

                Console.WriteLine("Benchmark (" + benchmarkLevel + ") with indexes from: " + currentDateTime + "\n");

                benchmark.PrintBenchmark();
                Console.WriteLine();
                benchmark.PrintQueries();

                writer.Flush();
            }

            // Restore the original standard output stream
            Console.SetOut(originalOutput);
            Console.WriteLine("\nFile Output/benchmark_with_indexes_" + benchmarkLevel + filtered + ".txt created.\n");

            // Save Results to excel file
            benchmark.GenerateExcel("../../../Output/benchmark_with_indexes_" + benchmarkLevel + filtered + ".xlsx");
            Console.WriteLine("File Output/benchmark_with_indexes_" + benchmarkLevel + filtered + ".xlsx created.\n");
        }

        private static void RegisterDbProviders()
        {
            // Register the SQL Client Factory
            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
            DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance);
            DbProviderFactories.RegisterFactory("Oracle.ManagedDataAccess.Client", OracleClientFactory.Instance);
            Console.WriteLine("Database providers registered successfully.\n");
        }
    }
}