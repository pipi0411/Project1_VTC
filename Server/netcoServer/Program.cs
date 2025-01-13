using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace TCPServer
{
    class Program
    {
        // Mock dữ liệu tài khoản
        private static Dictionary<string, (string Password, string Role)> users = new Dictionary<string, (string, string)>
        {
            { "admin", ("admin123", "admin") },
            { "user1", ("user123", "user") },
            { "user2", ("password123", "user") }
        };

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

                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

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

                client.Close();
            }
        }

        static bool ValidateLogin(string username, string password, out string role)
        {
            role = string.Empty;
            if (users.ContainsKey(username) && users[username].Password == password)
            {
                role = users[username].Role;
                return true;
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
