﻿----------------------------------------
-- Category: Attributes
----------------------------------------
-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2. Unnecessary complications (1)
-- Description: We already know the attribute country, there is no need to have it in the SELECT.
-- Version: redundancy
SELECT FirstName, LastName, Country
FROM Customer
WHERE Country = 'Czech Republic'

-- Version: correct
SELECT FirstName, LastName, 'Czech Republic' AS Country
FROM Customer
WHERE Country = 'Czech Republic'
-- end

----------------------------------------
-- Category: Distinct
----------------------------------------
-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.2. Error 2
-- Description: Using distinct on already unique values.
-- Version: redundancy
SELECT DISTINCT(AlbumId)
FROM Album

-- Version: correct
SELECT AlbumId
FROM Album
-- end

----------------------------------------
-- Category: Conditions
----------------------------------------
-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.4. Error 8
-- Description: Duplicated (redundant) conditions with OR operator.
-- Version: redundancy
SELECT *
FROM Customer
WHERE Country = 'USA' OR Country = 'USA'

-- Version: correct
SELECT *
FROM Customer
WHERE Country = 'USA'
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.4. Error 8
-- Description: Duplicated (redundant) conditions with AND operator.
-- Version: redundancy
SELECT *
FROM Customer
WHERE Country = 'USA' AND Country = 'USA'

-- Version: correct
SELECT *
FROM Customer
WHERE Country = 'USA'
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 1. Introduction
-- Description: Mutually exclusive conditions.
-- Version: redundancy
SELECT *
FROM Album
WHERE Title = 'Fireball' AND Title = 'Outbreak'

-- Version: correct
SELECT *
FROM Album
WHERE 1 = 0
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.4. Error 8
-- Description: Mutually exclusive conditions.
-- Version: redundancy
SELECT Name
FROM Track
WHERE UnitPrice < 1 AND
      UnitPrice > 1.5

-- Version: correct
SELECT Name
FROM Track
WHERE 1 = 0
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.4. Error 8
-- Description: Using unnecessary conditions that were already fulfilled by another condition.
-- Version: redundancy
SELECT *
FROM Track
WHERE UnitPrice > 0.5 AND
      UnitPrice > 1

-- Version: correct
SELECT *
FROM Track
WHERE UnitPrice > 1
-- end

-- Description: Duplicate values in the IN list.
-- Version: redundancy
SELECT *
FROM Track
WHERE GenreId IN (1, 1, 2, 3, 3)

-- Version: correct
SELECT *
FROM Track
WHERE GenreId IN (1, 2, 3)
-- end

-- Description: Conditioning attribute to be from any of its unique values.
-- Version: redundancy
SELECT *
FROM InvoiceLine
WHERE TrackId IN (SELECT TrackId FROM InvoiceLine)

-- Version: correct
SELECT *
FROM InvoiceLine
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.4 Error 11
-- Description: Unnecessary condition, comparing name to "anything".
-- Version: redundancy
SELECT TrackId, Name
FROM Track
WHERE Name LIKE '%'

-- Version: correct
SELECT TrackId, Name
FROM Track
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.4 Error 12, 4 Error 34
-- Description: Using LIKE without wildcards.
-- Version: redundancy
SELECT FirstName, LastName
FROM Customer
WHERE Country LIKE 'USA'

-- Version: correct
SELECT FirstName, LastName
FROM Customer
WHERE Country = 'USA'
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.4 Error 14
-- Description: Unnecessary IN/EXISTS condition that could be replaced by simple comparison.
-- Version: redundancy
SELECT *
FROM Customer
WHERE Country NOT IN
(SELECT Country
 FROM Customer
 WHERE Country = 'USA')

-- Version: correct
SELECT *
FROM Customer
WHERE Country != 'USA'
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.4 Error 13
-- Description: Unnecessarily complicated SELECT in EXISTS-subquery. (using unnecessarily DISTINCT)
-- Filter: true
-- Version: redundancy
SELECT *
FROM Album
WHERE EXISTS (
    SELECT DISTINCT TrackId
    FROM Track
    WHERE Album.AlbumId = Track.AlbumId
)

-- Version: correct
SELECT *
FROM Album
WHERE EXISTS (
    SELECT TrackId
    FROM Track
    WHERE Album.AlbumId = Track.AlbumId
)
-- end

-- Description: Unnecessarily complicated JOIN with DISTINCT that can be replaced by simpler EXISTS.
-- Version: redundancy
SELECT DISTINCT Album.AlbumId, Album.Title
FROM Album
JOIN Track ON Album.AlbumId = Track.AlbumId
JOIN Genre ON Track.GenreId = Genre.GenreId
WHERE Genre.Name = 'Pop'

-- Version: correct
SELECT Album.AlbumId, Album.Title
FROM Album
WHERE EXISTS (
    SELECT 1
    FROM Track
    JOIN Genre ON Track.GenreId = Genre.GenreId
    WHERE Track.AlbumId = Album.AlbumId AND Genre.Name = 'Pop'
)
-- end


