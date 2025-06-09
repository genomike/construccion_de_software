using MediatR;
using Microsoft.AspNetCore.Mvc;
using EtapaDeJuicio.Domain.Entities;
using EtapaDeJuicio.Domain.ValueObjects;
using EtapaDeJuicio.Application.Commands.Audiencias;
using EtapaDeJuicio.Domain.Entities.Audiencias;

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
    }    /// <summary>
    /// Inicia un interrogatorio a un testigo
    /// </summary>
    [HttpPost("testigo")]
    public async Task<ActionResult<Guid>> IniciarInterrogatorioTestigo([FromBody] IniciarInterrogatorioTestigoRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Iniciando interrogatorio a testigo {Testigo} en audiencia {AudienciaId} por {Interrogador}",
                request.TestigoId, request.AudienciaId, request.InterrogadorId);

            var descripcion = $"Interrogatorio a testigo {request.TestigoId} por {request.TipoInterrogador}";
            var observaciones = $"Interrogador: {request.InterrogadorId}, Tipo: {request.TipoInterrogador}";

            var command = new RegistrarActividadCommand(
                request.AudienciaId,
                descripcion,
                (int)TipoActividad.TestimonialDeclaraciones,
                observaciones
            );

            var result = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(ObtenerInterrogatorio), new { id = result.ActividadId }, result.ActividadId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar interrogatorio a testigo");
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Inicia un interrogatorio al acusado
    /// </summary>
    [HttpPost("acusado")]
    public async Task<ActionResult<Guid>> IniciarInterrogatorioAcusado([FromBody] IniciarInterrogatorioAcusadoRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Iniciando interrogatorio a acusado {AcusadoId} en audiencia {AudienciaId} por {Interrogador}",
                request.AcusadoId, request.AudienciaId, request.InterrogadorId);

            var descripcion = $"Interrogatorio a acusado {request.AcusadoId} por {request.TipoInterrogador}";
            var observaciones = $"Interrogador: {request.InterrogadorId}, Tipo: {request.TipoInterrogador}";

            var command = new RegistrarActividadCommand(
                request.AudienciaId,
                descripcion,
                (int)TipoActividad.TestimonialDeclaraciones,
                observaciones
            );

            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(ObtenerInterrogatorio), new { id = result.ActividadId }, result.ActividadId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar interrogatorio al acusado");
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Realiza una pregunta en el interrogatorio
    /// </summary>
    [HttpPost("{audienciaId}/actividades/{actividadId}/preguntas")]
    public async Task<ActionResult> RealizarPregunta(Guid audienciaId, Guid actividadId, [FromBody] RealizarPreguntaRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Pregunta realizada en interrogatorio {ActividadId} de audiencia {AudienciaId}: {Pregunta}",
                actividadId, audienciaId, request.Pregunta);

            var descripcion = $"Pregunta en interrogatorio: {request.Pregunta}";
            var observaciones = $"Preguntado por: {request.PreguntadoPor}, Timestamp: {request.Timestamp}";

            var command = new RegistrarActividadCommand(
                audienciaId,
                descripcion,
                (int)TipoActividad.TestimonialDeclaraciones,
                observaciones
            );

            var result = await _mediator.Send(command);
            
            return Ok(new { actividadId = result.ActividadId, pregunta = request.Pregunta });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar pregunta en interrogatorio {ActividadId}", actividadId);
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Registra una respuesta en el interrogatorio
    /// </summary>
    [HttpPost("{audienciaId}/actividades/{actividadId}/respuestas")]
    public async Task<ActionResult> RegistrarRespuesta(Guid audienciaId, Guid actividadId, [FromBody] RegistrarRespuestaRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Respuesta registrada en interrogatorio {ActividadId} de audiencia {AudienciaId}: {Respuesta}",
                actividadId, audienciaId, request.Respuesta);

            var descripcion = $"Respuesta en interrogatorio: {request.Respuesta}";
            var observaciones = $"Timestamp: {request.Timestamp}, Observaciones: {request.Observaciones}";

            var command = new RegistrarActividadCommand(
                audienciaId,
                descripcion,
                (int)TipoActividad.TestimonialDeclaraciones,
                observaciones
            );

            var result = await _mediator.Send(command);

            return Ok(new { actividadId = result.ActividadId, respuesta = request.Respuesta });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar respuesta en interrogatorio {ActividadId}", actividadId);
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Finaliza un interrogatorio
    /// </summary>
    [HttpPost("{audienciaId}/actividades/{actividadId}/finalizar")]
    public async Task<ActionResult> FinalizarInterrogatorio(Guid audienciaId, Guid actividadId)
    {
        try
        {
            _logger.LogInformation("Finalizando interrogatorio {ActividadId} de audiencia {AudienciaId}", actividadId, audienciaId);
            
            var descripcion = "Interrogatorio finalizado";
            var observaciones = $"Finalizado en: {DateTime.UtcNow}";

            var command = new RegistrarActividadCommand(
                audienciaId,
                descripcion,
                (int)TipoActividad.TestimonialDeclaraciones,
                observaciones
            );

            var result = await _mediator.Send(command);
            
            return Ok(new { mensaje = $"Interrogatorio {actividadId} finalizado correctamente", actividadId = result.ActividadId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al finalizar interrogatorio {ActividadId}", actividadId);
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Obtiene un interrogatorio por ID
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<InterrogatorioDto> ObtenerInterrogatorio(Guid id)
    {
        try
        {
            // TODO: Implementar query para obtener actividad de interrogatorio
            _logger.LogInformation("Solicitud de interrogatorio con ID: {Id}", id);
            return NotFound($"Interrogatorio con ID {id} no encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener interrogatorio con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Obtiene todos los interrogatorios de una audiencia
    /// </summary>
    [HttpGet("audiencia/{audienciaId}")]
    public ActionResult<IEnumerable<InterrogatorioDto>> ObtenerInterrogatoriosPorAudiencia(Guid audienciaId)
    {
        try
        {
            _logger.LogInformation("Solicitud de interrogatorios para audiencia: {AudienciaId}", audienciaId);
            // TODO: Implementar query para obtener actividades de tipo TestimonialDeclaraciones de una audiencia
            return Ok(new List<InterrogatorioDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener interrogatorios para audiencia: {AudienciaId}", audienciaId);
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Realiza una objeción durante el interrogatorio
    /// </summary>
    [HttpPost("{audienciaId}/actividades/{actividadId}/objeciones")]
    public async Task<ActionResult> RealizarObjecion(Guid audienciaId, Guid actividadId, [FromBody] RealizarObjecionRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Objeción realizada en interrogatorio {ActividadId} de audiencia {AudienciaId} por {ObjetorId}: {Motivo}",
                actividadId, audienciaId, request.ObjetorId, request.Motivo);

            var descripcion = $"Objeción en interrogatorio: {request.Motivo}";
            var observaciones = $"Objetor: {request.ObjetorId}, Timestamp: {request.Timestamp}";

            var command = new RegistrarActividadCommand(
                audienciaId,
                descripcion,
                (int)TipoActividad.TestimonialDeclaraciones,
                observaciones
            );

            var result = await _mediator.Send(command);

            return Ok(new { actividadId = result.ActividadId, objecion = request.Motivo });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar objeción en interrogatorio {ActividadId}", actividadId);
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Obtiene todos los interrogatorios
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<InterrogatorioDto>> ObtenerTodosLosInterrogatorios()
    {
        try
        {
            _logger.LogInformation("Solicitud de todos los interrogatorios");
            
            // TODO: Implementar query para obtener todas las actividades de tipo TestimonialDeclaraciones
            // Por ahora retornamos una lista vacía
            return Ok(new List<InterrogatorioDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los interrogatorios");
            return StatusCode(500, "Error interno del servidor");
        }
    }/// <summary>
    /// Crea un nuevo interrogatorio genérico
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CrearInterrogatorio([FromBody] CrearInterrogatorioRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Creando interrogatorio genérico con pregunta: {Pregunta}",
                request.Pregunta);

            // Para interrogatorios genéricos, necesitamos obtener el AudienciaId de alguna manera
            // Por ahora usaremos un ID por defecto o requeriremos que se pase en el request
            if (request.AudienciaId == Guid.Empty)
            {
                return BadRequest("Se requiere un ID de audiencia válido para crear el interrogatorio");
            }

            var descripcion = !string.IsNullOrEmpty(request.Pregunta) 
                ? $"Interrogatorio: {request.Pregunta}" 
                : "Interrogatorio genérico";
            
            var observaciones = $"Tipo: {request.Tipo}, Fecha: {request.FechaHora}";
            
            if (!string.IsNullOrEmpty(request.Respuesta))
            {
                observaciones += $", Respuesta: {request.Respuesta}";
            }

            var command = new RegistrarActividadCommand(
                request.AudienciaId,
                descripcion,
                (int)TipoActividad.TestimonialDeclaraciones,
                observaciones
            );

            var result = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(ObtenerInterrogatorio), new { id = result.ActividadId }, result.ActividadId);
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
public record CrearInterrogatorioRequest(Guid Id, Guid AudienciaId, string? Pregunta, string? Respuesta, DateTime FechaHora, string? Tipo);

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
