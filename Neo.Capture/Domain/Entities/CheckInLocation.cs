using LowCodeHub.QueryableExtensions.Entities;

namespace Neo.Capture.Domain.Entities
{
    public class CheckInLocation : TrackedBaseEntity<Guid>
    {
        public required Guid ProfileLocationId { get; set; }
        public required string LocationName { get; set; }
        public string[]? ImageUrls { get; set; }  // URLs stored in Google Cloud Storage
        public ProfileLocation ProfileLocation { get; set; }
    }
}
