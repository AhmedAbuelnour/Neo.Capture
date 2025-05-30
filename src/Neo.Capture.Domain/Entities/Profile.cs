using LowCodeHub.QueryableExtensions.Entities;

namespace Neo.Capture.Domain.Entities
{
    public class Profile : TrackedBaseEntity<Guid>
    {
        public string PhoneNumber { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public ICollection<ProfileLocation> Locations { get; set; }
    }
}
