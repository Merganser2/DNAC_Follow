using System.Globalization;
using ComputerMapping.Data;
using ComputerMapping.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace ComputerMapping;

internal class DbInteractions
{
    private IConfiguration _config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                               .Build();

    private readonly DataContextDapper _dapper; 
    private readonly DataContextEF _entityFramework; // Not currently in use but may add back

    internal DbInteractions()
    {
        _config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                            .Build();
        _dapper = new DataContextDapper(_config);

        _entityFramework = new DataContextEF(_config);
    }

    internal void InsertComputerInfoFromJson(string jsonPath)
    {
        string computersJson = File.ReadAllText(jsonPath);

        // We need these or some values won't get mapped
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Get the rows to add from the Json file into collection of computers
        IEnumerable<Computer>? computers = JsonSerializer.Deserialize<IEnumerable<Computer>>(computersJson, jsonOptions);

        // Insert the data to add to the database
        if (computers is not null)
        {
            string startOf_SQL_ComputerInsertCmd = @"INSERT INTO TutorialAppSchema.Computer (
            Motherboard,
            HasWifi,
            HasLTE,
            ReleaseDate,
            Price,
            VideoCard        
            ) VALUES ('";

            foreach (var computer in computers)
            {
                string insertSqlCmd = startOf_SQL_ComputerInsertCmd + 
                          EscapeSingleQuote(computer.Motherboard)
                + "','" + computer.HasWifi
                + "','" + computer.HasLTE
                + "','" + computer.ReleaseDate
                + "','" + computer.Price
                + "','" + EscapeSingleQuote(computer.VideoCard)
                + "')";

                // Insert the row from Json using Dapper
                _dapper.ExecuteSql(insertSqlCmd);
            }
        }

        string computersCopySystem = JsonSerializer.Serialize(computers, jsonOptions); // System.Text.Json

        File.WriteAllText("computersCopySystem.txt", computersCopySystem);        
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
        // IEnumerable<Computer>? computersEF = _entityFramework.Computer?.ToList<Computer>();

        ShowComputerInfo(computers);
        // ShowComputerInfo(computersEF);
    }

    // Single-quote will look like VARCHAR terminator to SQL; 
    // using two sequentially tells it to put one ' in string
    private string EscapeSingleQuote(string motherboard)
    {
        return motherboard.Replace("'","''");
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
                    + "','" + singleComputer.ReleaseDate?.ToString("yyyy-MM-dd")
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