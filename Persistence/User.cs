namespace Persistence
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal Balance { get; set; }
        public int ComputerId { get; set; }
        public string Role { get; set; }
    }
}