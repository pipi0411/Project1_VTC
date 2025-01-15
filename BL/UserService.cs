using DAL;
using Persistence;
using MySql.Data.MySqlClient;

namespace BL
{
    public class UserService
    {
        private readonly UserRepository _repository;
        private readonly UserDbContext _dbContext;

        public UserService()
        {
            _repository = new UserRepository();
            _dbContext = new UserDbContext();
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

    }
}
