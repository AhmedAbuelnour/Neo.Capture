using Neo.Common.UserProvider;
using Neo.Common.UserProvider.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Neo.Capture.Application.Interfaces.Services
{
    public class CurrentUserProvider(IHttpContextAccessor contextAccessor) : ICurrentUserProvider
    {
        public CurrentUser GetCurrentUser()
        {
            var userId = GetClaimValue(JwtRegisteredClaimNames.Sub);

            return new CurrentUser(userId, null, null, null, null);
        }

        public UserDevice GetUserDevice()
        {
            var httpContext = contextAccessor.HttpContext;

            if (httpContext == null)
            {
                return null;
            }
            var deviceId = httpContext.User.FindFirst("device_id")?.Value;
            var ipAddress = contextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();
            return new UserDevice(new Guid(deviceId ?? Guid.Empty.ToString()), null, ipAddress, null);

        }

        private string? GetClaimValue(string claimType)
        {
            return contextAccessor.HttpContext?.User.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
        }
    }
}
