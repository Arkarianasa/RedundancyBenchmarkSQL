using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyBenchmarkSQL
{
    internal class Queries
    {
        public List<Query> queryList { get; set; }

        public Queries()
        {
            queryList = new List<Query>();
        }

        public void AddQuery(Query query)
        {
            queryList.Add(query);
        }

        public Query GetQuery(int index)
        {
            if (index >= 0 && index < queryList.Count)
            {
                return queryList[index];
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
        }

        public int Count()
        {
            return queryList.Count;
        }

        public void PrintQueries()
        {
            foreach (var query in queryList)
            {
                query.Print();
                Console.WriteLine("----------------------------------------------------------------------------------------------------");
            }
        }

        public void ReadQueriesFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            string[] lines = File.ReadAllLines(filePath);

            string category = "";
            string source = "";
            string reference = "";
            string description = "";
            string version = "";

            string redundancyQuery = "";
            string correctQuery = "";

            string sqlserverRedundancyQuery = "";
            string sqlserverCorrectQuery = "";

            string oracleRedundancyQuery = "";
            string oracleCorrectQuery = "";

            string postgreRedundancyQuery = "";
            string postgreCorrectQuery = "";

            string mysqlRedundancyQuery = "";
            string mysqlCorrectQuery = "";

            foreach (string line in lines)
            {
                if (line.StartsWith("-- end"))
                {
                    Query currentQuery = new Query(category, description, correctQuery, redundancyQuery);

                    if (source != "" && reference != "")
                        currentQuery.AddSourceAndReference(source, reference);

                    if (sqlserverRedundancyQuery != "" && sqlserverCorrectQuery != "")
                        currentQuery.SetSqlServerQueries(sqlserverCorrectQuery, sqlserverRedundancyQuery);

                    if (oracleRedundancyQuery != "" && oracleCorrectQuery != "")
                        currentQuery.SetOracleQueries(oracleCorrectQuery, oracleRedundancyQuery);

                    if (postgreRedundancyQuery != "" && postgreCorrectQuery != "")
                        currentQuery.SetPostgreSqlQueries(postgreCorrectQuery, postgreRedundancyQuery);

                    if (mysqlRedundancyQuery != "" && mysqlCorrectQuery != "")
                        currentQuery.SetMySqlQueries(mysqlCorrectQuery, mysqlRedundancyQuery);

                    AddQuery(currentQuery);

                    // reset parameters for next query                    
                    source = "";
                    reference = "";

                    description = "";
                    version = "";

                    redundancyQuery = "";
                    correctQuery = "";

                    sqlserverRedundancyQuery = "";
                    sqlserverCorrectQuery = "";

                    oracleRedundancyQuery = "";
                    oracleCorrectQuery = "";

                    postgreRedundancyQuery = "";
                    postgreCorrectQuery = "";

                    mysqlRedundancyQuery = "";
                    mysqlCorrectQuery = "";
                }
                else if (line.StartsWith("-- Category:"))
                {
                    category = line.Split(':')[1].Trim();
                }
                else if (line.StartsWith("-- Source:"))
                {
                    source = line.Split(':')[1].Trim();
                }
                else if (line.StartsWith("-- Reference:"))
                {
                    reference = line.Split(':')[1].Trim();
                }
                else if (line.StartsWith("-- Description:"))
                {
                    description = line.Split(':')[1].Trim();
                }
                else if (line.StartsWith("-- Version:"))
                {
                    version = line.Split(':')[1].Trim().ToLower();
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    switch(version)
                    {
                        case "error":
                            redundancyQuery += line + "\n";
                            break;
                        case "correct":
                            correctQuery += line + "\n";
                            break;
                        case "sqlserver error":
                            sqlserverRedundancyQuery += line + "\n";
                            break;
                        case "sqlserver correct":
                            sqlserverCorrectQuery += line + "\n";
                            break;
                        case "oracle error":
                            oracleRedundancyQuery += line + "\n";
                            break;
                        case "oracle correct":
                            oracleCorrectQuery += line + "\n";
                            break;
                        case "postgre error":
                            postgreRedundancyQuery += line + "\n";
                            break;
                        case "postgre correct":
                            postgreCorrectQuery += line + "\n";
                            break;
                        case "mysql error":
                            mysqlRedundancyQuery += line + "\n";
                            break;
                        case "mysql correct":
                            mysqlCorrectQuery += line + "\n";
                            break;
                    }
                }
            }
        }
    }
}