----------------------------------------
-- Category: Joins and Unions
----------------------------------------
-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.3. Error 6
-- Description: Using unnecessary JOIN if we only use attributes from one table.
-- Version: redundancy
SELECT Invoice.InvoiceId, Invoice.Total
FROM Invoice
JOIN Customer ON Invoice.CustomerId = Customer.CustomerId
WHERE Invoice.Total < 1

-- Version: correct
SELECT Invoice.InvoiceId, Invoice.Total
FROM Invoice
WHERE Invoice.Total < 1
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.2 Error 4
-- Description: JOINing table on itself that just duplicates output columns.
-- Version: redundancy
SELECT *
FROM Genre as g1
JOIN Genre as g2 ON g1.GenreId=g2.GenreId

-- Version: correct
SELECT GenreId, Name, GenreId, Name
FROM Genre

-- Version: oracle redundancy
SELECT *
FROM Genre g1
JOIN Genre g2 ON g1.GenreId=g2.GenreId

-- Version: oracle correct
SELECT GenreId, Name, GenreId, Name
FROM Genre
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.8 Error 23
-- Description: Unnecessary UNION.
-- Version: redundancy
SELECT TrackId, Name
FROM Track
WHERE GenreId = (SELECT GenreId FROM Genre WHERE Name = 'Rock')

UNION

SELECT TrackId, Name
FROM Track
WHERE GenreId = (SELECT GenreId FROM Genre WHERE Name = 'Jazz')

-- Version: correct
SELECT TrackId, Name
FROM Track
WHERE GenreId = (SELECT GenreId FROM Genre WHERE Name = 'Rock')
   OR GenreId = (SELECT GenreId FROM Genre WHERE Name = 'Jazz')
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 3 Error 26
-- Description: UNION that could be replaced by UNION ALL.
-- Version: redundancy
SELECT TrackId, Name
FROM Track
WHERE GenreId = (SELECT GenreId FROM Genre WHERE Name = 'Classical')

UNION

SELECT TrackId, Name
FROM Track
WHERE GenreId = (SELECT GenreId FROM Genre WHERE Name = 'Hip Hop/Rap')

-- Version: correct
SELECT TrackId, Name
FROM Track
WHERE GenreId = (SELECT GenreId FROM Genre WHERE Name = 'Classical')

UNION ALL

SELECT TrackId, Name
FROM Track
WHERE GenreId = (SELECT GenreId FROM Genre WHERE Name = 'Hip Hop/Rap')
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 4 Error 35, 36
-- Description: Using outer join that can be replaced by inner join. All tuples generated by the outer join are eliminated by the WHERE-condition.
-- Version: redundancy
SELECT t.TrackId, t.Name, il.InvoiceLineId
FROM Track t LEFT OUTER JOIN InvoiceLine il ON t.TrackId = il.TrackId
WHERE il.InvoiceLineId IS NOT NULL

-- Version: correct
SELECT t.TrackId, t.Name, il.InvoiceLineId
FROM Track t 
INNER JOIN InvoiceLine il ON t.TrackId = il.TrackId
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 4 Error 35
-- Description: Condition on left table in left outer join that excludes all possible join partners.
-- Version: redundancy
SELECT t.TrackId, t.Name, il.InvoiceLineId
FROM Track t
LEFT OUTER JOIN InvoiceLine il ON t.TrackId = il.TrackId
WHERE il.InvoiceLineId = 52

-- Version: correct
SELECT t.TrackId, t.Name, il.InvoiceLineId
FROM Track t
INNER JOIN InvoiceLine il ON t.TrackId = il.TrackId
WHERE il.InvoiceLineId = 52
-- end

----------------------------------------
-- Category: Aggregations
----------------------------------------
-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.5 Error 17
-- Description: Applying COUNT() to a column that is unique.
-- Version: redundancy
SELECT Count(TrackId)
FROM Track

-- Version: correct
SELECT Count(*)
FROM Track
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.5 Error 15, 2.6 Error 22
-- Description: Unnecessary single distinct input value aggregations that could be replaced by SELECT DISTINCT.
-- Filter: true
-- Version: redundancy
SELECT MAX(UnitPrice)
FROM Track
GROUP BY UnitPrice

-- Version: correct
SELECT DISTINCT(UnitPrice)
FROM Track
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.5 Error 16
-- Description: Unnecessary DISTINCT in MIN / MAX aggregations.
-- Version: redundancy
SELECT MAX(DISTINCT UnitPrice)
FROM Track

-- Version: correct
SELECT MAX(UnitPrice)
FROM Track
-- end

-- Source: null
-- Reference: null
-- Description: Unnecessary replacing GROUP BY in MIN / MAX aggregations with subquery.
-- Version: redundancy
SELECT DISTINCT i1.CustomerId, (
SELECT SUM(i2.Total)
FROM Invoice AS i2
WHERE i1.CustomerId = i2.CustomerId
)
FROM Invoice AS i1

