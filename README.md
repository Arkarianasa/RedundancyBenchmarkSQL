<h2>Benchmark verifying the ability to remove redundancy in SQL</h2>
<p>
  Many database systems automatically remove redundant parts of the query before executing the SQL query.
  This can be a redundant DISTINCT, an outer join instead of an inner join, or redundant entire constructs such as a join or GROUP BY.
  The goal of this work is to prepare a benchmark that will run SQL queries on predefined databases and check if any construct has been removed.
</p>

<h3>Development:</h3>
<p><a href="https://visualstudio.microsoft.com/cs/vs/community/">Visual Studio 2022 Community Edition</a></p>
<a href="https://visualstudio.microsoft.com/cs/vs/community/">.NET 7.0</a>

<h3>Installation:</h3>
<p>You need to install following NuGet packages via the NuGet Package Manager in Visual Studio:</p>
<ul>
  <li>Microsoft.Data.SqlClient</li>
  <li>Oracle.ManagedDataAccess.Core</li>
  <li>MySql.Data</li>
  <li>Npgsql</li>
</ul>

<h3>Supported Database Servers:</h3>
<ul>
  <li>SQL Server</li>
  <li>Oracle</li>
  <li>MySQL</li>
  <li>PostgreSQL</li>
</ul>

<h3>Used Database Model:</h3>
<p>This project uses chinook sample database.</p>
<p>Copyright (c) 2008-2024 Luis Rocha</p>
https://github.com/lerocha/chinook-database/tree/master/ChinookDatabase
