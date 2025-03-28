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

            const string query = "SELECT role, online FROM users WHERE username = @username AND password = @password";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", password);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                bool isOnline = reader.GetBoolean("online");
                if (isOnline)
                {
                    return (false, "User is already logged in.");
                }

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

        public void UpdateOnlineStatus(string username, bool isOnline)
        {
            using var connection = _dbContext.GetConnection();
            connection.Open();

            const string query = "UPDATE users SET online = @isOnline WHERE username = @username";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@isOnline", isOnline);

            command.ExecuteNonQuery();
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
               Role = reader.GetString("role"),
               Online = reader.GetBoolean("online")
             };
           }

          return null; // Trả về null nếu không tìm thấy người dùng
        }
    }
}

