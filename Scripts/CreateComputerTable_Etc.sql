-- CREATE DATABASE DotNetCourseDatabase
-- GO
 
USE DotNetCourseDatabase
GO

-- But here, same Syntax IS assignment ??
UPDATE TutorialAppSchema.Computer SET CPUCores = 16 WHERE ReleaseDate > '2021-12-09'

-- in ADS, Ctrl-Space, with cursor in front of *,will give fields...
SELECT [ComputerId],
[Motherboard],
-- IsNull([CPUCores], 8) AS 'CPU Cores',
[CPUCores],
[HasWifi],
[HasLTE],
[ReleaseDate],
[Price],
[VideoCard] FROM TutorialAppSchema.Computer
ORDER BY ReleaseDate DESC
GO

-- Since many still null can use ISNULL function to set a value for any ISNULL found,
-- even in SELECT statement ?? or more likely just displays them as such,
-- and so will get a column without a name, unless named

-- If needed to insert different value into PK ComputerId, could do it with
-- following command (be sure to turn OFF when finished) - this could allow
-- you to make the next entry have a different starting/seed value
-- (I assume would prevent you from entering a duplicate ???)
-- SET IDENTITY_INSERT TutorialAppSchema.Computer ON
--  would allow you to set the PK to a higher value than the next natural value in Table

INSERT INTO TutorialAppSchema.Computer
(
[Motherboard],
[CPUCores],
[HasWifi],
[HasLTE],
[ReleaseDate],
[Price],
[VideoCard]
) VALUES (
    'FakeMotherboardName',74,1,1,'04-24-2024',99999.5640,'SomethingCrazy'
)
GO

-- NOTICE the 'is equal' syntax, looks like assignment in programming languages
-- DELETE FROM TutorialAppSchema.Computer WHERE ComputerId = 101

-- TRUNCATE TABLE TutorialAppSchema.Computer -- Or...would still need to 'USE' first?
--  TRUNCATE TABLE DotNetCourseDatabase.TutorialAppSchema.Computer

 
/* CREATE SCHEMA TutorialAppSchema
   GO
 
   CREATE TABLE TutorialAppSchema.Computer(
    ComputerId INT IDENTITY(1,1) PRIMARY KEY,
    Motherboard NVARCHAR(50),
    CPUCores INT,
    HasWifi BIT,
    HasLTE BIT,
    ReleaseDate DATE,
    Price DECIMAL(18,4),
    VideoCard NVARCHAR(50) -- normally 255 is best practice if don't know
); */

