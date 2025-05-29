using ErrorOr;
using Neo.Capture.Application.Features.AddLocation;

namespace Neo.Capture.Application.Interfaces.Services
{
    public interface ILocationService
    {
        Task<ErrorOr<Success>> AddAsync(Guid profileId, AddLocationRequest request, CancellationToken cancellationToken);

    }
}
