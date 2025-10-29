namespace ShovelHero.Models
{
    public class DemandCreateDto
    {
        public string TaskType { get; init; } = default!;
        public string AddressCode { get; init; } = default!;
        public int RequiredCount { get; init; }
        public string MeetingPoint { get; init; } = default!;
        public string RiskNote { get; init; } = default!;
        public string ContactInfo { get; init; } = default!;
    }
}
