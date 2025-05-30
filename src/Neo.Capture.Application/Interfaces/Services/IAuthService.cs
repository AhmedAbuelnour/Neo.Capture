using ErrorOr;
using Neo.Capture.Application.Features.Login;
using Neo.Capture.Application.Features.Register;
using Neo.Capture.Domain.Entities;

namespace Neo.Capture.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ErrorOr<Profile>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
        Task<ErrorOr<Profile>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    }
}
