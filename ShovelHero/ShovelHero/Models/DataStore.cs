namespace ShovelHero.Models
{
    public class DataStore
    {
        public List<Demand> Demands { get; set; } = new();
        public List<Application> Applications { get; set; } = new();

        public DataStore()
        {
            Demands.Add(new Demand
            {
                Id = Guid.NewGuid(),
                TaskType = "清理",
                AddressCode = "A-1",
                RequiredCount = 3,
                MeetingPoint = "捷運出口",
                RiskNote = "請攜帶手套",
                ContactInfo = "0912-345-678",
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
