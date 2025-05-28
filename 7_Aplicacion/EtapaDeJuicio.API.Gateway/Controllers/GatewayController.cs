using Microsoft.AspNetCore.Mvc;

namespace EtapaDeJuicio.API.Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public GatewayController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Obtiene información sobre los microservicios disponibles
    /// </summary>
    [HttpGet("services")]
    public IActionResult GetAvailableServices()
    {
        var services = new
        {
            Gateway = new
            {
                Name = "API Gateway",
                Version = "1.0.0",
                Description = "Gateway unificado para el sistema de Etapa de Juicio"
            },
            Microservices = new[]
            {
                new
                {
                    Name = "Gestor de Audiencias",
                    BaseUrl = _configuration["Microservices:GestorDeAudiencias:BaseUrl"],
                    Endpoints = new[]
                    {
                        "GET /api/audiencias - Obtener audiencias",
                        "POST /api/audiencias - Crear audiencia",
                        "GET /api/audiencias/{id} - Obtener audiencia por ID",
                        "PUT /api/audiencias/{id} - Actualizar audiencia",
                        "DELETE /api/audiencias/{id} - Eliminar audiencia"
                    }
                },
                new
                {
                    Name = "Gestor de Usuario",
                    BaseUrl = _configuration["Microservices:GestorDeUsuario:BaseUrl"],
                    Endpoints = new[]
                    {
                        "GET /api/usuarios - Obtener usuarios",
                        "POST /api/usuarios - Crear usuario",
                        "GET /api/usuarios/{id} - Obtener usuario por ID"
                    }
                },
                new
                {
                    Name = "Gestor de Interrogatorios",
                    BaseUrl = _configuration["Microservices:GestorDeInterrogatorios:BaseUrl"],
                    Endpoints = new[]
                    {
                        "GET /api/interrogatorios - Obtener interrogatorios",
                        "POST /api/interrogatorios - Crear interrogatorio"
                    }
                },
                new
                {
                    Name = "Gestor de Sentencias",
                    BaseUrl = _configuration["Microservices:GestorDeSentencias:BaseUrl"],
                    Endpoints = new[]
                    {
                        "GET /api/sentencias - Obtener sentencias",
                        "POST /api/sentencias - Crear sentencia"
                    }
                },
                new
                {
                    Name = "Gestor de Pruebas",
                    BaseUrl = _configuration["Microservices:GestorDePruebas:BaseUrl"],
                    Endpoints = new[]
                    {
                        "GET /api/pruebas - Obtener pruebas",
                        "POST /api/pruebas - Crear prueba"
                    }
                }
            }
        };

        return Ok(services);
    }

    /// <summary>
    /// Verifica el estado de salud del API Gateway
    /// </summary>
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "API Gateway",
            Version = "1.0.0"
        });
    }

    /// <summary>
    /// Obtiene información sobre las rutas configuradas
    /// </summary>
    [HttpGet("routes")]
    public IActionResult GetRoutes()
    {
        var routes = new
        {
            Routes = new[]
            {
                new { Path = "/api/audiencias/**", Target = "Gestor de Audiencias", Port = "7001" },
                new { Path = "/api/usuarios/**", Target = "Gestor de Usuario", Port = "7002" },
                new { Path = "/api/interrogatorios/**", Target = "Gestor de Interrogatorios", Port = "7003" },
                new { Path = "/api/sentencias/**", Target = "Gestor de Sentencias", Port = "7004" },
                new { Path = "/api/pruebas/**", Target = "Gestor de Pruebas", Port = "7005" }
            },
            Description = "Rutas configuradas en el API Gateway"
        };

        return Ok(routes);
    }
}
