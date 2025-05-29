using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using Neo.Capture.Application.Features.AddLocation;

namespace Neo.Capture.Apis
{
    public class LocationModules : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/api/location")
                            .WithTags("Location")
                            .WithGroupName("neo-capture")
                            .WithOpenApi();

            group.MapEndpoint<AddLocationEndpoint>();
        }
    }
}
