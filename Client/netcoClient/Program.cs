using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace TCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
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
                        // Kết nối server
                        try
                        {
                            using (TcpClient client = new TcpClient())
                            {
                                // Kết nối với server
                                client.Connect("127.0.0.1", 5000);
                                NetworkStream stream = client.GetStream();
                                
                                // Đăng nhập
                                if (Login(stream, out string role))
                                {
                                    ShowMenu(role);
                                }
                            }
                        }
                        catch (SocketException ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error connecting to server: {ex.Message}");
                            Console.ResetColor();
                        }
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
                        System.Threading.Thread.Sleep(2000);
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

        static bool Login(NetworkStream stream, out string role)
        {
            role = string.Empty;
            bool isAuthenticated = false;

            while (!isAuthenticated)
            {
                try
                {
                    Console.Clear();
                    DisplayNetcoLogo();
                    DisplayLoginScreen();

                    Console.Write("Enter username: ");
                    string username = Console.ReadLine();

                    Console.Write("Enter password (min 8 characters): ");
                    string password = ReadPassword();

                    // Kiểm tra mật khẩu đủ điều kiện
                    if (password.Length < 8)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nPassword must be at least 8 characters long.");
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(2000);
                        continue;
                    }

                    // Gửi thông tin đăng nhập tới server
                    var loginRequest = new { Username = username, Password = password };
                    string requestData = JsonSerializer.Serialize(loginRequest);
                    byte[] requestBytes = Encoding.UTF8.GetBytes(requestData + "\n");
                    stream.Write(requestBytes, 0, requestBytes.Length);

                    // Nhận phản hồi từ server
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        throw new Exception("No response from server.");
                    }

                    string responseData = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseData);

                    if (loginResponse != null && loginResponse.Success)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nLogin successful! Role: {loginResponse.Role}");
                        Console.ResetColor();
                        role = loginResponse.Role;
                        System.Threading.Thread.Sleep(1500);
                        isAuthenticated = true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nLogin failed! Invalid credentials.");
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(2000);
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error during login: {ex.Message}");
                    Console.ResetColor();
                    break;
                }
            }

            return isAuthenticated;
        }

        static void ShowMenu(string role)
        {
            while (true)
            {
                if (role == "admin")
                {
                    ShowAdminMenu();
                }
                else if (role == "user")
                {
                    ShowUserMenu();
                }
                else
                {
                    Console.WriteLine("Unknown role. Exiting...");
                    break;
                }

                Console.WriteLine("\nPress any key to return to the login screen...");
                Console.ReadKey();
                break; // Quay lại màn hình login
            }
        }

        static void ShowAdminMenu()
        {
            Console.Clear();
            DisplayNetcoLogo(); // Hiển thị logo NETCO
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("****************************************");
            Console.WriteLine("* Welcome to Internet Shop Manager v1.0 *");
            Console.WriteLine("*            Admin Menu                 *");
            Console.WriteLine("****************************************");
            Console.ResetColor();
            Console.WriteLine("1. Search Computers");
            Console.WriteLine("2. Search User");
            Console.WriteLine("3. Register User");
            Console.WriteLine("4. Update Money");
            Console.WriteLine("5. Logout");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            if (choice == "5")
            {
                Console.WriteLine("Logging out...");
            }
            else
            {
                Console.WriteLine($"You selected option {choice} as Admin.");
            }
        }

        static void ShowUserMenu()
        {
            Console.Clear();
            DisplayNetcoLogo(); // Hiển thị logo NETCO
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("****************************************");
            Console.WriteLine("* Welcome to Internet CO v1.0          *");
            Console.WriteLine("*            User Menu                 *");
            Console.WriteLine("****************************************");
            Console.ResetColor();
            Console.WriteLine("1. Please select a computer");
            Console.WriteLine("2. Make a Payment");
            Console.WriteLine("3. Game");
            Console.WriteLine("4. Logout");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            if (choice == "4")
            {
                Console.WriteLine("Logging out...");
            }
            else
            {
                Console.WriteLine($"You selected option {choice} as User.");
            }
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

        static void DisplayLoginScreen()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("****************************************");
            Console.WriteLine("* Welcome to Internet CO v1.0          *");
            Console.WriteLine("*            Login Screen              *");
            Console.WriteLine("****************************************");
            Console.ResetColor();
        }

        static string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(intercept: true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[0..^1];
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            return password;
        }

        class LoginResponse
        {
            public bool Success { get; set; }
            public string Role { get; set; }
        }
    }
}
