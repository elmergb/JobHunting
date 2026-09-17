using JobHunting.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace JobHunting.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Domain rule violation: {Message}", ex.Message);
                await WriteErrorResponse(context, StatusCodes.Status400BadRequest, "domain_error", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await WriteErrorResponse(context, StatusCodes.Status500InternalServerError, "server_error", "An unexpected error occurred.");
            }
        }

        private static async Task WriteErrorResponse(HttpContext context, int statusCode, string code, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var body = new
            {
                type = code,
                message,
                statusCode
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(body, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                })
            );
        }
    }
}
