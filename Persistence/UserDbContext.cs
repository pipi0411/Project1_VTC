using MySql.Data.MySqlClient;

namespace Persistence
{
    public class UserDbContext
    {
        private readonly string _connectionString;

        public UserDbContext()
        {
            _connectionString = "server=127.0.0.1;user=root;password=Duyanh0612;database=InternetCo"; // Replace with your actual connection string
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}


