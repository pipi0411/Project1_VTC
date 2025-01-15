using MySql.Data.MySqlClient;
using Persistence;

namespace DAL
{
    public class UserRepository
    {
        private readonly UserDbContext _dbContext;

        public UserRepository()
        {
            _dbContext = new UserDbContext();
        }

        public (bool, string) ValidateLogin(string username, string password)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "SELECT role FROM users WHERE username = @username AND password = @password";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return (true, reader.GetString("role"));
            }

            return (false, null);
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

        public void UpdateBalance(string username, decimal amount)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "UPDATE users SET balance = balance + @amount WHERE username = @username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@amount", amount);

            command.ExecuteNonQuery();
        }
    }
}

