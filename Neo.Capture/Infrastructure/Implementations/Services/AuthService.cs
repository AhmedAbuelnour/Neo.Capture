using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Neo.Capture.Application.Features.Login;
using Neo.Capture.Application.Features.Register;
using Neo.Capture.Application.Interfaces.Repositories;
using Neo.Capture.Application.Interfaces.Services;
using Neo.Capture.Domain.Entities;

namespace Neo.Capture.Infrastructure.Implementations.Services
{
    public class AuthService(IProfileRepository _profileRepo, IPasswordHasher<Profile> passwordHasher) : IAuthService
    {
        public async Task<ErrorOr<Profile>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            // remove the leading +966 or 0 from the phone number for comparison
            string normalizedPhoneNumber = GetNormalizedPhoneNumber(request.PhoneNumber);

            if (await _profileRepo.GetByPhoneNumberAsync(normalizedPhoneNumber, cancellationToken) is not null)
            {
                return Error.Conflict("user_already_existed", "Phone number already registered.");
            }

            Profile profile = new()
            {
                PhoneNumber = normalizedPhoneNumber,
                Id = Guid.CreateVersion7(),
                CreatedBy = "USER"
            };

            profile.PasswordHash = passwordHasher.HashPassword(profile, request.Password);

            if (await _profileRepo.RegisterAsync(profile, cancellationToken) <= 0)
            {
                return Error.Conflict("registration_failed", "Failed to register the user.");

            }

            return profile;
        }

        public async Task<ErrorOr<Profile>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            string normalizedPhoneNumber = GetNormalizedPhoneNumber(request.PhoneNumber);

            if (await _profileRepo.GetByPhoneNumberAsync(normalizedPhoneNumber, cancellationToken) is Profile profile)
            {
                if (passwordHasher.VerifyHashedPassword(profile, profile.PasswordHash, request.Password) == PasswordVerificationResult.Success)
                {
                    return profile;
                }
            }

            return Error.NotFound("user_not_found", "User not found.");
        }

        private static string GetNormalizedPhoneNumber(string phoneNumber)
        {
            return phoneNumber.StartsWith("+966") ? phoneNumber[4..] : phoneNumber.StartsWith("0") ? phoneNumber[1..] : phoneNumber;
        }
    }
}
