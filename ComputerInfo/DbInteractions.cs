using ComputerInfo.Data;
using ComputerInfo.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace ComputerInfo;

internal class DbInteractions
{
    private DataContextDapper _dapper = new DataContextDapper();

    internal void InsertComputerInfo()
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

        int result = _dapper.ExecuteSqlWithRowCount(insertSqlCmd);
        Console.WriteLine("*** Rows affected by insert: " + result + "***");
    }

    internal void GetAllComputersComputerInfo() {
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
            IEnumerable<Computer> computers = _dapper.GetRows<Computer>(sqlSelect);

            ShowComputerInfo(computers);
    }

    private static void ShowComputerInfo(IEnumerable<Computer> computers)
    {
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

    internal void TestDbConnectivity()
    {
        string testSqlConnectionCmd = "SELECT GETDATE()";

        // Dapper command that will return an array of results (rows)
        IEnumerable<DateTime> DateTimeIEnum = _dapper.GetRows<DateTime>(testSqlConnectionCmd);

        foreach (var date in DateTimeIEnum){
            Console.WriteLine(date);
        }

        DateTime currentTimeFromDatabase = _dapper.GetSingleRow<DateTime>(testSqlConnectionCmd);
        Console.WriteLine("Now trying single query");
        Console.WriteLine(currentTimeFromDatabase);        
    }
}