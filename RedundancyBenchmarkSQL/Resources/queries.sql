----------------------------------------
-- Category: Attributes
----------------------------------------
-- Source: Brass and Goldberg
-- Reference: 2. Unnecessary complications (1)
-- Description: We already know the attribute country, there is no need to have it in the SELECT.
-- Version: error
SELECT FirstName, LastName, Country
FROM Customer
WHERE Country = 'Czech Republic';

-- Version: correct
SELECT FirstName, LastName, 'Czech Republic'
FROM Customer
WHERE Country = 'Czech Republic';

----------------------------------------
-- Category: Distinct
----------------------------------------
-- Source: Brass and Goldberg
-- Reference: 2.2. Error 2
-- Description: Using distinct on already unique values.
-- Version: error
SELECT DISTINCT(MediaTypeId)
FROM MediaType;

-- Version: correct
SELECT MediaTypeId
FROM MediaType;

----------------------------------------
-- Category: Conditions
----------------------------------------
-- Source: Brass and Goldberg
-- Reference: 2.4. Error 8
-- Description: InvoiceId is unique for each invoice therefore BillingCountry condition is unnecessary and redundant.
-- Version: error
SELECT *
FROM Invoice
WHERE InvoiceId = 1 AND BillingCountry = 'Germany';

-- Version: correct
SELECT *
FROM Invoice
WHERE InvoiceId = 1;

-- Source: Brass and Goldberg
-- Reference: 2.4. Error 8
-- Description: Duplicated (redundant) conditions with OR operator.
-- Version: error
SELECT *
FROM Customer
WHERE Country = 'USA' OR Country = 'USA';

-- Version: correct
SELECT *
FROM Customer
WHERE Country = 'USA';

-- Source: Brass and Goldberg
-- Reference: 2.4. Error 8
-- Description: Duplicated (redundant) conditions with AND operator.
-- Version: error
SELECT *
FROM Invoice
WHERE BillingCity = 'Oslo' AND BillingCity = 'Oslo';

-- Version: correct
SELECT *
FROM Invoice
WHERE BillingCity = 'Oslo';

-- Source: Brass and Goldberg
-- Reference: 1. Introduction
-- Description: Mutually exclusive conditions.
-- Version: error
SELECT *
FROM Album
WHERE Title = 'Fireball' AND Title = 'Outbreak';

-- Version: correct
SELECT *
FROM Album
WHERE Title = 'Impossible title';

-- Source: Brass and Goldberg
-- Reference: 2.4. Error 8
-- Description: Mutually exclusive conditions.
-- Version: error
SELECT Name
FROM Track
WHERE UnitPrice < 1 AND
      UnitPrice > 1.5;

-- Version: correct
SELECT Name
FROM Track
WHERE UnitPrice < -5;

-- Source: Brass and Goldberg
-- Reference: 2.4. Error 8
-- Description: Using unnecessary conditions that were already fulfilled by another condition.
-- Version: error
SELECT *
FROM Track
WHERE Milliseconds > 300000 AND
      Milliseconds > 200000;

-- Version: correct
SELECT *
FROM Track
WHERE Milliseconds > 300000;

-- Source: Brass and Goldberg
-- Reference: 2.4. Error 8
-- Description: Redundant values in the IN list.
-- Version: error
SELECT *
FROM Track
WHERE GenreId IN (1, 1, 2, 3, 3);

-- Version: correct
SELECT *
FROM Track
WHERE GenreId IN (1, 2, 3);

-- Description: Conditioning attribute to be from any of its unique values.
-- Version: error
SELECT *
FROM InvoiceLine
WHERE TrackId IN (SELECT TrackId FROM InvoiceLine);

-- Version: correct
SELECT *
FROM InvoiceLine;

-- Description: Conditioning attribute to be from any of its unique values - all employees are from Canada.
-- Version: error
SELECT *
FROM Employee
WHERE Country = 'Canada';

-- Version: correct
SELECT *
FROM Employee;

-- Description: Unnecessary condition, comparing name to "anything".
-- Version: error
SELECT TrackId, Name
FROM Track
WHERE Name LIKE '%';

-- Version: correct
SELECT TrackId, Name
FROM Track;

-- Description: Using LIKE without wildcards.
-- Version: error
SELECT FirstName, LastName
FROM Customer
WHERE Country LIKE 'USA';

