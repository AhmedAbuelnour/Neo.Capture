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


namespace Neo.Capture.Application.Features.CheckIn
{
    public record class CheckInRequest(string Name, double Latitude, double Longitude, string[] ImageUrls);

    public class CheckInEndpoint(ILocationService _locationService, ICurrentUserProvider currentUserProvider) : IMinimalEndpoint<CheckInRequest>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("/check-in", Handle)
               .Accepts<CheckInRequest>("application/json")
               .Produces<EndpointResult>(200)
               .AddLogging<CheckInEndpoint>()
               .WithName("Check-in");
        }

        public async ValueTask<IResult> Handle(CheckInRequest request, CancellationToken cancellationToken)
        {
            CurrentUser currentUser = currentUserProvider.GetCurrentUser() ?? throw new UnauthorizedAccessException("User is not authenticated.");

            ErrorOr<Success> checkInResult = await _locationService.CheckInAsync(Guid.Parse(currentUser.UserId), request, cancellationToken);

            if (checkInResult.IsError)
            {
                return TypedResults.UnprocessableEntity(EndpointResult.Failure(checkInResult.FirstError));
            }

            return TypedResults.Ok(EndpointResult.Success());
        }
    }
}
