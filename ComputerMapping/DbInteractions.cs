using System.Globalization;
using ComputerMapping.Data;
using ComputerMapping.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using AutoMapper;

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

    // Uses JsonPropertyName attributes that we added above each property in Computer model
    internal void InsertComputerInfoFromSnakeJsonUsingJsonAttributes(string jsonPath)
    {
        string computersJson = File.ReadAllText(jsonPath); // passing in json file with underscores in keys

        // We can directly Deserialize into a Computer model because JsonPropertyName attributes convert the properties,
        IEnumerable<Computer>? computers = JsonSerializer.Deserialize<IEnumerable<Computer>>(computersJson);

        if (computers is not null)
        {
            InsertComputersIntoDb<Computer>(computers);
            string computersCopySystem = JsonSerializer.Serialize(computers); // System.Text.Json

            File.WriteAllText("computersUsingJsonAttributes.txt", computersCopySystem);        
        }
    }

    internal void InsertComputerInfoFromSnakeJsonUsingAutoMapper(string jsonPath)
    {
        string computersJson = File.ReadAllText(jsonPath); // passing in json file with underscores in keys

        Mapper mapper = GetComputerSnake_to_ComputerMapper(); 

        // We don't need the JsonSerializerOptions because Model already matches the Json file
        IEnumerable<ComputerSnake>? computerSnakes = JsonSerializer.Deserialize<IEnumerable<ComputerSnake>>(computersJson);

        // We can use these for serialization if we want output json to follow Camel Case convention
        JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (computerSnakes is not null)
        {
            // Notice mapper can also convert to/from an IEnumerable of the source/destination models
            var computers = mapper.Map<IEnumerable<Computer>>(computerSnakes);

            InsertComputersIntoDb<Computer>(computers);
            string computersCopySystem = JsonSerializer.Serialize(computers, jsonOptions); // System.Text.Json

            File.WriteAllText("computersCopyAutomapper.txt", computersCopySystem);        
        }
    }

    private Mapper GetComputerSnake_to_ComputerMapper()
    {
        return new Mapper(new MapperConfiguration((cfg) =>
        {
            cfg.CreateMap<ComputerSnake, Computer>()
             .ForMember(destination => destination.ComputerId, options =>
                 options.MapFrom(source => source.computer_id))
             .ForMember(destination => destination.CPUCores, options =>
                 options.MapFrom(source => source.cpu_cores))
             .ForMember(destination => destination.HasLTE, options =>
                 options.MapFrom(source => source.has_lte))
             .ForMember(destination => destination.HasWifi, options =>
                 options.MapFrom(source => source.has_wifi))
             .ForMember(destination => destination.Motherboard, options =>
                 options.MapFrom(source => source.motherboard))
             .ForMember(destination => destination.VideoCard, options =>
                 options.MapFrom(source => source.video_card))
             .ForMember(destination => destination.ReleaseDate, options =>
                 options.MapFrom(source => source.release_date))
             .ForMember(destination => destination.Price, options =>
                 options.MapFrom(source => source.price));
        }));
    }

    private void InsertComputersIntoDb<Computers>(IEnumerable<Computer> computers)
    {
        // Insert the data to add to the database
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

            // Console.WriteLine(computer.Motherboard); // Sanity check
            // Insert the row from Json using Dapper
            _dapper.ExecuteSql(insertSqlCmd);
        }
    }

    internal void GetAllComputersComputerInfo()
    {
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

        // With Dapper
        IEnumerable<Computer> computers = _dapper.GetRows<Computer>(sqlSelect);
        ShowComputerInfo(computers);

        // With Entity Framework
        // IEnumerable<Computer>? computersEF = _entityFramework.Computer?.ToList<Computer>();
        // ShowComputerInfo(computersEF);
    }

    internal void ClearAllComputerInfo()
    {
        string sqlSelect = "TRUNCATE TABLE TutorialAppSchema.Computer";

        // With Dapper
        int rowsRemoved = _dapper.ExecuteSqlWithRowCount(sqlSelect);
        Console.WriteLine(rowsRemoved + " rows removed.");
    }

    // Single-quote will look like VARCHAR terminator to SQL; 
    // using two sequentially tells it to put one ' in string
    private string EscapeSingleQuote(string? sqlCmdText) // TODO: change back
    {
        if (sqlCmdText is not null)
        return sqlCmdText.Replace("'","''");
        else return "";        
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