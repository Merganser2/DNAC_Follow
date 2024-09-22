namespace AsyncPlay; // {

internal class Program
{
    // Change from void to async Task - don't have to do this in new-style Program.cs (look up how/why)
    static async Task Main(string[] args)
    {
        Console.WriteLine("Program started");

        var task1 = new Task(() =>
        {
            Thread.Sleep(100);
            Console.WriteLine("*** Task 1...");
        });
        Console.WriteLine("After Task 1 created");

        task1.Start();

        Task task2 = ConsoleAfterDelayAsync("*** Task 2...", 150);
        // We don't need an await because it takes less time than Task 1
        Task task3 = ConsoleAfterDelayAsync("*** Task 3...", 50);
        await task2;
        System.Console.WriteLine("Finished waiting for task2");
        await task1;
        System.Console.WriteLine("Finished waiting for task1");
        await task3;
        System.Console.WriteLine("Finished waiting for task3");

        Console.WriteLine("End of method");
    }

   // Static methods
   static void ConsoleAfterDelay(string text, int delayMs)
    {
        Thread.Sleep(delayMs);
        Console.WriteLine(text);
    }

    static async Task ConsoleAfterDelayAsync(string text, int delayMs)
    {
        await Task.Delay(delayMs);
        Console.WriteLine(text);
    }
}

