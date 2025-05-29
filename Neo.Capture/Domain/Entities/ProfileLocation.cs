using LowCodeHub.QueryableExtensions.Entities;

namespace Neo.Capture.Domain.Entities
{
    public class ProfileLocation : TrackedBaseEntity<Guid>
    {
        public Guid ProfileId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Navigation property (optional if using ORM like EF Core)
        public Profile Profile { get; set; }
        public CheckInLocation CheckIn { get; set; }  // One-to-one
    }
}
