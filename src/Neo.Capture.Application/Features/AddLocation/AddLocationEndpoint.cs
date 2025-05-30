using ErrorOr;
using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using Neo.Capture.Application.Interfaces.Services;
using Neo.Capture.Domain.Operation;
using Neo.Common.UserProvider;
using Neo.Common.UserProvider.Models;

namespace Neo.Capture.Application.Features.AddLocation
{
    public record class AddLocationRequest(double Latitude, double Longitude);

    public class AddLocationEndpoint(ILocationService _locationService, ICurrentUserProvider currentUserProvider) : IMinimalEndpoint<AddLocationRequest>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("/add", Handle)
               .Accepts<AddLocationRequest>("application/json")
               .Produces<EndpointResult>(200)
               .AddLogging<AddLocationEndpoint>()
               .WithName("AddLocation");
        }

        public async ValueTask<IResult> Handle(AddLocationRequest request, CancellationToken cancellationToken)
        {
            CurrentUser currentUser = currentUserProvider.GetCurrentUser() ?? throw new UnauthorizedAccessException("User is not authenticated.");

            ErrorOr<Success> addLocationResult = await _locationService.AddAsync(Guid.Parse(currentUser.UserId), request, cancellationToken);

            if (addLocationResult.IsError)
            {
                return TypedResults.UnprocessableEntity(new EndpointResult
                {
                    IsSuccess = false,
                    ErrorCode = addLocationResult.FirstError.Code,
                    ErrorMessage = addLocationResult.FirstError.Description
                });
            }

            return TypedResults.Ok(new EndpointResult
            {
                IsSuccess = true,
            });
        }
    }
}
