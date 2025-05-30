using System.IO;
using System.IO.Compression;
using System.Text;
using Neo.Capture.Application.Interfaces.Repositories;
using Neo.Capture.Application.Interfaces.Services;
using Neo.Capture.Domain.Entities;

namespace Neo.Capture.Infrastructure.Implementations.Services
{
    public class ReportService(IProfileRepository _profileRepo, ILocationRepository _locationRepo, ICloudStorageService _cloudStorageService) : IReportService
    {
        public async Task<Stream> GenerateReportAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            string normalizedPhone = phoneNumber.StartsWith("+966") ? phoneNumber[4..] : phoneNumber.StartsWith("0") ? phoneNumber[1..] : phoneNumber;

            Profile? profile = await _profileRepo.GetByPhoneNumberAsync(normalizedPhone, cancellationToken);
            if (profile is null)
            {
                throw new InvalidOperationException("User not found");
            }

            IReadOnlyList<CheckInLocation> checkIns = await _locationRepo.GetCheckInsByProfileAsync(profile.Id, cancellationToken);

            MemoryStream output = new();
            using (var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true))
            {
                StringBuilder csv = new();
                csv.AppendLine("ImageName,Latitude,Longitude");
                foreach (var checkIn in checkIns)
                {
                    if (checkIn.ImageUrls == null) continue;
                    foreach (string url in checkIn.ImageUrls)
                    {
                        string fileName = Path.GetFileName(url);
                        using Stream fileStream = await _cloudStorageService.DownloadFileAsync("neo-capture-bucket", url, cancellationToken);
                        var entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                        using var entryStream = entry.Open();
                        await fileStream.CopyToAsync(entryStream, cancellationToken);
                        csv.AppendLine($"{fileName},{checkIn.ProfileLocation.Latitude},{checkIn.ProfileLocation.Longitude}");
                    }
                }

                var csvEntry = archive.CreateEntry("locations.csv", CompressionLevel.Optimal);
                using var writer = new StreamWriter(csvEntry.Open());
                writer.Write(csv.ToString());
            }
            output.Position = 0;
            return output;
        }
    }
}
