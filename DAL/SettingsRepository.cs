using MySql.Data.MySqlClient;
using Persistence;

namespace DAL
{
    public class SettingsRepository
    {
        private readonly UserDbContext _dbContext;

        public SettingsRepository()
        {
            _dbContext = new UserDbContext();
        }

        public decimal GetRatePerHour()
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "SELECT value FROM settings WHERE key_name = 'rate_per_hour'";
            using var command = new MySqlCommand(query, connection);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetDecimal("value");
            }

            throw new Exception("Rate per hour setting not found.");
        }

        public void UpdateRatePerHour(decimal newRate)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "UPDATE settings SET value = @value WHERE key_name = 'rate_per_hour'";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@value", newRate);

            command.ExecuteNonQuery();
        }
    }
}