-- Version: correct
SELECT FirstName, LastName
FROM Customer
WHERE Country = 'USA';

-- Description: Unnecessary IN/EXISTS condition that could be replaced by simple comparison.
-- Version: error
SELECT *
FROM Invoice
WHERE BillingCountry NOT IN
(SELECT BillingCountry
 FROM Invoice
 WHERE BillingCountry = 'Germany');

-- Version: correct
SELECT *
FROM Invoice
WHERE BillingCountry != 'Germany';

----------------------------------------
-- Category: Joins
----------------------------------------
-- Source: Brass and Goldberg
-- Reference: 2.3. Error 6
-- Description: Using unnecessary JOIN if we only use attributes from one table.
-- Version: error
SELECT Invoice.InvoiceId, Invoice.Total
FROM Invoice
JOIN Customer ON Invoice.CustomerId = Customer.CustomerId
WHERE Invoice.Total < 1;

-- Version: correct
SELECT Invoice.InvoiceId, Invoice.Total
FROM Invoice
WHERE Invoice.Total < 1;

-- Description: JOINing table on itself that just duplicates output columns.
-- Version: error
SELECT *
FROM Genre as g1
JOIN Genre as g2 ON g1.GenreId=g2.GenreId;

-- Version: correct
SELECT GenreId, Name, GenreId, Name
FROM Genre;

-- Description: Unnecessary UNION.
-- Version: error
SELECT *
FROM Genre
UNION
SELECT *
FROM Genre;

-- Version: correct
SELECT *
FROM Genre;

-- Description: Using outer join that can be replaced by inner join. All tuples generated by the outer join are eliminated by the WHERE-condition.
-- Version: error
SELECT t.TrackId, t.Name, il.InvoiceLineId
FROM Track t LEFT OUTER JOIN InvoiceLine il ON t.TrackId = il.TrackId
WHERE il.InvoiceLineId IS NOT NULL;

-- Version: correct
SELECT t.TrackId, t.Name, il.InvoiceLineId
FROM Track t 
INNER JOIN InvoiceLine il ON t.TrackId = il.TrackId;

----------------------------------------
-- Category: Aggregations
----------------------------------------
-- Description: Applying COUNT() to a column that is unique.
-- Version: error
SELECT Count(TrackId)
FROM Track;

-- Version: correct
SELECT Count(*)
FROM Track;

-- Description: Applying COUNT() to a column that is unique.
-- Version: error
SELECT Count(Name)
FROM Track;

-- Version: correct
SELECT Count(*)
FROM Track;

-- Description: Unnecessary single distinct input value aggregations that could be replaced by SELECT DISTINCT.
-- Version: error
SELECT MAX(UnitPrice)
FROM Track
GROUP BY UnitPrice;

-- Version: correct
SELECT DISTINCT(UnitPrice)
FROM Track;

-- Description: Unnecessary DISTINCT in MIN / MAX aggregations.
-- Version: error
SELECT MAX(DISTINCT Milliseconds)
FROM Track;

-- Version: correct
SELECT MAX(Milliseconds)
FROM Track;

----------------------------------------
-- Category: Grouping
----------------------------------------
-- Description: Using group by on id is unnecessary and redundant because there is always only one tuple per id.
-- Version: error
SELECT ArtistId, COUNT(*)
FROM Artist
GROUP BY ArtistId;

-- Version: correct
SELECT ArtistId, '1'
FROM Artist;

---------------- Having ----------------
----------------------------------------
-- Description: Doing the join condition under HAVING instead of in WHERE is possible, but has awful performance.
-- Version: error
SELECT Invoice.BillingCity, COUNT(*)
FROM Invoice, InvoiceLine
GROUP BY Invoice.BillingCity, Invoice.InvoiceId
HAVING Invoice.InvoiceId = InvoiceLine.InvoiceId;

/*
Semantic errors that can cause redundant parts in the SQL query execution plan:

Joins:
Unnecessary join operations in the query (joining tables when they are not needed).

Subqueries:
Using subqueries that do not contribute to the final result or are unnecessary.

Conditions:
Including conditions in the WHERE clause that are redundant or do not affect the query result.

Grouping:
Grouping by attributes that are not necessary for the query result, for example by ID.

Aggregations:
Unnecessary aggregations.
*/
