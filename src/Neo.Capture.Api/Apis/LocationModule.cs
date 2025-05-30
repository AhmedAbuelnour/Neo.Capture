using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using Neo.Capture.Application.Features.AddLocation;
using Neo.Capture.Application.Features.CheckIn;

namespace Neo.Capture.Apis
{
    public class LocationModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/api/location")
                            .WithTags("Location")
                            .WithGroupName("neo-capture")
                            .WithOpenApi();

            group.MapEndpoint<AddLocationEndpoint>();

            group.MapEndpoint<CheckInEndpoint>();
        }
    }
}
