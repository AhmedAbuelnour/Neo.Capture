using ErrorOr;
using Neo.Capture.Application.Features.AddLocation;
using Neo.Capture.Application.Interfaces.Repositories;
using Neo.Capture.Application.Interfaces.Services;
using Neo.Capture.Domain.Entities;

namespace Neo.Capture.Infrastructure.Implementations.Services
{
    public sealed class LocationService(IProfileRepository _profileRepo, ILocationRepository _locationRepo) : ILocationService
    {
        public async Task<ErrorOr<Success>> AddAsync(Guid profileId, AddLocationRequest request, CancellationToken cancellationToken)
        {
            if (await _profileRepo.GetByIdAsync(profileId, cancellationToken) is not null)
            {
                if (await _locationRepo.AddAsync(new ProfileLocation
                {
                    ProfileId = profileId,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    CreatedBy = "USER",
                }, cancellationToken) > 0)
                {
                    return Result.Success;
                }
            }

            return Error.Conflict("location_addition_failed", "Failed to add the location.");
        }
    }
}
