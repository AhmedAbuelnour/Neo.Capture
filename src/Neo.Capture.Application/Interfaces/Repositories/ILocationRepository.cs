using Neo.Capture.Domain.Entities;

namespace Neo.Capture.Application.Interfaces.Repositories
{
    public interface ILocationRepository
    {
        Task<int> AddAsync(ProfileLocation location, CancellationToken cancellationToken = default);
        Task<int> CheckInAsync(CheckInLocation checkInLocation, CancellationToken cancellationToken);
    }
}
