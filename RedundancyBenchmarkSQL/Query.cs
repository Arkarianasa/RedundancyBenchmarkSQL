using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyBenchmarkSQL
{
    internal class Query
    {
        string Category;
        string Source = null;
        string Reference = null;
        string Description;

        string CorrectQuery;
        string RedundantQuery;

        Dictionary<string, List<string>> SqlServerPlan = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> OraclePlan = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> PostgreSqlPlan = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> MySqlPlan = new Dictionary<string, List<string>>();
        public Query(string category, string description, string correctQuery, string redundantQuery)
        {
            Category = category;
            Description = description;
            CorrectQuery = correctQuery;
            RedundantQuery = redundantQuery;
        }

        public void AddSourceAndReference(string source, string reference)
        {
            Source = source;
            Reference = reference;
        }

        public string GetCorrectQuery()
        {
            return CorrectQuery;
        }

        public string GetRedundantQuery()
        {
            return RedundantQuery;
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

            if (Source != null)
            {
                Console.WriteLine("Source: " + Source);
            }
            if (Reference != null)
            {
                Console.WriteLine("Reference: " + Reference);
            }

            Console.WriteLine("Description: " + Description);

            Console.WriteLine("Correct query:\n" + CorrectQuery);

            Console.WriteLine("Query with redundancy:\n" + RedundantQuery);

            Console.WriteLine("Results:");

            if (SqlServerPlan.Count() > 0)
            {
                Console.WriteLine("\nSQL Server:");
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
                Console.WriteLine("\nOracle:");
                Console.WriteLine("   Correct Query:");
                foreach (string operation in OraclePlan["correct"])
                {
                    Console.WriteLine("   - " + operation);
                }

                Console.WriteLine("\n   Query with redundancy:");
                foreach (string operation in OraclePlan["redundant"])
                {
                    Console.WriteLine("   - " + operation);
                }
            }

            if (MySqlPlan.Count() > 0)
            {
                Console.WriteLine("\nMy SQL:");
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
                Console.WriteLine("\nPostgre SQL:");
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
