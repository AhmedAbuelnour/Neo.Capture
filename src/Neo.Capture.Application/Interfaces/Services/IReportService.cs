using System.IO;

namespace Neo.Capture.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<Stream> GenerateReportAsync(string phoneNumber, CancellationToken cancellationToken = default);
    }
}
