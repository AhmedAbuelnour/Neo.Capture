using Neo.Capture.Application.Interfaces.Repositories;
using Neo.Capture.Domain.Entities;

namespace Neo.Capture.Infrastructure.Implementations.Repositories
{
    public class LocationRepository(NeoCaptureDbContext _dbContext) : ILocationRepository
    {
        public async Task<int> AddAsync(ProfileLocation location, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<ProfileLocation>().Add(location);

            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> CheckInAsync(CheckInLocation checkInLocation, CancellationToken cancellationToken)
        {
            _dbContext.Set<CheckInLocation>().Add(checkInLocation);

            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
