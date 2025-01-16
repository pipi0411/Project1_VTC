using System;
using Persistence;
using BL;

public class AdminService
{
    private ComputerService computerService;
    private UserService userService;

    public AdminService()
    {
        computerService = new ComputerService();
        userService = new UserService();
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

    public void SearchUsers()
    {
        Console.Write("Enter username to search: ");
        string username = Console.ReadLine();
        var user = userService.GetUserByUsername(username);
        if (user != null)
        {
            DisplayUserDetails(user);
        }
        else
        {
            Console.WriteLine("User not found.");
        }
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }

    private void DisplayUserDetails(User user)
    {
        Console.WriteLine($"ID: {user.Id}");
        Console.WriteLine($"Name: {user.Username}");
        Console.WriteLine($"Balance: {user.Balance} VND");
        var computer = computerService.GetComputerById(user.ComputerId);
        if (computer != null)
        {
            Console.WriteLine($"Computer: {computer.Name}");
        }
        else
        {
            Console.WriteLine("Computer: None");
        }
    }

    public void RegisterUser()
    {
        Console.Write("Enter new username: ");
        string username = Console.ReadLine();
        if (userService.GetUserByUsername(username) != null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nUsername already exists.");
            Console.ResetColor();
            System.Threading.Thread.Sleep(1500);
            return;
        }

        Console.Write("Enter password: ");
        string password = Console.ReadLine();
        if (password.Length < 8)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nPassword must be at least 8 characters long.");
            Console.ResetColor();
            System.Threading.Thread.Sleep(1500);
            return;
        }
        var user = new User
        {
            Username = username,
            Password = password,
            Role = "user" // Đặt vai trò mặc định là "user"
        };
        userService.RegisterUser(user);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nUser registered successfully.");
        
        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}