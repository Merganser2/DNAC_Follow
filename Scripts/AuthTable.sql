USE DotNetCourseDatabase
GO

SELECT [Email],
[PasswordHash],
[PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email IS NOT NULL

-- Delete it all
-- TRUNCATE TABLE TutorialAppSchema.Auth

-- CREATE TABLE TutorialAppSchema.Auth (
--     Email NVARCHAR(50),
--     PasswordHash VARBINARY(MAX),
--     PasswordSalt VARBINARY(MAX)   
-- )

-- Create a Posts Table

-- CREATE TABLE TutorialAppSchema.Posts (
--     PostId INT IDENTITY(1,1),
--     UserId INT,
--     PostTitle NVARCHAR(255),
--     PostContent NVARCHAR(MAX),
--     PostCreated DATETIME,
--     PostUpdated DATETIME
-- )

-- Insert rows in an order via clustered index
-- Mostly looking on single user's posts but want to include the identity field
-- Had we made PostId the PRIMARY KEY...
-- Sort by UserId first, then PostId
-- CREATE CLUSTERED INDEX cix_Posts_UserId_PostId ON TutorialAppSchema.Posts(UserId, PostId)

SELECT [PostId],
[UserId],
[PostTitle],
[PostContent],
[PostCreated],
[PostUpdated] FROM TutorialAppSchema.Posts

-- INSERT INTO TutorialAppSchema.Posts (
-- [UserId],
-- [PostTitle],
-- [PostContent],
-- [PostCreated],
-- [PostUpdated]
-- )
-- VALUES ()
