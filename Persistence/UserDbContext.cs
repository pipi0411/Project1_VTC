﻿using MySql.Data.MySqlClient;

namespace Persistence
{
    public class UserDbContext
    {
        private readonly string _connectionString;

        public UserDbContext()
        {
            _connectionString = "server=sql12.freesqldatabase.com;Database=sql12792212;User Id=sql12792212;Password=4fMrp9ckSV;Port=3306"; // Replace with your actual connection string
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}


