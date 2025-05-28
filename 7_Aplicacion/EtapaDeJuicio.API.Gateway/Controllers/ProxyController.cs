using Microsoft.AspNetCore.Mvc;

namespace EtapaDeJuicio.API.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProxyController> _logger;

    public ProxyController(
        IHttpClientFactory httpClientFactory, 
        IConfiguration configuration,
        ILogger<ProxyController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Proxy directo para obtener audiencias del microservicio
    /// </summary>
    [HttpGet("audiencias")]
    public async Task<IActionResult> GetAudiencias([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["Microservices:GestorDeAudiencias:BaseUrl"];
            
            if (string.IsNullOrEmpty(baseUrl))
            {
                return StatusCode(503, new { Error = "Microservicio de audiencias no disponible" });
            }

            var response = await client.GetAsync($"{baseUrl}/api/audiencias?page={page}&pageSize={pageSize}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            return StatusCode((int)response.StatusCode, new { Error = "Error al comunicarse con el microservicio" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener audiencias");
            return StatusCode(500, new { Error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Ejemplo de agregación de datos de múltiples microservicios
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var dashboard = new
            {
                Timestamp = DateTime.UtcNow,
                Services = new List<object>()
            };

            var services = new[]
            {
                new { Name = "Audiencias", ConfigKey = "Microservices:GestorDeAudiencias:BaseUrl", Endpoint = "/api/audiencias" },
                new { Name = "Usuarios", ConfigKey = "Microservices:GestorDeUsuario:BaseUrl", Endpoint = "/api/usuarios" }
            };            var tasks = services.Select(async service =>
            {
                try
                {
                    var baseUrl = _configuration[service.ConfigKey];
                    if (string.IsNullOrEmpty(baseUrl))
                    {
                        return new { Service = service.Name, Status = "Unavailable", Data = (object?)null };
                    }

                    var response = await client.GetAsync($"{baseUrl}{service.Endpoint}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        return new { Service = service.Name, Status = "Available", Data = (object?)content };
                    }

                    return new { Service = service.Name, Status = "Error", Data = (object?)null };
                }
                catch
                {
                    return new { Service = service.Name, Status = "Error", Data = (object?)null };
                }
            }).ToArray();

            var results = await Task.WhenAll(tasks);

            return Ok(new
            {
                Dashboard = dashboard.Timestamp,
                Services = results,
                Summary = new
                {
                    TotalServices = results.Length,
                    AvailableServices = results.Count(r => r.Status == "Available"),
                    UnavailableServices = results.Count(r => r.Status != "Available")
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener dashboard");
            return StatusCode(500, new { Error = "Error interno del servidor" });
        }
    }
}
