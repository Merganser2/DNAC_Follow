using ComputerInfo.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace ComputerInfo;

internal static class DbRelated
{
    internal static void InsertComputerInfo()
    {
        Computer myComputer = new Computer()
        {
            Motherboard = "Z690",
            HasWifi = true,
            HasLTE = false,
            ReleaseDate = DateTime.Now,
            Price = 943.87m,
            VideoCard = "RTX 2060"
        };

        string insertSqlCmd = @"INSERT INTO TutorialAppSchema.Computer (
            Motherboard,
            HasWifi,
            HasLTE,
            ReleaseDate,
            Price,
            VideoCard        
        ) VALUES ('" + myComputer.Motherboard
                     + "','" + myComputer.HasWifi
                     + "','" + myComputer.HasLTE
                     + "','" + myComputer.ReleaseDate
                     + "','" + myComputer.Price
                     + "','" + myComputer.VideoCard
                     + "')";
        Console.WriteLine(insertSqlCmd);

        string ConnectionString = "Server=localhost;Database=DotNetCourseDatabase;TrustServerCertificate=true;Trusted_Connection=true";
        IDbConnection dbConnection = new SqlConnection(ConnectionString);

        int result = dbConnection.Execute(insertSqlCmd);
        Console.WriteLine("*** Rows affected by insert: " + result + "***");

    }

    internal static void GetComputerInfo() {
        string ConnectionString = "Server=localhost;Database=DotNetCourseDatabase;TrustServerCertificate=true;Trusted_Connection=true";
        IDbConnection dbConnection = new SqlConnection(ConnectionString);

            // TODO: When did we add field Computer.ComputerId, ??
            // Using best practice of fully qualifying fields with Table name
            string sqlSelect = @"
            SELECT 
                Computer.Motherboard,
                Computer.HasWifi,
                Computer.HasLTE,
                Computer.ReleaseDate,
                Computer.Price,
                Computer.VideoCard
             FROM TutorialAppSchema.Computer";

            // This is for when using DataContextDapper
            // IEnumerable<Computer> computers = dapper.LoadData<Computer>(sqlSelect);
            IEnumerable<Computer> computers = dbConnection.Query<Computer>(sqlSelect);

            Console.WriteLine("'ComputerId','Motherboard','HasWifi','HasLTE','ReleaseDate'" 
                + ",'Price','VideoCard'");
            foreach(Computer singleComputer in computers)
            {
                // Console.WriteLine("'" + singleComputer.ComputerId 
                Console.WriteLine("'" + singleComputer.Motherboard  
                    // + "','" + singleComputer.Motherboard
                    + "','" + singleComputer.HasWifi
                    + "','" + singleComputer.HasLTE
                    + "','" + singleComputer.ReleaseDate.ToString("yyyy-MM-dd")
                    + "','" + singleComputer.Price.ToString("0.00", CultureInfo.InvariantCulture) // In some regions decimal requires comma as separator
                    + "','" + singleComputer.VideoCard + "'");
            }
    }

    internal static void TestDbConnectivity()
    {
        string ConnectionString = "Server=localhost;Database=DotNetCourseDatabase;TrustServerCertificate=true;Trusted_Connection=true";

        IDbConnection dbConnection = new SqlConnection(ConnectionString);

        string testSqlConnectionCmd = "SELECT GETDATE()";

        // Dapper command that will return an array of results (rows)
        var DateTimeIEnum = dbConnection.Query<DateTime>(testSqlConnectionCmd);

        foreach (var date in DateTimeIEnum){
            Console.WriteLine(date);
        }

        DateTime currentTimeFromDatabase = dbConnection.QuerySingle<DateTime>(testSqlConnectionCmd);
        Console.WriteLine("Now trying single query");
        Console.WriteLine(currentTimeFromDatabase);
        
    }
}