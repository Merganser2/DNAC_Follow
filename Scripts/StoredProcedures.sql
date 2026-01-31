USE DotNetCourseDatabase
GO

-- NOTE: Above two lines are necessary to avoid an error,
--       as the Stored Procedure must be the first in the batch

-- Trying "storedProc_" as my prefix convention - cannot use "sp_" as that is convention in master db
-- CREATE PROCEDURE TutorialAppSchema.storedProc_UsersGet
ALTER PROCEDURE TutorialAppSchema.storedProc_UsersGet
AS 
BEGIN 
  -- Alias not required, but is a convention? But also
  -- tells Azure Data Studio that we want to alias our fields as well ? (it didn't already?)
  SELECT [Users].[UserId],
        [Users].[FirstName],
        [Users].[LastName],
        [Users].[Email],
        [Users].[Gender],
        [Users].[Active] 
  FROM TutorialAppSchema.Users AS Users
  -- WHY am I not seeing Results for this query ?????
END

-- NOTE: END above is convention, if something goes below here it will become
--       part of the Stored Procedure, if run as part of the CREATE or ALTER command (?)

/* EXEC TutorialAppSchema.storedProc_UsersGet 
Can uncomment this after having run CREATE PROCEDURE to run the stored procedure -
but careful not to leave uncommented when altering or will become part of the Stored Proc (?) */
