using MediatR;
using Microsoft.AspNetCore.Mvc;
using EtapaDeJuicio.Domain.Entities;
using EtapaDeJuicio.Domain.ValueObjects;

namespace EtapaDeJuicio.GestorDeInterrogatorios.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InterrogatoriosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<InterrogatoriosController> _logger;

    public InterrogatoriosController(IMediator mediator, ILogger<InterrogatoriosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Inicia un interrogatorio a un testigo
    /// </summary>
    [HttpPost("testigo")]
    public async Task<ActionResult<Guid>> IniciarInterrogatorioTestigo([FromBody] IniciarInterrogatorioTestigoRequest request)
    {
        try
        {
            var interrogatorioId = Guid.NewGuid();
            
            _logger.LogInformation(
                "Iniciando interrogatorio a testigo {Testigo} en audiencia {AudienciaId} por {Interrogador}",
                request.TestigoId, request.AudienciaId, request.InterrogadorId);

            // TODO: Implementar comando para crear interrogatorio
            
            return CreatedAtAction(nameof(ObtenerInterrogatorio), new { id = interrogatorioId }, interrogatorioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar interrogatorio a testigo");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Inicia un interrogatorio al acusado
    /// </summary>
    [HttpPost("acusado")]
    public async Task<ActionResult<Guid>> IniciarInterrogatorioAcusado([FromBody] IniciarInterrogatorioAcusadoRequest request)
    {
        try
        {
            var interrogatorioId = Guid.NewGuid();
            
            _logger.LogInformation(
                "Iniciando interrogatorio a acusado {AcusadoId} en audiencia {AudienciaId} por {Interrogador}",
                request.AcusadoId, request.AudienciaId, request.InterrogadorId);

            return CreatedAtAction(nameof(ObtenerInterrogatorio), new { id = interrogatorioId }, interrogatorioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar interrogatorio al acusado");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Realiza una pregunta en el interrogatorio
    /// </summary>
    [HttpPost("{id}/preguntas")]
    public async Task<ActionResult> RealizarPregunta(Guid id, [FromBody] RealizarPreguntaRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Pregunta realizada en interrogatorio {InterrogatorioId}: {Pregunta}",
                id, request.Pregunta);

            // TODO: Implementar comando para agregar pregunta al interrogatorio
            
            return Ok($"Pregunta agregada al interrogatorio {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar pregunta en interrogatorio {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Registra una respuesta en el interrogatorio
    /// </summary>
    [HttpPost("{id}/respuestas")]
    public async Task<ActionResult> RegistrarRespuesta(Guid id, [FromBody] RegistrarRespuestaRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Respuesta registrada en interrogatorio {InterrogatorioId}: {Respuesta}",
                id, request.Respuesta);

            return Ok($"Respuesta registrada en interrogatorio {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar respuesta en interrogatorio {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Finaliza un interrogatorio
    /// </summary>
    [HttpPost("{id}/finalizar")]
    public async Task<ActionResult> FinalizarInterrogatorio(Guid id)
    {
        try
        {
            _logger.LogInformation("Finalizando interrogatorio {InterrogatorioId}", id);
            
            return Ok($"Interrogatorio {id} finalizado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al finalizar interrogatorio {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un interrogatorio por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<InterrogatorioDto>> ObtenerInterrogatorio(Guid id)
    {
        try
        {
            // TODO: Implementar query para obtener interrogatorio
            _logger.LogInformation("Solicitud de interrogatorio con ID: {Id}", id);
            return NotFound($"Interrogatorio con ID {id} no encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener interrogatorio con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene todos los interrogatorios de una audiencia
    /// </summary>
    [HttpGet("audiencia/{audienciaId}")]
    public async Task<ActionResult<IEnumerable<InterrogatorioDto>>> ObtenerInterrogatoriosPorAudiencia(Guid audienciaId)
    {
        try
        {
            _logger.LogInformation("Solicitud de interrogatorios para audiencia: {AudienciaId}", audienciaId);
            return Ok(new List<InterrogatorioDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener interrogatorios para audiencia: {AudienciaId}", audienciaId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Realiza una objeción durante el interrogatorio
    /// </summary>
    [HttpPost("{id}/objeciones")]
    public async Task<ActionResult> RealizarObjecion(Guid id, [FromBody] RealizarObjecionRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Objeción realizada en interrogatorio {InterrogatorioId} por {ObjetorId}: {Motivo}",
                id, request.ObjetorId, request.Motivo);

            return Ok($"Objeción registrada en interrogatorio {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar objeción en interrogatorio {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Obtiene todos los interrogatorios
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InterrogatorioDto>>> ObtenerTodosLosInterrogatorios()
    {
        try
        {
            _logger.LogInformation("Solicitud de todos los interrogatorios");
            
            // TODO: Implementar query para obtener todos los interrogatorios
            // Por ahora retornamos una lista vacía
            return Ok(new List<InterrogatorioDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los interrogatorios");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea un nuevo interrogatorio genérico
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CrearInterrogatorio([FromBody] CrearInterrogatorioRequest request)
    {
        try
        {
            var interrogatorioId = request.Id != Guid.Empty ? request.Id : Guid.NewGuid();
            
            _logger.LogInformation(
                "Creando interrogatorio genérico {InterrogatorioId} con pregunta: {Pregunta}",
                interrogatorioId, request.Pregunta);

            // TODO: Implementar comando para crear interrogatorio genérico
            // Por ahora retornamos el ID creado
            
            return CreatedAtAction(nameof(ObtenerInterrogatorio), new { id = interrogatorioId }, interrogatorioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear interrogatorio genérico");
            return StatusCode(500, "Error interno del servidor");
        }
    }
}

// DTOs para las requests
public record IniciarInterrogatorioTestigoRequest(Guid AudienciaId, Guid TestigoId, Guid InterrogadorId, string TipoInterrogador);
public record IniciarInterrogatorioAcusadoRequest(Guid AudienciaId, Guid AcusadoId, Guid InterrogadorId, string TipoInterrogador);
public record RealizarPreguntaRequest(string Pregunta, Guid PreguntadoPor, DateTime Timestamp);
public record RegistrarRespuestaRequest(string Respuesta, DateTime Timestamp, string Observaciones);
public record RealizarObjecionRequest(Guid ObjetorId, string Motivo, DateTime Timestamp);
public record CrearInterrogatorioRequest(Guid Id, string? Pregunta, string? Respuesta, DateTime FechaHora, string? Tipo);

// DTO para respuesta
public record InterrogatorioDto(
    Guid Id,
    Guid AudienciaId,
    Guid InterrogadoId,
    string TipoInterrogado,
    Guid InterrogadorId,
    string TipoInterrogador,
    DateTime FechaInicio,
    DateTime? FechaFin,
    string Estado,
    List<PreguntaRespuestaDto> PreguntasRespuestas);

public record PreguntaRespuestaDto(
    string Pregunta,
    string Respuesta,
    DateTime TimestampPregunta,
    DateTime? TimestampRespuesta,
    Guid PreguntadoPor);
