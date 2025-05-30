using ErrorOr;
using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using LowCodeHub.MinimalEndpoints.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Neo.Capture.Application.Interfaces.Services;

namespace Neo.Capture.Application.Features.GenerateReport
{
    public sealed class GenerateReportEndpoint(IReportService _reportService) : IMinimalEndpoint<string>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapGet("/download", Handle)
               .Produces(200)
               .AddLogging<GenerateReportEndpoint>()
               .WithName("GenerateReport");
        }

        public async ValueTask<IResult> Handle([FromQuery(Name = "phoneNumber")] string phoneNumber, CancellationToken cancellationToken)
        {
            ErrorOr<ReportFile> generateReportResult = await _reportService.GenerateReportAsync(phoneNumber, cancellationToken);

            if (generateReportResult.IsError)
            {
                return TypedResults.UnprocessableEntity(EndpointResult.Failure(generateReportResult.FirstError));
            }

            return TypedResults.File(generateReportResult.Value.Content, "application/zip", generateReportResult.Value.FileName);
        }
    }
}
