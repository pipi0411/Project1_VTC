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
public User GetUserByUsername(string username)
{
    using var connection = _dbContext.GetConnection();
    connection.Open();

    const string query = "SELECT id, username, password, role FROM users WHERE username = @username";
    using var command = new MySqlCommand(query, connection);
    command.Parameters.AddWithValue("@username", username);

    using var reader = command.ExecuteReader();
    if (reader.Read())
    {
        return new User
        {
            Id = reader.GetInt32("id"),
            Username = reader.GetString("username"),
            Password = reader.GetString("password"), // Lấy mật khẩu đã mã hóa
            Role = reader.GetString("role")
        };
    }

    return null;
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

