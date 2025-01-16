using System;
using System.Text;
using System.Threading;
using BL;
using System.Timers;
using Persistence;
using Games;


namespace ConsolePL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            var adminService = new AdminService();
            try
            {
                while (true)
                {
                    Console.Clear();
                    DisplayNetcoLogo();
                    Console.WriteLine("****************************************");
                    Console.WriteLine("* Welcome to Internet CO v1.0          *");
                    Console.WriteLine("*                                       *");
                    Console.WriteLine("* 1. Login                              *");
                    Console.WriteLine("* 2. Exit                               *");
                    Console.WriteLine("****************************************");
                    Console.Write("Select an option: ");
                    string option = Console.ReadLine();

                    if (option == "1")
                    {
                        Login();
                    }
                    else if (option == "2")
                    {
                        Console.WriteLine("Exiting...");
                        break;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(1500);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Unexpected error: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void Login()
        {
            var userService = new UserService();
            var adminService = new AdminService();

            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password (min 8 characters): ");
            string password = ReadPassword();

            if (password.Length < 8)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nPassword must be at least 8 characters long.");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                return;
            }

            var result = userService.Login(username, password);

            if (result.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nLogin successful! Role: {result.Data}");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);

                var settingsService = new SettingsService();
                decimal ratePerHour;
                try
                {
                   ratePerHour = settingsService.GetRatePerHour();
                }
                catch (Exception ex)
                {
                   Console.ForegroundColor = ConsoleColor.Red;
                   Console.WriteLine($"\nUnexpected error: {ex.Message}");
                   Console.ResetColor();
                   System.Threading.Thread.Sleep(2000);
                   return;
                }

                // Lấy số dư tài khoản
                var balance = userService.GetBalance(username);
                Console.WriteLine($"\nYour current balance: {balance} VND");
                
                // Kiểm tra số dư
                decimal requiredBalance = ratePerHour; // Số tiền tối thiểu để sử dụng dịch vụ
                if (balance < requiredBalance)
                {
                   Console.ForegroundColor = ConsoleColor.Red;
                   Console.WriteLine("\nInsufficient balance. Please add more money to your account.");
                   Console.ResetColor();
                   AddMoney(username, userService);
                }
                ShowMenu(result.Data, username, userService, adminService);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nLogin failed: {result.Message}");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
            }
        }

    static void AddMoney(string username, UserService userService)
    {
    Console.Write("Enter amount to add (<= 100,000 VND transaction): ");
    if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount <= 100000)
    {
        try
        {
            userService.UpdateUserBalance(username, amount);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSuccessfully added {amount} VND to your account.");
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
        Console.WriteLine("\nInvalid amount. Please enter exactly 100,000 VND.");
        Console.ResetColor();
    }
    System.Threading.Thread.Sleep(1500);
    }


        static void ShowMenu(string role, string username, UserService userService, AdminService adminService )
        {
            while (true)
            {
                if (role == "admin")
                {
                    ShowAdminMenu(adminService);
                }
                else if (role == "user")
                {
                    userService.SelectComputer(username);
                    ShowUserMenu(role,username, userService);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Unknown role. Exiting...");
                    Console.ResetColor();
                    break;
                }

                Console.WriteLine("\nPress any key to return to the login screen...");
                Console.ReadKey();
                break;
            }
        }

    static void ShowAdminMenu(AdminService adminService)
   {
    var settingsService = new SettingsService();
    bool isRunning = true;
    while (isRunning)
    {
        Console.Clear();
        DisplayNetcoLogo();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("****************************************");
        Console.WriteLine("* Welcome to Internet Shop Manager v1.0 *");
        Console.WriteLine("*            Admin Menu                 *");
        Console.WriteLine("****************************************");
        Console.ResetColor();

        Console.WriteLine("1. Update Rate Per Hour");
        Console.WriteLine("2. Search Computers");
        Console.WriteLine("3. Display All Computers");
        Console.WriteLine("4. Search User");
        Console.WriteLine("5. Register User");
        Console.WriteLine("6. Logout");
        Console.Write("Enter your choice: ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                Console.Write("Enter new rate per hour (VND): ");
                if (decimal.TryParse(Console.ReadLine(), out decimal newRate) && newRate > 0)
                {
                try
                {
                settingsService.UpdateRatePerHour(newRate);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nSuccessfully updated rate per hour to {newRate} VND.");
                }
                catch (Exception ex)
                {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError updating rate: {ex.Message}");
                }
                finally
                {
                Console.ResetColor();
                }
                }
                else
                {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nInvalid rate. Please try again.");
                Console.ResetColor();
                }
                System.Threading.Thread.Sleep(1500);
                break;
            case "2":
                adminService.SearchComputers();
                break;
            case "3":
                adminService.DisplayAllComputers();
                break;
            case "4":
                adminService.SearchUsers();
                break;
            case "5":
                adminService.RegisterUser();
                break;
            case "6":
                Console.WriteLine("Logging out...");
                isRunning = false;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid option. Please try again.");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                break;
        }
    }
   }


    static void ShowUserMenu(string role, string username, UserService userService)
    {
        var sessionService = new SessionService();
        var settingsService = new SettingsService();
        decimal ratePerHour;
        try
        {
           ratePerHour = settingsService.GetRatePerHour();
        }
        catch (Exception ex)
        {
           Console.ForegroundColor = ConsoleColor.Red;
           Console.WriteLine($"\nUnexpected error: {ex.Message}");
           Console.ResetColor();
           System.Threading.Thread.Sleep(1500);
           return;
        }
    
        System.Timers.Timer sessionTimer= null;
        bool sessionActive = false;
        DateTime sessionStartTime = DateTime.MinValue;

        while (true)
        {
            Console.Clear();
            DisplayNetcoLogo();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("****************************************");
            Console.WriteLine("* Welcome to Internet CO v1.0          *");
            Console.WriteLine("*            User Menu                 *");
            Console.WriteLine("****************************************");
            Console.ResetColor();
    
            // Kiểm tra số dư
            var balance = userService.GetBalance(username);
            Console.WriteLine($"\nYour current balance: {balance} VND");
            decimal requiredBalance = ratePerHour;
    
            if (balance < requiredBalance)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nInsufficient balance. Please add more money to use other features.");
                Console.ResetColor();
                AddMoney(username, userService);
                continue; // Quay lại kiểm tra số dư sau khi nạp tiền
            }
    
            Console.WriteLine("1. Start Session");
            Console.WriteLine("2. End Session");
            Console.WriteLine("3. Add Money");
            Console.WriteLine("4. Play Game");
            Console.WriteLine("5. Logout");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();
    
            if (choice == "1")
            {
                if (balance < ratePerHour)
                {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nInsufficient balance to start a session. Please add more money.");
                Console.ResetColor();
                AddMoney(username, userService);
                continue;
                }
                try
                {
                    sessionService.StartSession(username);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nSession started successfully!");
                    Console.ResetColor();

                    sessionActive = true;
                    sessionStartTime = DateTime.Now;
                    // Start a timer to display the running session time
                    sessionTimer = new System.Timers.Timer(1000);
                    sessionTimer.Elapsed += (sender, e) => DisplayRunningSessionTime(sessionStartTime);
                    sessionTimer.Start();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nError starting session: {ex.Message}");
                }
                finally
                {
                    Console.ResetColor();
                    System.Threading.Thread.Sleep(1500);
                }
            }
            else if (choice == "2")
            {
                try
                {
                var cost = sessionService.EndSession(username, ratePerHour);
                if (balance - cost < requiredBalance)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nInsufficient balance to cover the session cost. Please add more money.");
                    Console.ResetColor();
                    AddMoney(username, userService);
                    continue;
                }
                userService.UpdateUserBalance(username, -cost);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nSession ended successfully! Cost: {cost} VND");
                // Stop the session thread
                sessionActive = false;
                sessionTimer?.Stop();
                sessionTimer?.Dispose();

                }
                catch (Exception ex)
                {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError ending session: {ex.Message}");
                }
                finally
                {
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                }
            }
            else if (choice == "3")
            {
                AddMoney(username, userService);
            }
            else if (choice == "4")
            {
                Console.Clear();
                Console.WriteLine("Starting Snake Game...");
                System.Threading.Thread.Sleep(1000);

                try
                {
                    var snakeGame = new SnakeGame();
                    snakeGame.StartGame();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nError running Snake Game: {ex.Message}");
                    Console.ResetColor();
                }

                Console.WriteLine("\nExiting Snake Game... Returning to User Menu...");
                System.Threading.Thread.Sleep(2000);
            }
            else if (choice == "5")
            {
            if (sessionActive)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nYou must end your session before logging out.");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
            }
            else
            {
                userService.Logout(username);
                Console.WriteLine("Logging out...");
                break;
            }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid option. Please try again.");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
            }

            // Display the running session time at the bottom of the console
            if (sessionActive)
            {
              var elapsedTime = DateTime.Now - sessionStartTime;
              Console.SetCursorPosition(0, Console.WindowHeight - 2); // Adjust the cursor position to the bottom of the console
              Console.WriteLine($"\nSession running time: {elapsedTime:hh\\:mm\\:ss}");
            }
        }
    }


        static string ReadPassword()
        {
            StringBuilder password = new();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Length--;
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            return password.ToString();
        }

        static void DisplayNetcoLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("******************************************************");
            Console.WriteLine("*                                                    *");
            Console.WriteLine("*    ███    ██ ███████ ████████  ██████   ██████     *");
            Console.WriteLine("*    ████   ██ ██         ██    ██     █ ██    ██    *");
            Console.WriteLine("*    ██ ██  ██ ███████    ██    ██       ██    ██    *");
            Console.WriteLine("*    ██  ██ ██ ██         ██    ██     █ ██    ██    *");
            Console.WriteLine("*    ██   ████ ███████    ██     ██████   ██████     *");
            Console.WriteLine("*                                                    *");
            Console.WriteLine("******************************************************");
            Console.ResetColor();
        }

        static void DisplayRunningSessionTime(DateTime sessionStartTime)
        {
          var elapsedTime = DateTime.Now - sessionStartTime;
          Console.SetCursorPosition(0, 20); // Adjust the cursor position to where you want to display the running time
          Console.WriteLine($"\nSession running time: {elapsedTime:hh\\:mm\\:ss}");
        }
    }
}
