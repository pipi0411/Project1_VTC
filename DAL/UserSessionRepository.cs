using System;
using MySql.Data.MySqlClient;
using Persistence;

namespace DAL
{
    public class UserSessionRepository
    {
        private readonly UserDbContext _dbContext;

        public UserSessionRepository()
        {
            _dbContext = new UserDbContext();
        }

        public void StartSession(string username)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "INSERT INTO sessions (username, start_time) VALUES (@username, @start_time)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@start_time", DateTime.Now);

            command.ExecuteNonQuery();
        }

        public DateTime EndSession(string username)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "UPDATE sessions SET end_time = @end_time WHERE username = @username AND end_time IS NULL";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@end_time", DateTime.Now);

            command.ExecuteNonQuery();

            return DateTime.Now;
        }

        public DateTime GetStartTime(string username)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "SELECT start_time FROM sessions WHERE username = @username AND end_time IS NULL";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetDateTime("start_time");
            }

            throw new Exception("No active session found for this user.");
        }
    }
}
