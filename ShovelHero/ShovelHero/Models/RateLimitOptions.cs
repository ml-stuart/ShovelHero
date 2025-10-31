namespace ShovelHero.Models
{
    public class RateLimitOptions
    {
        public int RequestLimit { get; set; } = 5;
        public int TimeWindowMinutes { get; set; } = 1;
        public bool Enabled { get; set; } = true;
    }
}
