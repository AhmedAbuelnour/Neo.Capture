using System.IO.Compression;
using System.Text;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Neo.Capture.Application.Interfaces.Repositories;
using Neo.Capture.Application.Interfaces.Services;
using Neo.Capture.Domain.Entities;

namespace Neo.Capture.Infrastructure.Implementations.Services
{
    public sealed class ReportService(
        NeoCaptureDbContext _dbContext,
        IProfileRepository _profileRepo,
        ICloudStorageService _storageService) : IReportService
    {
        public async Task<ErrorOr<ReportFile>> GenerateReportAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            string normalized = NormalizePhoneNumber(phoneNumber);

            Profile? profile = await _profileRepo.GetByPhoneNumberAsync(normalized, cancellationToken);
            if (profile is null)
            {
                return Error.NotFound("user_not_found", "User not found.");
            }

            var checkIns = await _dbContext.CheckInLocations
                .Include(c => c.ProfileLocation)
                .Where(c => c.ProfileLocation.ProfileId == profile.Id)
                .ToListAsync(cancellationToken);

            using MemoryStream zipStream = new();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                StringBuilder csv = new();
                csv.AppendLine("FileName,Latitude,Longitude");
                int index = 1;

                foreach (var checkIn in checkIns)
                {
                    if (checkIn.ImageUrls is null)
                        continue;

                    foreach (var url in checkIn.ImageUrls)
                    {
                        if (string.IsNullOrWhiteSpace(url))
                            continue;
                        var (bucket, name) = ParseGsUri(url);
                        byte[] data = await _storageService.DownloadFileAsync(bucket, name, cancellationToken);
                        string extension = Path.GetExtension(name);
                        string entryName = $"image_{index}{extension}";
                        var entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);
                        await using (var entryStream = entry.Open())
                        {
                            await entryStream.WriteAsync(data, cancellationToken);
                        }
                        csv.AppendLine($"{entryName},{checkIn.ProfileLocation.Latitude},{checkIn.ProfileLocation.Longitude}");
                        index++;
                    }
                }

                var csvEntry = archive.CreateEntry("locations.csv", CompressionLevel.Fastest);
                await using (var writer = new StreamWriter(csvEntry.Open(), Encoding.UTF8))
                {
                    await writer.WriteAsync(csv.ToString());
                }
            }

            return new ReportFile(zipStream.ToArray(), $"report_{normalized}.zip");
        }

        private static (string bucket, string name) ParseGsUri(string uri)
        {
            if (uri.StartsWith("gs://"))
            {
                string without = uri[5..];
                int idx = without.IndexOf('/');
                if (idx > -1)
                {
                    return (without[..idx], without[(idx + 1)..]);
                }
            }
            throw new ArgumentException("Invalid Google Storage url", nameof(uri));
        }

        private static string NormalizePhoneNumber(string phoneNumber)
        {
            return phoneNumber.StartsWith("+966") ? phoneNumber[4..] : phoneNumber.StartsWith("0") ? phoneNumber[1..] : phoneNumber;
        }
    }
}
