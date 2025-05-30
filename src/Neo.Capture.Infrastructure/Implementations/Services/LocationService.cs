using ErrorOr;
using Neo.Capture.Application.Features.AddLocation;
using Neo.Capture.Application.Features.CheckIn;
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

        public async Task<ErrorOr<Success>> CheckInAsync(Guid profileId, CheckInRequest request, CancellationToken cancellationToken)
        {
            if (await _profileRepo.GetByIdAsync(profileId, cancellationToken) is not null)
            {
                if (await _locationRepo.CheckInAsync(new CheckInLocation
                {
                    LocationName = request.Name,
                    ProfileLocation = new ProfileLocation
                    {
                        ProfileId = profileId,
                        Latitude = request.Latitude,
                        Longitude = request.Longitude,
                        CreatedBy = "USER",
                    },
                    ImageUrls = request.ImageUrls,
                    CreatedBy = "USER",
                }, cancellationToken) > 0)
                {
                    return Result.Success;
                }
            }
            return Error.Conflict("check_in_failed", "Failed to check in.");
        }
    }
}
