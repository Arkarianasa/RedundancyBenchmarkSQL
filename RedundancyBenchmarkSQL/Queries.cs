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
            string source = null;
            string reference = null;
            string description = "";
            string version = "";
            string redundancyQuery = "";
            string correctQuery = "";

            foreach (string line in lines)
            {
                if (line.StartsWith("-- Category:"))
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
                    if (version == "error")
                    {
                        redundancyQuery += line + "\n";
                    }
                    else if (version == "correct")
                    {
                        correctQuery += line + "\n";
                    }
                }
                else if (redundancyQuery != "" && correctQuery != "")
                {
                    Query currentQuery = new Query(category, description, correctQuery, redundancyQuery);
                    if (source != null && reference != null)
                    {
                        currentQuery.AddSourceAndReference(source, reference);
                    }
                    AddQuery(currentQuery);

                    // reset parameters for next query
                    currentQuery = null;
                    source = null;
                    reference = null;
                    description = "";
                    version = "";
                    redundancyQuery = "";
                    correctQuery = "";
                }
            }
        }
    }
}
