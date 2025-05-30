using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Neo.Capture.Application.Exceptions
{
    public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "An exception occurred");

            int statusCode = GetStatusCodeForException(exception);

            httpContext.Response.StatusCode = statusCode;

            await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = ToProblem(httpContext, exception, statusCode)
            });

            return true;
        }

        private static ProblemDetails ToProblem(HttpContext httpContext, Exception exception, int statusCode)
        {
            return new ProblemDetails
            {
                Type = exception.GetType().Name,
                Status = statusCode,
                Title = GetTitleForStatusCode(statusCode),
                Detail = exception.Message,
                Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
                Extensions = new Dictionary<string, object?>
                {
                    { "traceId", httpContext.Features.Get<IHttpActivityFeature>()?.Activity?.Id },
                    { "timestamp", DateTime.UtcNow.ToString("o") },
                    { "userId", GetUserId(httpContext) },
                    { "userAgent", httpContext.Request.Headers.UserAgent.ToString() },
                    { "queryParams", httpContext.Request.Query.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()) }
                }
            };
        }

        private static int GetStatusCodeForException(Exception exception)
        {
            return exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private static string GetTitleForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest => "Bad GetApplicationGroupRequest",
                StatusCodes.Status401Unauthorized => "Unauthorized",
                StatusCodes.Status404NotFound => "Not Found",
                StatusCodes.Status500InternalServerError => "Internal Server Error",
                _ => "Error"
            };
        }

        private static string? GetUserId(HttpContext httpContext)
        {
            // Check for the user ID in claims
            var userId = httpContext.User.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;

            // If not found in claims, check headers
            if (string.IsNullOrEmpty(userId) && httpContext.Request.Headers.TryGetValue("sub", out var headerValue))
            {
                userId = headerValue.ToString();
            }

            return string.IsNullOrEmpty(userId) && httpContext.User.Identity?.IsAuthenticated == true
                ? httpContext.User.Identity.Name
                : userId;
        }
    }
}