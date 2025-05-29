using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using Neo.Capture.Application.Features.Login;
using Neo.Capture.Application.Features.Register;

namespace Neo.Capture.Apis
{
    public class AuthModules : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/api/auth")
                             .WithTags("Auth")
                             .WithGroupName("neo-capture")
                             .WithOpenApi();

            group.MapEndpoint<RegisterEndpoint>();
            group.MapEndpoint<LoginEndpoint>();

        }
    }
}
