﻿using System;
using System.Collections.Generic;
using DAL;
using Persistence;
using MySql.Data.MySqlClient;

namespace BL
{
    public class UserService
    {
        private readonly UserRepository _repository;
        private readonly UserDbContext _dbContext;
        private ComputerService computerService;

        public UserService()
        {
            _repository = new UserRepository();
            _dbContext = new UserDbContext();
            computerService = new ComputerService();
        }

        public ServiceResult<string> Login(string username, string password)
        {
            try
            {
                var (isValid, role) = _repository.ValidateLogin(username, password);

                if (isValid)
                {
                    return ServiceResult<string>.Ok(role);
                }

                return ServiceResult<string>.Fail("Invalid username or password.");
            }
            catch (Exception ex)
            {
                // Ghi log lỗi tại đây nếu cần
                return ServiceResult<string>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public void UpdateUserBalance(string username, decimal amount)
        {
           _repository.UpdateBalance(username, amount);
        }

        public decimal GetBalance(string username)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();
        
            const string query = "SELECT balance FROM users WHERE username = @username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
        
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetDecimal("balance");
            }
        
            throw new Exception("User not found.");
        }

        public User GetUserByUsername(string username)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "SELECT * FROM users WHERE username = @username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
            return new User
            {
              Id = reader.GetInt32("id"),
              Username = reader.GetString("username"),
              Balance = reader.GetDecimal("balance"),
              ComputerId = reader.IsDBNull(reader.GetOrdinal("computer_id")) ? 0 : reader.GetInt32("computer_id")
            };
            }

            return null;
        }

        public void RegisterUser(User user)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "INSERT INTO users (username, password, role) VALUES (@username, @password, @role)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@password", user.Password);
            command.Parameters.AddWithValue("@role", user.Role);

            command.ExecuteNonQuery();
        }

        public void SelectComputer(string username)
        {
            while (true)
            {
            Console.WriteLine("Available Computers:");
            var computers = computerService.GetAllComputers();
            foreach (var computer in computers)
            {
                if (!computer.IsOn)
                {
                    Console.WriteLine($"ID: {computer.Id}, Name: {computer.Name}, Status: Off");
                }
            }

            Console.Write("Enter Computer ID to select: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var computer = computerService.GetComputerById(id);
                if (computer != null && !computer.IsOn)
                {
                    computer.IsOn = true;
                    computer.CurrentUser = username;
                    computer.OnTime = DateTime.Now;
                    computerService.UpdateComputer(computer);

                    // Cập nhật ComputerId cho người dùng
                    UpdateUserComputerId(username, id);

                    Console.WriteLine($"Computer {computer.Name} selected successfully.");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid selection. Please select an off computer.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID.");
            }
            }
        }

        public void Logout(string username)
        {
            var computers = computerService.GetAllComputers();
            foreach (var computer in computers)
            {
                if (computer.CurrentUser == username)
                {
                    computer.IsOn = false;
                    computer.CurrentUser = null;
                    computer.OnTime = null;
                    computerService.UpdateComputer(computer);
                    Console.WriteLine($"User {username} logged out. Computer {computer.Name} is now off.");
                    break;
                }
            }
            // Đặt lại ComputerId của người dùng thành null hoặc giá trị mặc định
            UpdateUserComputerId(username, null);
        }

        private void UpdateUserComputerId(string username, int? computerId)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "UPDATE users SET computer_id = @computer_id WHERE username = @username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@computer_id", computerId.HasValue ? (object)computerId.Value : DBNull.Value);
            command.Parameters.AddWithValue("@username", username);

            command.ExecuteNonQuery();
        }

    }
}
