using ErrorOr;
using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using LowCodeHub.MinimalEndpoints.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Neo.Capture.Application.Interfaces.Services;
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
                return TypedResults.UnprocessableEntity(EndpointResult.Failure(addLocationResult.FirstError));
            }

            return TypedResults.Ok(EndpointResult.Success());
        }
    }
}
