namespace ShovelHero.Models
{
    public class ApplicationCreateDto
    {
        public Guid DemandId { get; init; }
        public string Name { get; init; } = default!;
        public string Phone { get; init; } = default!;
        public string AvailableTime { get; init; } = default!;
        public string Skills { get; init; } = default!;
        public string Tools { get; init; } = default!;
    }
}
