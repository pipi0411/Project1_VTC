using System;
using MySql.Data.MySqlClient;
using Persistence;

namespace BL
{
    public class SessionService
    {
        private readonly UserDbContext _dbContext;

        public SessionService()
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

        public decimal EndSession(string username, decimal ratePerHour)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string selectQuery = "SELECT id, start_time FROM sessions WHERE username = @username AND end_time IS NULL";
            using var selectCommand = new MySqlCommand(selectQuery, connection);
            selectCommand.Parameters.AddWithValue("@username", username);

            using var reader = selectCommand.ExecuteReader();
            if (!reader.Read())
            {
                throw new Exception("No active session found.");
            }

            var sessionId = reader.GetInt32("id");
            var startTime = reader.GetDateTime("start_time");
            reader.Close();

            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            var hours = (decimal)duration.TotalHours;

            const string updateQuery = "UPDATE sessions SET end_time = @end_time WHERE id = @id";
            using var updateCommand = new MySqlCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@end_time", endTime);
            updateCommand.Parameters.AddWithValue("@id", sessionId);

            updateCommand.ExecuteNonQuery();

            return hours * ratePerHour;
        }

        public bool IsSessionActive(string username)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "SELECT COUNT(*) FROM sessions WHERE username = @username AND end_time IS NULL";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);

            var count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }
        public DateTime GetSessionStartTime(string username)
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

            throw new Exception("No active session found.");
        }
    }
}