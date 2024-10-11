USE DotNetCourseDatabase
GO

SELECT [Email],
[PasswordHash],
[PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email IS NOT NULL

-- CREATE TABLE TutorialAppSchema.Auth (
--     Email NVARCHAR(50),
--     PasswordHash VARBINARY(MAX),
--     PasswordSalt VARBINARY(MAX)   
-- )