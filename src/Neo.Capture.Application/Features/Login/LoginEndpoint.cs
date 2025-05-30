using ErrorOr;
using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using LowCodeHub.MinimalEndpoints.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Neo.Capture.Application.Features.Register;
using Neo.Capture.Application.Interfaces.Services;
using Neo.Capture.Application.Providers;
using Neo.Capture.Domain.Entities;
using System.Text.RegularExpressions;

namespace Neo.Capture.Application.Features.Login
{

    public record LoginRequest(string PhoneNumber, string Password);
    public record LoginResponse(string AccessToken);

    public class LoginValidator : IMinimalValidator<LoginRequest>
    {
        public IEnumerable<ValidationFailure> Validate(LoginRequest request)
        {

            if (!Regex.IsMatch(request.PhoneNumber, @"^(0)?5\d{8}$"))
            {
                yield return new ValidationFailure
                {
                    AttemptedValue = request.PhoneNumber,
                    ErrorCode = "invalid_phone_number",
                    ErrorMessage = "Phone number must be a valid Saudi number starting with 05.",
                    PropertyName = nameof(RegisterRequest.PhoneNumber)
                };
            }

            // check password length to be at least 8 to 16 characters
            // at least 1 number
            // at least 1 capital letter

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8 || request.Password.Length > 16 || !Regex.IsMatch(request.Password, @"^(?=.*[A-Z])(?=.*\d).+$"))
            {
                yield return new ValidationFailure
                {
                    AttemptedValue = request.Password,
                    ErrorCode = "invalid_password",
                    ErrorMessage = "Password must be between 8 and 16 characters long, contain at least one number and one capital letter.",
                    PropertyName = nameof(RegisterRequest.Password)
                };
            }
        }
    }

    public class LoginEndpoint(IAuthService _authService, JwtTokenProvider jwtTokenProvider) : IMinimalEndpoint<LoginRequest>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("/login", Handle)
               .Accepts<LoginRequest>("application/json")
               .Produces<EndpointResult<LoginResponse>>(200)
               .AddValidator<LoginValidator>()
               .AddLogging<LoginEndpoint>()
               .WithName("Login");
        }

        public async ValueTask<IResult> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            ErrorOr<Profile> loginResult = await _authService.LoginAsync(request, cancellationToken);

            if (loginResult.IsError)
            {
                return TypedResults.UnprocessableEntity(EndpointResult.Failure(loginResult.FirstError));
            }

            JwtResponse jwtResponse = await jwtTokenProvider.GetJwtAsync(loginResult.Value, cancellationToken);

            return TypedResults.Ok(EndpointResult<LoginResponse>.Success(new LoginResponse(jwtResponse.AccessToken)));
        }
    }
}
