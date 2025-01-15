using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MySql.Data.MySqlClient;
using System.Threading;

namespace TCPServer
{
    class Program
    {
        // Mock dữ liệu tài khoản
        private const string connectionString = "server=127.0.0.1;user=root;password=Duyanh0612;database=InternetCo";

        static void Main(string[] args)
        {
            Console.WriteLine("Starting TCP Server...");
            TcpListener listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            Console.WriteLine("Server is listening on port 5000...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected!");

                // Xử lý client trong một thread mới
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);

            }
        }
        static void HandleClient(object clientObject)
        {
            TcpClient client = (TcpClient)clientObject;

            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Client disconnected.");
                        break;
                    }

                    string requestData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {requestData}");

                    // Deserialize JSON từ client
                    var loginRequest = JsonSerializer.Deserialize<LoginRequest>(requestData);

                    // Xử lý thông tin đăng nhập
                    string responseData;
                    if (ValidateLogin(loginRequest.Username, loginRequest.Password, out string role))
                    {
                        var response = new LoginResponse { Success = true, Role = role };
                        responseData = JsonSerializer.Serialize(response);
                    }
                    else
                    {
                        var response = new LoginResponse { Success = false, Role = null };
                        responseData = JsonSerializer.Serialize(response);
                    }

                    // Gửi phản hồi về client
                    byte[] responseBytes = Encoding.UTF8.GetBytes(responseData);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                    Console.WriteLine($"Sent: {responseData}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Connection closed.");
            }
        }

        static bool ValidateLogin(string username, string password, out string role)
        {
            role = string.Empty;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT role FROM users WHERE username = @username AND password = @password";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        role = reader.GetString("role");
                        return true;
                    }
                }
            }
            return false;
        }

        class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        class LoginResponse
        {
            public bool Success { get; set; }
            public string Role { get; set; }
        }
    }
}
