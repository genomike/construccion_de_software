using System.Diagnostics;
using System.Text.Json;

namespace EtapaDeJuicio.API.Gateway.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();
        
        // Log de entrada
        _logger.LogInformation(
            "Request {RequestId} started: {Method} {Path} from {RemoteIpAddress}",
            requestId,
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        try
        {
            // Agregar el request ID a los headers de respuesta
            context.Response.Headers.Add("X-Request-ID", requestId);
            
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Request {RequestId} failed: {Method} {Path}",
                requestId,
                context.Request.Method,
                context.Request.Path);
            
            await HandleExceptionAsync(context, ex);
        }
        finally
        {
            stopwatch.Stop();
            
            _logger.LogInformation(
                "Request {RequestId} completed: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new
        {
            Error = new
            {
                Message = "Error interno del servidor",
                Detail = exception.Message,
                RequestId = context.Response.Headers["X-Request-ID"].FirstOrDefault(),
                Timestamp = DateTime.UtcNow
            }
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
