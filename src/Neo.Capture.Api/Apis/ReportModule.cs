using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using Neo.Capture.Application.Features.GenerateReport;

namespace Neo.Capture.Apis
{
    public class ReportModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/api/report")
                                      .WithTags("Report")
                                      .WithGroupName("neo-capture")
                                      .WithOpenApi();

            group.MapEndpoint<GenerateReportEndpoint>();
        }
    }
}
