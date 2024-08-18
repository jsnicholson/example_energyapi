using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace Api.Exceptions
{
    // could use an exceptionfilterattribute if not all controllers want it
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError($"exception occurred | message:{exception.Message} trace:{exception.StackTrace}");

            if (_environment.IsDevelopment())
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsJsonAsync(new { }, cancellationToken);
            }
            else
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpContext.Response.WriteAsJsonAsync("An error has occurred", cancellationToken);
            }

            return true;
        }
    }
}