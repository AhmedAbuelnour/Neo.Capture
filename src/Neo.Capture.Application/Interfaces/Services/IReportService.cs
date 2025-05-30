using ErrorOr;
namespace Neo.Capture.Application.Interfaces.Services
{
    public record ReportFile(byte[] Content, string FileName);

    public interface IReportService
    {
        Task<ErrorOr<ReportFile>> GenerateReportAsync(string phoneNumber, CancellationToken cancellationToken);
    }
}
