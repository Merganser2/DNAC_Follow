// Playing with restoring Program.cs to before .NET 6 
// See https://aka.ms/new-console-template for more information
// The namespace, which is project name, is implicit by default
namespace ComputerMapping;

internal class Program
{
    static void Main(string[] args)
    {
        DbInteractions dbInteractions = new DbInteractions();

        dbInteractions.InsertComputerInfoFromJsonUsingAutoMapper("ComputersSnake.json");
        //dbInteractions.InsertComputerInfoFromJson("Computers.json");

        dbInteractions.GetAllComputersComputerInfo();

        // dbInteractions.TestDbConnectivity();
    }
}

