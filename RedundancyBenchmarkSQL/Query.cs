using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyBenchmarkSQL
{
    internal class Query
    {
        string Category;
        string Source = "";
        string Reference = "";
        string Description;

        //string CorrectQuery;
        //string RedundantQuery;
        Dictionary<string, string> DefaultQueries = new Dictionary<string, string>();

        Dictionary<string, string> SqlServerQueries = new Dictionary<string, string>();
        Dictionary<string, string> OracleQueries = new Dictionary<string, string>();
        Dictionary<string, string> PostgreSqlQueries = new Dictionary<string, string>();
        Dictionary<string, string> MySqlQueries = new Dictionary<string, string>();

        Dictionary<string, List<string>> SqlServerPlan = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> OraclePlan = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> PostgreSqlPlan = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> MySqlPlan = new Dictionary<string, List<string>>();

        public bool SqlServerComparison { get; set; } = false;
        public bool OracleComparison { get; set; } = false;
        public bool MySqlComparison { get; set; } = false;
        public bool PostgreSqlComparison { get; set; } = false;

        public Query(string category, string description, string correctQuery, string redundantQuery)
        {
            Category = category;
            Description = description;

            DefaultQueries.Add("correct", correctQuery);
            DefaultQueries.Add("redundant", redundantQuery);
        }

        public void SetSqlServerQueries(string correctQuery, string redundantQuery)
        {
            SqlServerQueries.Add("correct", correctQuery);
            SqlServerQueries.Add("redundant", redundantQuery);
        }

        public void SetOracleQueries(string correctQuery, string redundantQuery)
        {
            OracleQueries.Add("correct", correctQuery);
            OracleQueries.Add("redundant", redundantQuery);
        }

        public void SetPostgreSqlQueries(string correctQuery, string redundantQuery)
        {
            PostgreSqlQueries.Add("correct", correctQuery);
            PostgreSqlQueries.Add("redundant", redundantQuery);
        }

        public void SetMySqlQueries(string correctQuery, string redundantQuery)
        {
            MySqlQueries.Add("correct", correctQuery);
            MySqlQueries.Add("redundant", redundantQuery);
        }

        public void AddSourceAndReference(string source, string reference)
        {
            Source = source;
            Reference = reference;
        }

        public string GetCorrectQuery()
        {
            return DefaultQueries["correct"];
        }

        public string GetCorrectQuery(string providerName)
        {
            switch (providerName)
            {
                case "Microsoft.Data.SqlClient":
                    if (SqlServerQueries.Count() == 0)
                    {
                        return DefaultQueries["correct"];
                    }
                    else
                    {
                        return SqlServerQueries["correct"];
                    }
                case "Oracle.ManagedDataAccess.Client":
                    if (OracleQueries.Count() == 0)
                    {
                        return DefaultQueries["correct"];
                    }
                    else
                    {
                        return OracleQueries["correct"];
                    }
                case "MySql":
                    if (MySqlQueries.Count() == 0)
                    {
                        return DefaultQueries["correct"];
                    }
                    else
                    {
                        return MySqlQueries["correct"];
                    }
                case "Npgsql":
                    if (PostgreSqlQueries.Count() == 0)
                    {
                        return DefaultQueries["correct"];
                    }
                    else
                    {
                        return PostgreSqlQueries["correct"];
                    }
            }

            return DefaultQueries["correct"];
        }

        public string GetRedundantQuery()
        {
            return DefaultQueries["redundant"];
        }

        public string GetRedundantQuery(string providerName)
        {
            switch (providerName)
            {
                case "Microsoft.Data.SqlClient":
                    if (SqlServerQueries.Count() == 0)
                    {
                        return DefaultQueries["redundant"];
                    }
                    else
                    {
                        return SqlServerQueries["redundant"];
                    }
                case "Oracle.ManagedDataAccess.Client":
                    if (OracleQueries.Count() == 0)
                    {
                        return DefaultQueries["redundant"];
                    }
                    else
                    {
                        return OracleQueries["redundant"];
                    }
                case "MySql":
                    if (MySqlQueries.Count() == 0)
                    {
                        return DefaultQueries["redundant"];
                    }
                    else
                    {
                        return MySqlQueries["redundant"];
                    }
                case "Npgsql":
                    if (PostgreSqlQueries.Count() == 0)
                    {
                        return DefaultQueries["redundant"];
                    }
                    else
                    {
                        return PostgreSqlQueries["redundant"];
                    }
            }

            return DefaultQueries["redundant"];
        }

        public void SetSqlServerPlan(string key, List<string> plan)
        {
            SqlServerPlan[key] = plan;
        }

        public void SetOraclePlan(string key, List<string> plan)
        {
            OraclePlan[key] = plan;
        }

        public void SetPostgreSqlPlan(string key, List<string> plan)
        {
            PostgreSqlPlan[key] = plan;
        }

        public void SetMySqlPlan(string key, List<string> plan)
        {
            MySqlPlan[key] = plan;
        }

        public void Print()
        {
            Console.WriteLine("Category: " + Category);

            if (Source != "")
            {
                Console.WriteLine("Source: " + Source);
            }
            if (Reference != "")
            {
                Console.WriteLine("Reference: " + Reference);
            }

            Console.WriteLine("Description: " + Description);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Correct query:");
            Console.ResetColor();
            Console.WriteLine(DefaultQueries["correct"]);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Query with redundancy:");
            Console.ResetColor();
            Console.WriteLine(DefaultQueries["redundant"]);

            Console.WriteLine("Results:");

            if (SqlServerPlan.Count() > 0)
            {
                if (SqlServerComparison)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nSQL Server (same plans):");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nSQL Server (different plans):");
                }
                Console.ResetColor();
                Console.WriteLine("   Correct Query:");
                foreach (string operation in SqlServerPlan["correct"])
                {
                    Console.WriteLine("   - " + operation);
                }

                Console.WriteLine("\n   Query with redundancy:");
                foreach (string operation in SqlServerPlan["redundant"])
                {
                    Console.WriteLine("   - " + operation);
                }
            }

            if (OraclePlan.Count() > 0)
            {
                if (OracleComparison)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nOracle (same plans):");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nOracle (different plans):");
                }
                Console.ResetColor();
                Console.WriteLine("   Correct Query:");
                foreach (string operation in OraclePlan["correct"])
                {
                    Console.WriteLine("   " + operation);
                }

                Console.WriteLine("\n   Query with redundancy:");
                foreach (string operation in OraclePlan["redundant"])
                {
                    Console.WriteLine("   " + operation);
                }
            }

            if (MySqlPlan.Count() > 0)
            {
                if (MySqlComparison)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nMy SQL (same plans):");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nMy SQL (different plans):");
                }
                Console.ResetColor();
                Console.WriteLine("   Correct Query:");
                foreach (string operation in MySqlPlan["correct"])
                {
                    Console.WriteLine("   - " + operation);
                }

                Console.WriteLine("\n   Query with redundancy:");
                foreach (string operation in MySqlPlan["redundant"])
                {
                    Console.WriteLine("   - " + operation);
                }
            }

            if (PostgreSqlPlan.Count() > 0)
            {
                if (PostgreSqlComparison)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nPostgre SQL (same plans):");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nPostgre SQL (different plans):");
                }
                Console.ResetColor();
                Console.WriteLine("   Correct Query:");
                foreach (string operation in PostgreSqlPlan["correct"])
                {
                    Console.WriteLine("   - " + operation);
                }

                Console.WriteLine("\n   Query with redundancy:");
                foreach (string operation in PostgreSqlPlan["redundant"])
                {
                    Console.WriteLine("   - " + operation);
                }
            }

        }
    }
}
