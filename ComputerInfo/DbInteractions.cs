using ComputerInfo.Data;
using ComputerInfo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace ComputerInfo;

internal class DbInteractions
{
    private IConfiguration _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                                      .Build();

    // Don't really need both Dapper and EF; this is more to just introduce their behaviors
    private readonly DataContextDapper _dapper; 
    private readonly DataContextEF _entityFramework;

    public DbInteractions()
    {
        _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                              .Build();
        _dapper = new DataContextDapper(_configuration);

        _entityFramework = new DataContextEF(_configuration);
    }

    internal void InsertComputerInfo()
    {
        Computer myComputer = new Computer()
        {
            Motherboard = "Z690",
            HasWifi = true,
            HasLTE = false,
            ReleaseDate = DateTime.Now,
            Price = 944.87m, // changed from 943.87m
            VideoCard = "RTX 2060"
        };

        // Doing same thing with EntityFramework in these two lines, that 
        //  we are doing with Dapper data context below.
        //  But we don't
        _entityFramework.Add(myComputer);
        int result = _entityFramework.SaveChanges();
        Console.WriteLine("*** Rows affected by insert according to EF: " + result + "***");

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
        //Console.WriteLine(insertSqlCmd);

        // Uncomment to do same thing with Dapper instead
        // result = _dapper.ExecuteSqlWithRowCount(insertSqlCmd);
        // Console.WriteLine("*** Rows affected by insert according to Dapper: " + result + "***");

        // Overwrites any existing file with contents passed in
        File.WriteAllText("log.txt", "\n" + insertSqlCmd + "\n");

        using StreamWriter openFile = new("log.txt", append: true);
        openFile.WriteLine(insertSqlCmd);
        openFile.Close();

        string textFromFile = File.ReadAllText("log.txt");
        Console.WriteLine(textFromFile);
    }

    internal void GetAllComputersComputerInfo()
    {
        // TODO: When did we add field Computer.ComputerId, ??
        // Using best practice of fully qualifying fields with Table name
        string sqlSelect = @"
            SELECT
                Computer.ComputerId, 
                Computer.Motherboard,
                Computer.HasWifi,
                Computer.HasLTE,
                Computer.ReleaseDate,
                Computer.Price,
                Computer.VideoCard
             FROM TutorialAppSchema.Computer";

        IEnumerable<Computer> computers = _dapper.GetRows<Computer>(sqlSelect);
        IEnumerable<Computer>? computersEF = _entityFramework.Computer?.ToList<Computer>();

        ShowComputerInfo(computers);
        ShowComputerInfo(computersEF);
    }

    private static void ShowComputerInfo(IEnumerable<Computer>? computers)
    {
        Console.WriteLine("'ComputerId','Motherboard','HasWifi','HasLTE','ReleaseDate'"
            + ",'Price','VideoCard'");

        if (computers is not null)
        {
            foreach (Computer singleComputer in computers)
            {
                Console.WriteLine("'" + singleComputer.ComputerId 
                    + "','" + singleComputer.Motherboard
                    + "','" + singleComputer.HasWifi
                    + "','" + singleComputer.HasLTE
                    + "','" + singleComputer.ReleaseDate.ToString("yyyy-MM-dd")
                    + "','" + singleComputer.Price.ToString("0.00", CultureInfo.InvariantCulture) // In some regions decimal requires comma as separator
                    + "','" + singleComputer.VideoCard + "'");
            }
        }
    }

    internal void TestDbConnectivity()
    {
        string testSqlConnectionCmd = "SELECT GETDATE()";

        // Dapper command that will return an array of results (rows)
        IEnumerable<DateTime> DateTimeIEnum = _dapper.GetRows<DateTime>(testSqlConnectionCmd);

        foreach (var date in DateTimeIEnum)
        {
            Console.WriteLine(date);
        }

        DateTime currentTimeFromDatabase = _dapper.GetSingleRow<DateTime>(testSqlConnectionCmd);
        Console.WriteLine("Now trying single query");
        Console.WriteLine(currentTimeFromDatabase);
    }
}