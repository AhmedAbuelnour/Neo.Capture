using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using Neo.Capture.Application.Features.RemoveAttachment;
using Neo.Capture.Application.Features.UploadAttachment;

namespace Neo.Capture.Api.Modules
{
    public class AttachmentModule : IModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            RouteGroupBuilder group = app.MapGroup("/api/attachments")
                              .WithTags("Attachments")
                              .WithGroupName("neo-capture")
                              .WithOpenApi();

            group.MapEndpoint<UploadAttachmentEndpoint>();

            group.MapEndpoint<RemoveAttachmentEndpoint>();
        }
    }
}
