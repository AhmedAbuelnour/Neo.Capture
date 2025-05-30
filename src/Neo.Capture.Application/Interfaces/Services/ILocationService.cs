using ErrorOr;
using Neo.Capture.Application.Features.AddLocation;
using Neo.Capture.Application.Features.CheckIn;

namespace Neo.Capture.Application.Interfaces.Services
{
    public interface ILocationService
    {
        Task<ErrorOr<Success>> AddAsync(Guid profileId, AddLocationRequest request, CancellationToken cancellationToken);
        Task<ErrorOr<Success>> CheckInAsync(Guid profileId, CheckInRequest request, CancellationToken cancellationToken);
    }
}
