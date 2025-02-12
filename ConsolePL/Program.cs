using System;
using System.Text;
using System.Threading;
using BL;
using System.Timers;
using Persistence;
using Games;
using System.Threading.Tasks;


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
                    Console.WriteLine("* Welcome to Internet CO v1.0           *");
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
                   System.Threading.Thread.Sleep(1500);
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
            while (true)
            {
                if (role == "admin")
                {
                    ShowAdminMenu(adminService);
                }
                else if (role == "user")
                {
                    userService.SelectComputer(username, ratePerHour);
                    ShowUserMenu(role ,username, userService, ratePerHour);
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
        Console.WriteLine(" ***************************************");
        Console.WriteLine("* Welcome to Internet Shop Manager v1.0 *");
        Console.WriteLine("*            Admin Menu                 *");
        Console.WriteLine(" ***************************************");
        Console.ResetColor();

        string[] menuItems = {
            "1. Update Rate Per Hour",
            "2. Search Computers",
            "3. Display All Computers",
            "4. Search User",
            "5. Register User",
            "6. Logout"
        };
        DisplayMenu("Select an option", menuItems);
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


    static async void ShowUserMenu(string role, string username, UserService userService, decimal ratePerHour)
    {
        var sessionService = new SessionService();
        var settingsService = new SettingsService();
    
        System.Timers.Timer sessionTimer= null;
        bool sessionActive = false;
        DateTime sessionStartTime = DateTime.MinValue;

        void UpdateRunningSessionTime(object sender, System.Timers.ElapsedEventArgs e)
        {
        if (sessionActive)
        {
            var elapsedTime = DateTime.Now - sessionStartTime;
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;
            Console.SetCursorPosition(0, Console.WindowHeight - 2); // Adjust the cursor position to the bottom of the console
            Console.WriteLine($"Session running time: {elapsedTime:hh\\:mm\\:ss}");
            Console.SetCursorPosition(cursorLeft, cursorTop); // Restore the cursor position
        }
        }

        async Task<bool> WaitForDeposit(string username, UserService userService)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nYou must add money to continue. If you do not deposit within 1 minute, you will be logged out.");
            Console.ResetColor();

            var cts = new CancellationTokenSource();
            var taskDelay = Task.Delay(60000, cts.Token); // Chờ 1 phút

            while (!taskDelay.IsCompleted)
            {
                await Task.Delay(5000); // Kiểm tra sau mỗi 5 giây
                decimal balance = userService.GetBalance(username);
                if (balance >= ratePerHour)
                {
                    cts.Cancel(); // Hủy đếm ngược nếu user đã nạp tiền
                    return true;
                }
            }
            return false; // Nếu hết 1 phút mà chưa nạp tiền
        }

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
            // Hiển thị ratePerHour
            Console.WriteLine($"\nCurrent rate per hour: {ratePerHour} VND");
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
                // continue; // Quay lại kiểm tra số dư sau khi nạp tiền
                balance = userService.GetBalance(username);
                if (balance > requiredBalance) break; // Nếu user nạp đủ tiền thì tiếp tục

                // Nếu sau 1 phút không nạp tiền, tự động logout
                if (!await WaitForDeposit(username, userService))
                {
                    // Stop the session thread
                    sessionActive = false;
                    sessionTimer?.Stop();
                    sessionTimer?.Dispose();

                   Console.WriteLine("\nTime expired! Logging out...");
                   System.Threading.Thread.Sleep(2000);
                   userService.Logout(username);
                   return;
                }
            }

            // Tự động bắt đầu phiên ngay sau khi kiểm tra số dư
            if (!sessionActive)
            {
            try
            {
                sessionService.StartSession(username);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nSession started successfully!");
                Console.ResetColor();

                sessionActive = true;
                sessionStartTime = DateTime.Now;

                sessionTimer = new System.Timers.Timer(1000);
                sessionTimer.Elapsed += UpdateRunningSessionTime;
                sessionTimer.Start();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError starting session: {ex.Message}");
                Console.ResetColor();
                System.Threading.Thread.Sleep(1500);
                return;
            }
            }

            string[] menuItems = {
            "1. End Session",
            "2. Add Money",
            "3. Play Game",
            "4. View Session History"
            };
            DisplayMenu("Select an option", menuItems);
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();
   
            if (choice == "1")
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

                // Tự động đăng xuất ngay sau khi kết thúc phiên
                Console.WriteLine("\nLogging out...");
                System.Threading.Thread.Sleep(1500);
                userService.Logout(username);
                return;

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
            else if (choice == "2")
            {
                AddMoney(username, userService);
            }
            else if (choice == "3")
            {
                Console.Clear();
                Console.WriteLine("Starting Guess Number Game...");
                System.Threading.Thread.Sleep(1000);

                try
                {
                    var game = new GuessNumberGame();
                    game.StartGame();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nError running Guess Number Game: {ex.Message}");
                    Console.ResetColor();
                }
            }
            else if (choice == "4")
            {
                ViewSessionHistory(username, sessionService, ratePerHour);
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
          Console.Clear();
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

 

        static void ViewSessionHistory(string username, SessionService sessionService, decimal ratePerHour)
        {
           Console.Clear();
           Console.ForegroundColor = ConsoleColor.Cyan;
           Console.WriteLine("**********************************");
           Console.WriteLine("*        SESSION HISTORY         *");
           Console.WriteLine("**********************************");
           Console.ResetColor();

           var history = sessionService.GetSessionHistory(username, ratePerHour);
    
           if (history.Count == 0)
           {
              Console.WriteLine("\nNo previous sessions found.");
           }
           else
           {
              // Định nghĩa độ rộng cố định cho các cột
              const int col1Width = 5;  // ID
              const int col2Width = 22; // Start Time
              const int col3Width = 22; // End Time
              const int col4Width = 12; // Cost (VND)

              // Hiển thị tiêu đề bảng
              Console.WriteLine("\n" +
              "ID".PadRight(col1Width) +
              "Start Time".PadRight(col2Width) +
              "End Time".PadRight(col3Width) +
              "Cost (VND)".PadLeft(col4Width));
              Console.WriteLine(new string('-', col1Width + col2Width + col3Width + col4Width));

              // Hiển thị từng phiên trong lịch sử
              foreach (var session in history)
              {
                string endTime = session.EndTime.HasValue ? session.EndTime.Value.ToString("g") : "In Progress";
                string cost = session.Cost > 0 ? $"{session.Cost:N0} VND" : "0 VND";

                Console.WriteLine(
                  session.SessionId.ToString().PadRight(col1Width) +
                  session.StartTime.ToString("g").PadRight(col2Width) +
                  endTime.PadRight(col3Width) +
                  cost.PadLeft(col4Width)
                );
              }
           }

           Console.WriteLine("\nPress any key to return...");
           Console.ReadKey();
        }
        static void DisplayMenu(string title, string[] menuItems)
        {
          int menuWidth = 50; // Độ rộng cố định cho menu

          Console.ForegroundColor = ConsoleColor.Cyan;
          Console.WriteLine("╔" + new string('═', menuWidth) + "╗");
          Console.WriteLine("║" + title.PadRight(menuWidth) + "║");
          Console.WriteLine("╠" + new string('═', menuWidth) + "╣");

          Console.ResetColor();
          foreach (var item in menuItems)
          {
            Console.WriteLine("║ " + item.PadRight(menuWidth - 2) + " ║");
          }

           Console.ForegroundColor = ConsoleColor.Cyan;
           Console.WriteLine("╚" + new string('═', menuWidth) + "╝");
           Console.ResetColor();
        }
    }
}
