using System;
using Persistence;

public class AdminService
{
    private ComputerService computerService;

    public AdminService()
    {
        computerService = new ComputerService();
    }

    public void SearchComputers()
    {
        Console.Write("Enter Computer ID to search: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var computer = computerService.GetComputerById(id);
            if (computer != null)
            {
                DisplayComputerDetails(computer);
            }
            else
            {
                Console.WriteLine("Computer not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }

    private void DisplayComputerDetails(Computer computer)
    {
        Console.WriteLine($"ID: {computer.Id}");
        Console.WriteLine($"Name: {computer.Name}");
        Console.WriteLine($"Status: {(computer.IsOn ? "On" : "Off")}");
        Console.WriteLine($"Current Users: {computer.CurrentUser ?? "None"}");
        if (computer.IsOn && computer.OnTime.HasValue)
        {
            var elapsedTime = DateTime.Now - computer.OnTime.Value;
            Console.WriteLine($"Running Time: {elapsedTime:hh\\:mm\\:ss}");
        }
    }

    public void DisplayAllComputers()
    {
        var computers = computerService.GetAllComputers();
        foreach (var computer in computers)
        {
            DisplayComputerDetails(computer);
            Console.WriteLine();
        }
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}