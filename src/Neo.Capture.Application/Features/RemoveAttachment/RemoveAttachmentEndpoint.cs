using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using Microsoft.AspNetCore.Mvc;
using Neo.Capture.Application.Interfaces.Services;
using Neo.Capture.Domain.Operation;

namespace Neo.Capture.Application.Features.RemoveAttachment
{
    public sealed class RemoveAttachmentEndpoint(ICloudStorageService _storageService) : IMinimalEndpoint<string>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapDelete("/remove", Handle)
               .Produces<EndpointResult>(200)
               .AddLogging<RemoveAttachmentEndpoint>()
               .WithName("RemoveAttachment");
        }

        public async ValueTask<IResult> Handle([FromQuery(Name = "Url")] string attachmentUrl, CancellationToken cancellationToken)
        {
            return TypedResults.Ok(new EndpointResult
            {
                IsSuccess = await _storageService.RemoveFileAsync("neo-capture-bucket", attachmentUrl, cancellationToken),
            });
        }
    }
}
