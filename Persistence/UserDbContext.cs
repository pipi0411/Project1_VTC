using MySql.Data.MySqlClient;

namespace Persistence
{
    public class UserDbContext
    {
        private readonly string _connectionString;

        public UserDbContext()
        {
            _connectionString = "server=sql12.freesqldatabase.com;Database=sql12770067;User Id=sql12770067;Password=4j8VladhPb;Port=3306"; // Replace with your actual connection string
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}


