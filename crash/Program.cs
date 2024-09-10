using crash.support;
using crash.Models;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Dapper;
// Playing with restoring Program.cs to before .NET 6 
//  except for inline ?? namespaces
// See https://aka.ms/new-console-template for more information
// The namespace, which is project name, is normally implicit, but:
namespace crash;

// Runs Main method by default? or something
// Used Shift-Alt-F to format (VS Code)
internal class Program
{
    static void Main(string[] args)
    {
        string ConnectionString = "Server=localhost;Database=DotNetCourseDatabase;TrustServerCertificate=true;Trusted_Connection=true";

        IDbConnection dbConnection = new SqlConnection(ConnectionString);

        string testSqlConnectionCmd = "SELECT GETDATE()";

        // Dapper command that will return an array of results (rows)
        var DateTimeIEnum = dbConnection.Query<DateTime>(testSqlConnectionCmd);

        foreach (var date in DateTimeIEnum){
            Console.WriteLine(date);
        }
        //string unused; // Doesn't seem to be compilation error to not assign        

        // args would still be accessible if Program class and Main
        // were hidden as usual
        if (args?.Length > 0) {
        Console.WriteLine(args[0]);
        }
        else {Console.WriteLine("No args passed");}

        Computer myComputer = new Computer()
        {
            Motherboard = "Z690",
            HasWifi = true,
            HasLTE = false,
            ReleaseDate = DateTime.Now,
            Price = 943.87m,
            VideoCard = "RTX 2060"
        };

        myComputer.HasWifi = false;
        Console.WriteLine(myComputer.Motherboard);
        Console.WriteLine(myComputer.HasWifi);
        Console.WriteLine(myComputer.ReleaseDate);
        Console.WriteLine(myComputer.VideoCard);

        // Why is it warning, if null it won't loop?
        // foreach (string argument in args)
        // {
        //     Console.WriteLine(argument);
        // }

        // Support.PlayWithTypes();
    }

}

