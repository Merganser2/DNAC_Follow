USE DotNetCourseDatabase
GO

-- Taking our query from before, and adding a Clustered Index
-- to make it faster. The UserSalary table will now be physically
-- ordered by UserId such that SQL knows exactly where to find
-- any row with a given id.
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

--CREATE CLUSTERED INDEX cix_UserSalary_UserId ON TutorialAppSchema.UserSalary(UserId)

-- Non-clustered: will tell us the page...(also includes the cluster)
CREATE INDEX ix_UserSalary_Salary ON TutorialAppSchema.UserSalary(Salary)
-- or
CREATE NONCLUSTERED INDEX ix_UserJobInfo_JobTitle ON TutorialAppSchema.UserJobInfo(JobTitle)
       INCLUDE (Department)
