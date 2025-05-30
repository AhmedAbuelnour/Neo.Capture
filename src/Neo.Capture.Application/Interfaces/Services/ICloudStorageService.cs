using Microsoft.AspNetCore.Http;

namespace Neo.Capture.Application.Interfaces.Services
{
    public interface ICloudStorageService
    {
        Task<string> UploadFileAsync(string bucketName, IFormFile file, CancellationToken cancellationToken);
        Task<string> UploadFileAsync(string bucketName, IFormFile file, string customFileName, CancellationToken cancellationToken);
        Task<bool> RemoveFileAsync(string bucketName, string fileName, CancellationToken cancellationToken);
        Task<bool> FileExistsAsync(string bucketName, string fileName, CancellationToken cancellationToken);
        Task<byte[]> DownloadFileAsync(string bucketName, string fileName, CancellationToken cancellationToken);
    }
}
