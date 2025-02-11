using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Persistence;

namespace BL 
{
public class ComputerService
{
    private string connectionString;

    public ComputerService()
    {
        // Initialize the connection string (replace with your actual connection string)
        connectionString = "server=127.0.0.1;user=root;database=InternetCo";
    }

    public Computer GetComputerById(int id)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        const string query = "SELECT * FROM computers WHERE id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Computer
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                IsOn = reader.GetBoolean("is_on"),
                CurrentUser = reader.IsDBNull(reader.GetOrdinal("current_users")) ? null : reader.GetString("current_users"),
                OnTime = reader.IsDBNull(reader.GetOrdinal("on_time")) ? (DateTime?)null : reader.GetDateTime("on_time")
            };
        }

        return null;
    }

    public List<Computer> GetAllComputers()
    {
        var computers = new List<Computer>();

        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        const string query = "SELECT * FROM computers";
        using var command = new MySqlCommand(query, connection);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            computers.Add(new Computer
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                IsOn = reader.GetBoolean("is_on"),
                CurrentUser = reader.IsDBNull(reader.GetOrdinal("current_users")) ? null : reader.GetString("current_users"),
                OnTime = reader.IsDBNull(reader.GetOrdinal("on_time")) ? (DateTime?)null : reader.GetDateTime("on_time")
            });
        }

        return computers;
    }

    public void UpdateComputer(Computer computer)
    {
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        const string query = "UPDATE computers SET name = @name, is_on = @is_on, current_users = @current_users, on_time = @on_time WHERE id = @id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", computer.Id);
        command.Parameters.AddWithValue("@name", computer.Name);
        command.Parameters.AddWithValue("@is_on", computer.IsOn);
        command.Parameters.AddWithValue("@current_users", computer.CurrentUser);
        command.Parameters.AddWithValue("@on_time", computer.OnTime);

        command.ExecuteNonQuery();
    }
}
}
