using ErrorOr;
using LowCodeHub.MinimalEndpoints.Abstractions;
using LowCodeHub.MinimalEndpoints.Extensions;
using LowCodeHub.MinimalEndpoints.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Neo.Capture.Application.Interfaces.Services;
using Neo.Capture.Application.Providers;
using Neo.Capture.Domain.Entities;
using Neo.Capture.Domain.Operation;
using System.Text.RegularExpressions;


namespace Neo.Capture.Application.Features.Register
{
    public record RegisterRequest(string PhoneNumber, string Password);
    public record RegisterResponse(string AccessToken);

    public class RegisterValidator : IMinimalValidator<RegisterRequest>
    {
        public IEnumerable<ValidationFailure> Validate(RegisterRequest request)
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
            // at least 1 capital leeter

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

    public class RegisterEndpoint(IAuthService _authService, JwtTokenProvider jwtTokenProvider) : IMinimalEndpoint<RegisterRequest>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("/register", Handle)
               .Accepts<RegisterRequest>("application/json")
               .Produces<EndpointResult<RegisterResponse>>(200)
               .AddValidator<RegisterRequest>()
               .AddLogging<RegisterEndpoint>()
               .WithName("Register");
        }

        public async ValueTask<IResult> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            ErrorOr<Profile> registerResults = await _authService.RegisterAsync(request, cancellationToken);

            if (registerResults.IsError)
            {
                return TypedResults.UnprocessableEntity(new EndpointResult
                {
                    IsSuccess = false,
                    ErrorCode = registerResults.FirstError.Code,
                    ErrorMessage = registerResults.FirstError.Description
                });
            }

            JwtResponse jwtResponse = await jwtTokenProvider.GetJwtAsync(registerResults.Value, cancellationToken);

            return TypedResults.Ok(new EndpointResult<RegisterResponse>
            {
                IsSuccess = true,
                Value = new RegisterResponse(jwtResponse.AccessToken),
            });
        }
    }
}
