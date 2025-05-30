using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Neo.Capture.Application.Interfaces.Services;
using Neo.Capture.Domain.Operation;


namespace Neo.Capture.Application.Features.UploadAttachment
{
    public sealed class UploadAttachmentEndpoint(ICloudStorageService _storageService) : IMinimalEndpoint<IFormFile>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("/upload", Handle)
               .Accepts<IFormFile>("multipart/form-data")
               .Produces<EndpointResult<string>>(200)
               .AddLogging<UploadAttachmentEndpoint>()
               .WithName("UploadAttachment")
               .DisableAntiforgery();
        }

        public async ValueTask<IResult> Handle([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            return TypedResults.Ok(new EndpointResult<string>
            {
                IsSuccess = true,
                Value = await _storageService.UploadFileAsync("neo-capture-bucket", file, cancellationToken)
            });
        }
    }
}
