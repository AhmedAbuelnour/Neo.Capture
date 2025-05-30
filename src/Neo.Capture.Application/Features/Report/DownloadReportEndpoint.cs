using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using Microsoft.AspNetCore.Mvc;
using Neo.Capture.Application.Interfaces.Services;

namespace Neo.Capture.Application.Features.Report
{
    public record ReportRequest(string PhoneNumber);

    public class DownloadReportEndpoint(IReportService _reportService) : IMinimalEndpoint<ReportRequest>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapGet("/report/{phoneNumber}", Handle)
               .Produces<FileResult>(200)
               .AddLogging<DownloadReportEndpoint>()
               .WithName("DownloadReport");
        }

        public async ValueTask<IResult> Handle([FromRoute] ReportRequest request, CancellationToken cancellationToken)
        {
            Stream zipStream = await _reportService.GenerateReportAsync(request.PhoneNumber, cancellationToken);
            return Results.File(zipStream, "application/zip", "report.zip");
        }
    }
}
