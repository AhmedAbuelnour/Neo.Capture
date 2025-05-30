using Microsoft.EntityFrameworkCore;
using Neo.Capture.Application.Interfaces.Repositories;
using Neo.Capture.Domain.Entities;

namespace Neo.Capture.Infrastructure.Implementations.Repositories
{
    public sealed class ProfileRepository(NeoCaptureDbContext _dbContext) : IProfileRepository
    {
        public async Task<Profile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken)
        {
            return await _dbContext.Set<Profile>().Where(a => a.Id == profileId).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<Profile?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Profile>().Where(a => a.PhoneNumber == phoneNumber).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> RegisterAsync(Profile profile, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<Profile>().Add(profile);

            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
