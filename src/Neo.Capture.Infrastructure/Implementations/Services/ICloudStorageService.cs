using Google;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Neo.Capture.Application.Interfaces.Services;

namespace Neo.Capture.Infrastructure.Implementations.Services
{
    public class GoogleCloudStorageService : ICloudStorageService
    {
        private readonly StorageClient _storageClient;

        public GoogleCloudStorageService(IConfiguration configuration)
        {
            //GoogleCredential googleCredential = GoogleCredential.FromJson(configuration["GoogleCloudStorage:CredentialsJson"]);

            //_storageClient = StorageClient.Create(googleCredential);
        }

        public GoogleCloudStorageService(StorageClient storageClient)
        {
            _storageClient = storageClient ?? throw new ArgumentNullException(nameof(storageClient));
        }

        /// <summary>
        /// Upload a file using the original filename
        /// </summary>
        public async Task<string> UploadFileAsync(string bucketName, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty", nameof(file));

            string fileName = $"{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")}_{file.FileName}";

            return await UploadFileAsync(bucketName, file, fileName, cancellationToken);
        }

        /// <summary>
        /// Upload a file with a custom filename
        /// </summary>
        public async Task<string> UploadFileAsync(string bucketName, IFormFile file, string customFileName, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty", nameof(file));

            if (string.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentException("Bucket name cannot be null or empty", nameof(bucketName));

            if (string.IsNullOrWhiteSpace(customFileName))
                throw new ArgumentException("File name cannot be null or empty", nameof(customFileName));

            try
            {
                using var stream = file.OpenReadStream();

                Google.Apis.Storage.v1.Data.Object objectOptions = new()
                {
                    Bucket = bucketName,
                    Name = customFileName,
                    ContentType = file.ContentType ?? "application/octet-stream"
                };

                var uploadedObject = await _storageClient.UploadObjectAsync(objectOptions, stream, cancellationToken: cancellationToken);

                // Return the GCS URI
                return $"gs://{bucketName}/{customFileName}";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to upload file {customFileName} to bucket {bucketName}", ex);
            }
        }

        /// <summary>
        /// Remove a file from the bucket
        /// </summary>
        public async Task<bool> RemoveFileAsync(string bucketName, string fileName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentException("Bucket name cannot be null or empty", nameof(bucketName));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

            try
            {
                await _storageClient.DeleteObjectAsync(bucketName, fileName, cancellationToken: cancellationToken);
                return true;
            }
            catch (GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // File doesn't exist
                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to remove file {fileName} from bucket {bucketName}", ex);
            }
        }

        /// <summary>
        /// Check if a file exists in the bucket
        /// </summary>
        public async Task<bool> FileExistsAsync(string bucketName, string fileName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(bucketName))
                throw new ArgumentException("Bucket name cannot be null or empty", nameof(bucketName));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

            try
            {
                await _storageClient.GetObjectAsync(bucketName, fileName, cancellationToken: cancellationToken);
                return true;
            }
            catch (GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to check if file {fileName} exists in bucket {bucketName}", ex);
            }
        }
    }
}
