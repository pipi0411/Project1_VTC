using System;
using System.Collections.Generic;

namespace InternetShopManager
{
    class Program
    {
        // Mock dữ liệu tài khoản
        private static Dictionary<string, (string Password, string Role)> users = new Dictionary<string, (string, string)>
        {
            { "admin", ("admin123", "admin") },
            { "user", ("user123", "user") }
        };

        static void Main(string[] args)
        {
            Console.Title = "Login Console Application";
            string username = string.Empty;
            string role = string.Empty;

            // Màn hình đăng nhập
            bool isAuthenticated = false;
            while (!isAuthenticated)
            {
                Console.Clear();
                DisplayLoginScreen();

                Console.Write("Username: ");
                username = Console.ReadLine();

                Console.Write("Password: ");
                string password = ReadPassword();

                // Xác thực thông tin tài khoản
                if (ValidateLogin(username, password, out role))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nLogin successfully! Role: {0}", role);
                    Console.ResetColor();
                    isAuthenticated = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nLogin failed! Please try again.");
                    Console.ResetColor();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }

            // Chuyển vào menu chính
            if (role == "admin")
            {
                DisplayAdminMenu();
            }
            else if (role == "user")
            {
                DisplayUserMenu();
            }
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

        static bool ValidateLogin(string username, string password, out string role)
        {
            role = string.Empty;

            // Kiểm tra username và password có tồn tại trong dictionary không
            if (users.ContainsKey(username) && users[username].Password == password)
            {
                role = users[username].Role; // Lấy role của người dùng
                return true;
            }
            return false; // Đăng nhập không thành công
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

        static void DisplayAdminMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("****************************************");
            Console.WriteLine("* Welcome to Internet Shop Manager v1.0 *");
            Console.WriteLine("*            Admin Menu                 *");
            Console.WriteLine("****************************************");
            Console.ResetColor();

            string[] menuItems = new string[]
            {
                "1. Search Computers",
                "2. Search User",
                "3. Register User",
                "4. Logout"
            };

            foreach (var item in menuItems)
            {
                Console.WriteLine(item);
            }

            Console.Write("\nPlease select an option: ");
            Console.ReadLine();
            Console.WriteLine("\nBack to login...");
            Console.ReadKey();
        }

        static void DisplayUserMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("****************************************");
            Console.WriteLine("* Welcome to Internet CO v1.0          *");
            Console.WriteLine("*            User Menu                 *");
            Console.WriteLine("****************************************");
            Console.ResetColor();

            string[] menuItems = new string[]
            {
                "1. Please select a computer",
                "2. Payment",
                "3. Game",
                "4. Logout"
            };

            foreach (var item in menuItems)
            {
                Console.WriteLine(item);
            }

            Console.Write("\nPlease select an option: ");
            Console.ReadLine();
            Console.WriteLine("\nBack to login...");
            Console.ReadKey();
        }
    }
}
