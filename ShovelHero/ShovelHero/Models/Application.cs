namespace ShovelHero.Models
{
    public class Application
    {
        public Guid Id { get; init; }
        public Guid DemandId { get; init; }
        public string Name { get; init; } = default!;
        public string Phone { get; init; } = default!;
        public string AvailableTime { get; init; } = default!;
        public string Skills { get; init; } = default!;
        public string Tools { get; init; } = default!;
        public DateTime CreatedAt { get; init; }
    }
}
