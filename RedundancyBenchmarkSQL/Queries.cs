using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace RedundancyBenchmarkSQL
{
    internal class Queries
    {
        public List<Query> QueryList { get; set; }

        public Queries()
        {
            QueryList = new List<Query>();
        }

        public void AddQuery(Query query)
        {
            QueryList.Add(query);
        }

        public Query GetQuery(int index)
        {
            if (index >= 0 && index < QueryList.Count)
            {
                return QueryList[index];
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }
        }

        public int Count()
        {
            return QueryList.Count;
        }

        public void GenerateExcel(string name, bool filterQueries)
        {
            // Set the license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(name);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Queries");

                // Add headers
                worksheet.Cells[1, 1].Value = "Description";
                worksheet.Cells[1, 2].Value = "SqlServer";
                worksheet.Cells[1, 3].Value = "Oracle";
                worksheet.Cells[1, 4].Value = "MySql";
                worksheet.Cells[1, 5].Value = "PostgreSql";

                // Add query data
                for (int i = 0; i < QueryList.Count; i++)
                {
                    var query = QueryList[i];
                    worksheet.Cells[i + 2, 1].Value = query.Description;
                    worksheet.Cells[i + 2, 2].Value = query.SqlServerComparison ? 1 : 0;
                    worksheet.Cells[i + 2, 3].Value = query.OracleComparison ? 1 : 0;
                    worksheet.Cells[i + 2, 4].Value = query.MySqlComparison ? 1 : 0;
                    worksheet.Cells[i + 2, 5].Value = query.PostgreSqlComparison ? 1 : 0;
                }

                // Add total counts for each database provider
                worksheet.Cells[QueryList.Count + 2, 1].Value = "Total Count";
                worksheet.Cells[QueryList.Count + 2, 2].Formula = $"=SUM(B2:B{QueryList.Count + 1})";
                worksheet.Cells[QueryList.Count + 2, 3].Formula = $"=SUM(C2:C{QueryList.Count + 1})";
                worksheet.Cells[QueryList.Count + 2, 4].Formula = $"=SUM(D2:D{QueryList.Count + 1})";
                worksheet.Cells[QueryList.Count + 2, 5].Formula = $"=SUM(E2:E{QueryList.Count + 1})";

                // Add headers
                worksheet.Cells[1, 8].Value = "Category";
                worksheet.Cells[1, 9].Value = "Total Queries";
                worksheet.Cells[1, 10].Value = "SqlServer";
                worksheet.Cells[1, 11].Value = "Oracle";
                worksheet.Cells[1, 12].Value = "MySql";
                worksheet.Cells[1, 13].Value = "PostgreSql";

                var categories = QueryList.Select(q => q.Category).Distinct().ToList();

                var totalCounts = categories.Select(cat => QueryList.Count(q => q.Category == cat)).ToList();

                if (filterQueries)
                    totalCounts = categories.Select(cat => QueryList.Count(q => q.Category == cat && (!q.Filter))).ToList();

                var sqlServerCounts = categories.Select(cat => QueryList.Count(q => q.Category == cat && q.SqlServerComparison)).ToList();
                var oracleCounts = categories.Select(cat => QueryList.Count(q => q.Category == cat && q.OracleComparison)).ToList();
                var mySqlCounts = categories.Select(cat => QueryList.Count(q => q.Category == cat && q.MySqlComparison)).ToList();
                var postgreSqlCounts = categories.Select(cat => QueryList.Count(q => q.Category == cat && q.PostgreSqlComparison)).ToList();

                // Add query data
                for (int i = 0; i < categories.Count; i++)
                {
                    worksheet.Cells[i + 2, 8].Value = categories[i];
                    worksheet.Cells[i + 2, 9].Value = totalCounts[i];
                    worksheet.Cells[i + 2, 10].Value = sqlServerCounts[i];
                    worksheet.Cells[i + 2, 11].Value = oracleCounts[i];
                    worksheet.Cells[i + 2, 12].Value = mySqlCounts[i];
                    worksheet.Cells[i + 2, 13].Value = postgreSqlCounts[i];
                }

                // Add total counts for each database provider
                worksheet.Cells[categories.Count + 2, 8].Value = "Total Count";
                worksheet.Cells[categories.Count + 2, 9].Formula = $"=SUM(I2:I{categories.Count + 1})";
                worksheet.Cells[categories.Count + 2, 10].Formula = $"=SUM(J2:J{categories.Count + 1})";
                worksheet.Cells[categories.Count + 2, 11].Formula = $"=SUM(K2:K{categories.Count + 1})";
                worksheet.Cells[categories.Count + 2, 12].Formula = $"=SUM(L2:L{categories.Count + 1})";
                worksheet.Cells[categories.Count + 2, 13].Formula = $"=SUM(M2:M{categories.Count + 1})";

                // Save the Excel package to a file
                var fileInfo = new FileInfo(name);
                package.SaveAs(fileInfo);
            }
        }

        public void PrintQueries(bool filterQueries)
        {
            foreach (var query in QueryList)
            {
                if (filterQueries && query.Filter)
                    continue;
                query.Print();
                Console.WriteLine("----------------------------------------------------------------------------------------------------");
            }
        }

        public void ResetQueriesResults()
        {
            for (int i = 0; i < QueryList.Count(); i++)
            {
                QueryList[i].ResetQueryResult();
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

            bool filterQuery = false;

            foreach (string line in lines)
            {
                if (line.StartsWith("-- end"))
                {
                    Query currentQuery = new Query(category, description, correctQuery, redundancyQuery, filterQuery);

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

                    filterQuery = false;
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
                else if (line.StartsWith("-- Filter: true"))
                {
                    filterQuery = true;
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    switch(version)
                    {
                        case "redundancy":
                            redundancyQuery += line + "\n";
                            break;
                        case "correct":
                            correctQuery += line + "\n";
                            break;
                        case "sqlserver redundancy":
                            sqlserverRedundancyQuery += line + "\n";
                            break;
                        case "sqlserver correct":
                            sqlserverCorrectQuery += line + "\n";
                            break;
                        case "oracle redundancy":
                            oracleRedundancyQuery += line + "\n";
                            break;
                        case "oracle correct":
                            oracleCorrectQuery += line + "\n";
                            break;
                        case "postgre redundancy":
                            postgreRedundancyQuery += line + "\n";
                            break;
                        case "postgre correct":
                            postgreCorrectQuery += line + "\n";
                            break;
                        case "mysql redundancy":
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
