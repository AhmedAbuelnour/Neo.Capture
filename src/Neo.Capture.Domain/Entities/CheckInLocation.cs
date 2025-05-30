using LowCodeHub.QueryableExtensions.Entities;

namespace Neo.Capture.Domain.Entities
{
    public class CheckInLocation : TrackedBaseEntity<Guid>
    {
        public Guid ProfileLocationId { get; set; }
        public required string LocationName { get; set; }
        public string[]? ImageUrls { get; set; }  // URLs stored in Google Cloud Storage
        public required ProfileLocation ProfileLocation { get; set; }
    }
}
