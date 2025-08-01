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
    // Định dạng trạng thái với màu
    string statusText = computer.IsOn ? "On " : "Off";
    Console.ForegroundColor = computer.IsOn ? ConsoleColor.Green : ConsoleColor.Red;

    // Tính thời gian chạy nếu máy tính đang bật
    string runningTime = "N/A";
    if (computer.IsOn && computer.OnTime.HasValue)
    {
        var elapsedTime = DateTime.Now - computer.OnTime.Value;
        runningTime = $"{elapsedTime:hh\\:mm\\:ss}";
    }

    // In từng dòng dữ liệu
    Console.WriteLine($"║ {computer.Id,-6} ║ {computer.Name,-14} ║ {statusText,-10} ║ {computer.CurrentUser ?? "None",-16} ║ {runningTime,-12} ║");

    Console.ResetColor();
}


public void DisplayAllComputers()
{
    var computers = computerService.GetAllComputers();

    if (computers == null || computers.Count == 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("No computers available.");
        Console.ResetColor();
        return;
    }

    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;

    // In tiêu đề bảng
    Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
    Console.WriteLine("║   ID   ║      Name      ║   Status   ║   Current User   ║ Running Time ║");
    Console.WriteLine("══════════════════════════════════════════════════════════════════════════");
    Console.ResetColor();

    // Hiển thị từng dòng thông tin máy tính
    foreach (var computer in computers)
    {
        DisplayComputerDetails(computer);
    }

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("═══════════════════════════════════════════════════════════════════════");
    Console.ResetColor();

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
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;

    // In tiêu đề bảng
    Console.WriteLine("═══════════════════════════════════════════════════════════════════");
    Console.WriteLine("║   ID   ║     Name      ║   Balance   ║   Computer   ║   Status  ║");
    Console.WriteLine("═══════════════════════════════════════════════════════════════════");
    Console.ResetColor();

    // In thông tin người dùng với màu sắc cho trạng thái online
    string onlineStatus = user.Online ? "Online" : "Offline";
    ConsoleColor statusColor = user.Online ? ConsoleColor.Green : ConsoleColor.Red;
    
    Console.Write($"║ {user.Id,-6} ║ {user.Username,-13} ║ {user.Balance,-11} ║ {GetComputerName(user.ComputerId),-12} ║ ");
    Console.ForegroundColor = statusColor;
    Console.Write($"{onlineStatus,-9}");
    Console.ResetColor();
    Console.WriteLine(" ║");

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("═══════════════════════════════════════════════════════════════════");
    Console.ResetColor();
}

private string GetComputerName(int? computerId)
{
    if (computerId.HasValue)
    {
        var computer = computerService.GetComputerById(computerId.Value);
        return computer != null ? computer.Name : "None";
    }
    return "None";
}

public void RegisterUser()
{
    Console.Write("Enter new username: ");
    string username = Console.ReadLine();
    if (userService.GetUserByUsername(username) != null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nUsername already exists. Please try again.");
        Console.ResetColor();
        System.Threading.Thread.Sleep(1500);
        return;
    }

    string password, confirmPassword = string.Empty;
    do
    {
        Console.Write("Enter password (at least 8 characters): ");
        password = Console.ReadLine();
        if (password.Length < 8)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nPassword must be at least 8 characters long.");
            Console.ResetColor();
            System.Threading.Thread.Sleep(1500);
            continue;
        }

        Console.Write("Confirm password: ");
        confirmPassword = Console.ReadLine();

        if (password != confirmPassword)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nPasswords do not match. Please try again.");
            Console.ResetColor();
            System.Threading.Thread.Sleep(1500);
        }
    } while (password.Length < 8 || password != confirmPassword);

    decimal balance = 0;
    do 
    {
        Console.Write("Enter initial balance (VND): ");
        if (!decimal.TryParse(Console.ReadLine(), out balance) || balance < 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nInvalid balance. Please enter a non-negative number.");
            Console.ResetColor();
            System.Threading.Thread.Sleep(1500);
        }
    } while (balance < 0);

    Console.Write("\nAre you sure you want to register this user? (Y/N): ");
    string confirmation = Console.ReadLine().Trim().ToLower();
    if (confirmation != "y")
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nRegistration cancelled. Returning to admin menu...");
        Console.ResetColor();
        System.Threading.Thread.Sleep(1500);
        return;
    }

    var user = new User
    {
        Username = username,
        Password = password,
        Balance = balance,
        Role = "user",
        Online = false  // User mới tạo sẽ có trạng thái offline
    };
    
    userService.RegisterUser(user);
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nUser registered successfully.");
    Console.ResetColor();

    Console.WriteLine("\nPress any key to return to the menu...");
    Console.ReadKey();
}

public void PaymentUser()
{
    Console.Write("Enter User ID to add money: ");
    if (int.TryParse(Console.ReadLine(), out int userId))
    {
        var user = userService.GetUserById(userId);
        if (user != null)
        {
            Console.Write($"Enter amount to add to {user.Username}'s account: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                try
                {
                    userService.UpdateUserBalance(user.Username, amount);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nSuccessfully added {amount} VND to {user.Username}'s account.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nError adding money: {ex.Message}");
                }
                finally
                {
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nInvalid amount. Please enter a positive amount.");
                Console.ResetColor();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nUser not found.");
            Console.ResetColor();
        }
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\nInvalid User ID.");
        Console.ResetColor();
    }
    System.Threading.Thread.Sleep(1500);
}

}