-- Version: correct
SELECT CustomerId, SUM(Total)
FROM Invoice
GROUP BY CustomerId

-- Version: oracle redundancy
SELECT DISTINCT i1.CustomerId, (
SELECT SUM(i2.Total)
FROM Invoice i2
WHERE i1.CustomerId = i2.CustomerId
)
FROM Invoice i1

-- Version: oracle correct
SELECT CustomerId, SUM(Total)
FROM Invoice
GROUP BY CustomerId
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.6 Error 19
-- Description: Using aggregation in subquery on id is unnecessary and redundant because there is always only one tuple per id.
-- Version: redundancy
SELECT Invoice.InvoiceId, (
SELECT COUNT(*)
FROM Customer
WHERE Customer.CustomerId = Invoice.CustomerId
)
FROM Invoice

-- Version: correct
SELECT Invoice.InvoiceId, 1
FROM Invoice
-- end

----------------------------------------
-- Category: Grouping
----------------------------------------
-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.6 Error 19
-- Description: Using aggregation with group by on id is unnecessary and redundant because there is always only one tuple per id.
-- Filter: true
-- Version: redundancy
SELECT ArtistId, COUNT(*)
FROM Artist
GROUP BY ArtistId

-- Version: correct
SELECT ArtistId, '1'
FROM Artist
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.6 Error 19
-- Description: Using aggregation with group by on ids is unnecessary and redundant because there is always only one tuple per id.
-- Filter: true
-- Version: redundancy
SELECT ArtistId, AlbumId, COUNT(*) AS Count
FROM Album
GROUP BY ArtistId, AlbumId

-- Version: correct
SELECT ArtistId, AlbumId, '1' AS Count
FROM Album
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.6 Error 18
-- Description: Unnecessary GROUP BY in EXISTS subquery.
-- Version: redundancy
SELECT *
FROM Album
WHERE EXISTS (
    SELECT 1
    FROM Track
    WHERE TrackId = Track.TrackId AND Album.Title = 'Outbreak'
    GROUP BY TrackId
)

-- Version: correct
SELECT *
FROM Album
WHERE EXISTS (
    SELECT 1
    FROM Track
    WHERE TrackId = Track.TrackId AND Album.Title = 'Outbreak'
)
-- end


----------------------------------------
-- Category: Case
----------------------------------------
-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.6 Error 19
-- Description: Using aggregation with group by on id is unnecessary and redundant because there is always only one tuple per id.
-- Version: redundancy
SELECT TrackId,
CASE
    WHEN Count(Name) = 1 THEN 'Yes' ELSE 'No'
END AS IsUnique
FROM Track
GROUP BY TrackId

-- Version: correct
SELECT TrackId, 'Yes' AS IsUnique
FROM Track
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.4 Error 12, 4 Error 34
-- Description: Using LIKE without wildcards.
-- Version: redundancy
SELECT FirstName, LastName,
CASE
    WHEN Country LIKE 'USA' THEN 'Yes' ELSE 'No'
END AS IsFromUSA
FROM Customer

-- Version: correct
SELECT FirstName, LastName,
CASE
WHEN Country = 'USA' THEN 'Yes' ELSE 'No'
END AS IsFromUSA
FROM Customer
-- end

-- Description: Conditioning attribute to be from any of its unique values.
-- Version: redundancy
SELECT Name, TrackId, CASE
    WHEN TrackId IN (SELECT TrackId FROM Track) THEN 'Yes' ELSE 'No'
END
FROM Track

-- Version: correct
SELECT Name, TrackId, 'Yes'
FROM Track
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.4. Error 8
-- Description: Duplicated (redundant) conditions with OR operator.
-- Version: redundancy
SELECT FirstName, LastName,
CASE
    WHEN Country = 'USA' OR Country = 'USA' THEN 'Yes' ELSE 'No'
END AS IsFromUSA
FROM Customer

-- Version: correct
SELECT FirstName, LastName,
CASE
    WHEN Country = 'USA' THEN 'Yes' ELSE 'No'
END AS IsFromUSA
FROM Customer
-- end

----------------------------------------
-- Category: Window
----------------------------------------
-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.6 Error 19
-- Description: Using aggregation with group by on id is unnecessary and redundant because there is always only one tuple per id.
-- Version: redundancy
SELECT ArtistId, COUNT(*) OVER (PARTITION BY ArtistId) AS Count
FROM Artist

-- Version: correct
SELECT ArtistId, '1'
FROM Artist
-- end

-- Source: Semantic errors in SQL queries: A quite complete list.
-- Reference: 2.6 Error 19
-- Description: Using aggregation with group by on ids is unnecessary and redundant because there is always only one tuple per id.
-- Version: redundancy
SELECT ArtistId, AlbumId, COUNT(*) OVER (PARTITION BY ArtistId, AlbumId) AS Count
FROM Album

-- Version: correct
SELECT ArtistId, AlbumId, '1' AS Count
FROM Album
-- end
