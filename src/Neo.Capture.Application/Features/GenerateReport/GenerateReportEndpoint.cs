using ErrorOr;
using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Neo.Capture.Application.Interfaces.Services;
using Neo.Capture.Domain.Operation;

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
            ErrorOr<ReportFile> result = await _reportService.GenerateReportAsync(phoneNumber, cancellationToken);

            if (result.IsError)
            {
                return TypedResults.UnprocessableEntity(new EndpointResult
                {
                    IsSuccess = false,
                    ErrorCode = result.FirstError.Code,
                    ErrorMessage = result.FirstError.Description
                });
            }

            return TypedResults.File(result.Value.Content, "application/zip", result.Value.FileName);
        }
    }
}
