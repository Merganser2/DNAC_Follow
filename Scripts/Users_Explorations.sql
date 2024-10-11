-- Other tip, e.g., if expansion doesn't work, instead of having to restart:
-- Ctrl-Shift-p brings up command search, select "Refresh Intellisense Cache"

USE DotNetCourseDatabase
GO 

SELECT COUNT(*) FROM TutorialAppSchema.Users

SELECT * FROM TutorialAppSchema.Users

DELETE FROM TutorialAppSchema.Users
WHERE UserId = 1004

-- Click after the *, then Ctrl-Space to expand (restarted ADS to get it to work)
-- SELECT [UserId],[FirstName],[LastName],[Email],[Gender],
-- [Active] FROM DotNetCourseDatabase.TutorialAppSchema.Users -- AS Users

-- Or with an alias, like 'Users', will precede with [alias]
-- Click after the *, then Ctrl-Space to expand (restarted ADS to get it to work)
-- Purpose of this is to fully qualify a field with the Table from which it came, to disambiguate
SELECT [Users].[UserId],
[Users].[FirstName] + ' ' + [Users].[LastName] AS FullName,
[Users].[Email],
[Users].[Gender],
UserJobInfo.[JobTitle],
UserSalary.Salary,
[Users].[Active]
FROM DotNetCourseDatabase.TutorialAppSchema.Users AS Users
    INNER JOIN TutorialAppSchema.UserSalary AS UserSalary -- JOIN is INNER by default
        ON UserSalary.UserId = Users.UserId
    LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
        ON Users.UserId = UserJobInfo.UserId -- allows me to do it even though not an alias 
WHERE Users.[Active] = 1  -- Brackets around alias are optional, it would seem
ORDER BY Users.UserId DESC

-- 1000	Sherri Astlet	sastletrr@posterous.com	Male	Junior Executive	111265.5400	1
SELECT * FROM TutorialAppSchema.UserSalary WHERE UserId = 1000
SELECT * FROM TutorialAppSchema.UserJobInfo WHERE UserId = 1000
SELECT * FROM TutorialAppSchema.UserJobInfo ORDER BY UserId DESC
-- DELETE FROM TutorialAppSchema.UserSalary WHERE UserId = 1000
-- DELETE FROM TutorialAppSchema.UserSalary WHERE (Salary = 111265.5400) -- ((Salary = 111265.5400) AND (UserId = NULL))
SELECT * FROM TutorialAppSchema.UserSalary ORDER BY UserId DESC
--INSERT INTO TutorialAppSchema.UserSalary([UserId],[Salary]) VALUES (1000,111265.5400) 
   -- NOTE on above: didn't have to turn IDENTITY_INSERT on because UserId is not a PK on UserSalary

-- WHERE EXISTS can be used to do the same thing as JOIN, but is faster,
--   because it doesn't have to pull in as many rows
SELECT [Users].[UserId],
[Users].[FirstName] + ' ' + [Users].[LastName] AS FullName,
UserSalary.Salary, -- I guess downside is can't show other table names being "joined"* 
                   -- Unless I add to FROM clause...does this negate speed advantage?
[Users].[Active]
FROM DotNetCourseDatabase.TutorialAppSchema.Users AS Users, TutorialAppSchema.UserSalary AS UserSalary 
WHERE EXISTS (
    SELECT * FROM TutorialAppSchema.UserSalary AS UserSalary
        WHERE UserSalary.UserId = Users.UserId
    )

/* UNION gets non-duplicates *between* Tables; i.e., if there are duplicates in one
  of the tables being joined, these will still show up. But if two Tables contain the
  same data, the UNION will just look like SELECT of either Table

  Below is example of when there might be a need to gather the same table from two
  different servers/data sources (or even queries done at different times),
  making sure to get all data in either:
*/
SELECT UserId, Salary FROM TutorialAppSchema.UserSalary
UNION -- Distinct /* Between the two queries */
--UNION ALL -- not Distinct between the two
SELECT UserId, Salary FROM TutorialAppSchema.UserSalary;

-- Side note: BETWEEN is inclusive of endpoints
