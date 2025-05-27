using MediatR;
using Microsoft.AspNetCore.Mvc;
using EtapaDeJuicio.Domain.Entities;
using EtapaDeJuicio.Domain.ValueObjects;
using TipoPrueba = EtapaDeJuicio.Domain.Entities.Pruebas.TipoPrueba;
using EtapaDeJuicio.Domain.Entities.Pruebas;

namespace EtapaDeJuicio.GestorDePruebas.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PruebasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PruebasController> _logger;

    public PruebasController(IMediator mediator, ILogger<PruebasController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Presenta una prueba documental
    /// </summary>
    [HttpPost("documentales")]
    public async Task<ActionResult<Guid>> PresentarPruebaDocumental([FromBody] PresentarPruebaDocumentalRequest request)
    {
        try
        {
            // Crear la prueba documental
            var prueba = new Prueba(
                request.Descripcion,
                TipoPrueba.Documental,
                request.PresentadaPor
            );

            // TODO: Implementar comando para persistir la prueba
            _logger.LogInformation("Prueba documental presentada: {Descripcion}", request.Descripcion);
            
            return CreatedAtAction(nameof(ObtenerPrueba), new { id = prueba.Id }, prueba.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al presentar prueba documental");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Presenta una prueba material
    /// </summary>
    [HttpPost("materiales")]
    public async Task<ActionResult<Guid>> PresentarPruebaMaterial([FromBody] PresentarPruebaMaterialRequest request)
    {
        try
        {
            var prueba = new Prueba(
                request.Descripcion,
                TipoPrueba.Material,
                request.PresentadaPor
            );

            _logger.LogInformation("Prueba material presentada: {Descripcion}", request.Descripcion);
            
            return CreatedAtAction(nameof(ObtenerPrueba), new { id = prueba.Id }, prueba.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al presentar prueba material");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Presenta una prueba testimonial
    /// </summary>
    [HttpPost("testimoniales")]
    public async Task<ActionResult<Guid>> PresentarPruebaTestimonial([FromBody] PresentarPruebaTestimonialRequest request)
    {
        try
        {
            var prueba = new Prueba(
                request.Descripcion,
                TipoPrueba.Testimonial,
                request.PresentadaPor
            );

            _logger.LogInformation("Prueba testimonial presentada: {Descripcion}", request.Descripcion);
            
            return CreatedAtAction(nameof(ObtenerPrueba), new { id = prueba.Id }, prueba.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al presentar prueba testimonial");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Presenta una prueba pericial
    /// </summary>
    [HttpPost("periciales")]
    public async Task<ActionResult<Guid>> PresentarPruebaPericial([FromBody] PresentarPruebaPericialRequest request)
    {
        try
        {
            var prueba = new Prueba(
                request.Descripcion,
                TipoPrueba.Pericial,
                request.PresentadaPor
            );

            _logger.LogInformation("Prueba pericial presentada: {Descripcion}", request.Descripcion);
            
            return CreatedAtAction(nameof(ObtenerPrueba), new { id = prueba.Id }, prueba.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al presentar prueba pericial");
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Obtiene una prueba por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PruebaDto>> ObtenerPrueba(Guid id)
    {
        try
        {
            // TODO: Implementar query para obtener prueba por ID
            _logger.LogInformation("Solicitud de prueba con ID: {Id}", id);
            return NotFound($"Prueba con ID {id} no encontrada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener prueba con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene todas las pruebas de una audiencia
    /// </summary>
    [HttpGet("audiencia/{audienciaId}")]
    public async Task<ActionResult<IEnumerable<PruebaDto>>> ObtenerPruebasPorAudiencia(Guid audienciaId)
    {
        try
        {
            // TODO: Implementar query para obtener pruebas por audiencia
            _logger.LogInformation("Solicitud de pruebas para audiencia: {AudienciaId}", audienciaId);
            return Ok(new List<PruebaDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pruebas para audiencia: {AudienciaId}", audienciaId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Valida una prueba
    /// </summary>
    [HttpPost("{id}/validar")]
    public async Task<ActionResult> ValidarPrueba(Guid id, [FromBody] ValidarPruebaRequest request)
    {
        try
        {
            _logger.LogInformation("Validando prueba {Id} por {ValidadaPor}", id, request.ValidadaPor);
            return Ok($"Prueba {id} validada correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar prueba con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}

// DTOs para las requests
public record PresentarPruebaDocumentalRequest(string Descripcion, string PresentadaPor, string TipoDocumento);
public record PresentarPruebaMaterialRequest(string Descripcion, string PresentadaPor, string TipoMaterial);
public record PresentarPruebaTestimonialRequest(string Descripcion, string PresentadaPor, string Testigo);
public record PresentarPruebaPericialRequest(string Descripcion, string PresentadaPor, string Perito);
public record ValidarPruebaRequest(string ValidadaPor, bool EsValida, string Observaciones);

// Clase simple para uso interno del controlador
public class Prueba
{
    public Guid Id { get; }
    public string Descripcion { get; }
    public TipoPrueba Tipo { get; }
    public string PresentadaPor { get; }
    public DateTime FechaPresentacion { get; }

    public Prueba(string descripcion, TipoPrueba tipo, string presentadaPor)
    {
        Id = Guid.NewGuid();
        Descripcion = descripcion;
        Tipo = tipo;
        PresentadaPor = presentadaPor;
        FechaPresentacion = DateTime.UtcNow;
    }
}

// DTO para respuestas
public record PruebaDto(
    Guid Id,
    string Descripcion,
    DateTime FechaPresentacion,
    string Tipo,
    bool EsValida,
    string? PresentadaPor,
    string? Observaciones
);
