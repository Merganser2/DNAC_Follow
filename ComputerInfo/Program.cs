// Playing with restoring Program.cs to before .NET 6 
//  except for inline ?? namespaces
// See https://aka.ms/new-console-template for more information
// The namespace, which is project name, is normally implicit, but:
namespace ComputerInfo;

// Runs Main method by default? or something
// Used Shift-Alt-F to format (VS Code)
internal class Program
{
    static void Main(string[] args)
    {
        // DbRelated.TestDbConnectivity();

        DbRelated.InsertComputerInfo();

        DbRelated.GetComputerInfo();
    }
}
