using Neo.Capture.Domain.Entities;

namespace Neo.Capture.Application.Interfaces.Repositories
{
    public interface IProfileRepository
    {
        Task<Profile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken);
        Task<Profile?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
        Task<int> RegisterAsync(Profile profile, CancellationToken cancellationToken = default);

    }
}
