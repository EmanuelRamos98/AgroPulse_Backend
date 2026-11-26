using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AgroPulse.Backend.Middleware
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Excepción no controlada: {Message}", exception.Message);

            var (statusCode, title) = exception switch
            {
                ArgumentException => (HttpStatusCode.BadRequest, "Datos inválidos"),
                InvalidOperationException => (HttpStatusCode.Conflict, "Conflicto de operación"),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Recurso no encontrado"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "No autorizado"),
                _ => (HttpStatusCode.InternalServerError, "Error interno del servidor")
            };

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = statusCode == HttpStatusCode.InternalServerError
                    ? "Ocurrió un error inesperado. Intentá nuevamente más tarde."
                    : exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
