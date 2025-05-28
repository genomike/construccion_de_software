using System.Text.Json;

namespace EtapaDeJuicio.API.Gateway.Middleware;

public class HealthCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HealthCheckMiddleware> _logger;

    public HealthCheckMiddleware(
        RequestDelegate next, 
        IConfiguration configuration, 
        IHttpClientFactory httpClientFactory,
        ILogger<HealthCheckMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await HandleHealthCheckAsync(context);
            return;
        }

        await _next(context);
    }

    private async Task HandleHealthCheckAsync(HttpContext context)
    {
        var healthChecks = new List<object>();
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        // Gateway health
        healthChecks.Add(new
        {
            Service = "API Gateway",
            Status = "Healthy",
            ResponseTime = "0ms",
            Timestamp = DateTime.UtcNow
        });

        // Check microservices
        var microservices = new[]
        {
            new { Name = "GestorDeAudiencias", ConfigKey = "Microservices:GestorDeAudiencias:BaseUrl" },
            new { Name = "GestorDeUsuario", ConfigKey = "Microservices:GestorDeUsuario:BaseUrl" },
            new { Name = "GestorDeInterrogatorios", ConfigKey = "Microservices:GestorDeInterrogatorios:BaseUrl" },
            new { Name = "GestorDeSentencias", ConfigKey = "Microservices:GestorDeSentencias:BaseUrl" },
            new { Name = "GestorDePruebas", ConfigKey = "Microservices:GestorDePruebas:BaseUrl" }
        };

        foreach (var microservice in microservices)
        {
            var baseUrl = _configuration[microservice.ConfigKey];
            if (string.IsNullOrEmpty(baseUrl))
            {
                healthChecks.Add(new
                {
                    Service = microservice.Name,
                    Status = "Unknown",
                    ResponseTime = "N/A",
                    Timestamp = DateTime.UtcNow,
                    Error = "Base URL not configured"
                });
                continue;
            }

            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                // Intentar conectar al microservicio
                var response = await client.GetAsync($"{baseUrl}/swagger/index.html");
                stopwatch.Stop();

                healthChecks.Add(new
                {
                    Service = microservice.Name,
                    Status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                    ResponseTime = $"{stopwatch.ElapsedMilliseconds}ms",
                    Timestamp = DateTime.UtcNow,
                    StatusCode = (int)response.StatusCode
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Health check failed for {ServiceName}", microservice.Name);
                
                healthChecks.Add(new
                {
                    Service = microservice.Name,
                    Status = "Unhealthy",
                    ResponseTime = "Timeout",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }

        var overallStatus = healthChecks.All(hc => ((dynamic)hc).Status == "Healthy") ? "Healthy" : "Degraded";

        var result = new
        {
            Status = overallStatus,
            Timestamp = DateTime.UtcNow,
            Services = healthChecks
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = overallStatus == "Healthy" ? 200 : 503;

        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}
