namespace Persistence
{
    public class Computer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOn { get; set; }
        public string CurrentUser { get; set; }
        public DateTime? OnTime { get; set; }
    }
}