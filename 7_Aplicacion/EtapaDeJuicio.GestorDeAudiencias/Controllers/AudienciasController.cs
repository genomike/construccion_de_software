using MediatR;
using Microsoft.AspNetCore.Mvc;
using EtapaDeJuicio.Application.Commands.Audiencias;
using EtapaDeJuicio.Application.Queries.Audiencias;
using EtapaDeJuicio.Application.DTOs;

namespace EtapaDeJuicio.GestorDeAudiencias.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudienciasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AudienciasController> _logger;

    public AudienciasController(IMediator mediator, ILogger<AudienciasController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }    /// <summary>
    /// Obtiene todas las audiencias
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AudienciaDto>>> ObtenerAudiencias()
    {
        try
        {
            var query = new ObtenerAudienciasQuery();
            var resultado = await _mediator.Send(query);
            return Ok(resultado.Audiencias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener audiencias");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene una audiencia por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AudienciaDto>> ObtenerAudienciaPorId(Guid id)
    {
        try
        {
            var query = new ObtenerAudienciaPorIdQuery(id);
            var audiencia = await _mediator.Send(query);
            
            if (audiencia == null)
                return NotFound($"No se encontró la audiencia con ID: {id}");
                
            return Ok(audiencia);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener audiencia con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Crea una nueva audiencia
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> CrearAudiencia([FromBody] CrearAudienciaCommand command)
    {
        try
        {
            var resultado = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObtenerAudienciaPorId), new { id = resultado.AudienciaId }, resultado.AudienciaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear audiencia");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Inicia una audiencia
    /// </summary>
    [HttpPost("{id}/iniciar")]
    public async Task<ActionResult> IniciarAudiencia(Guid id)
    {
        try
        {
            var command = new IniciarAudienciaCommand(id);
            await _mediator.Send(command);
            return Ok($"Audiencia {id} iniciada correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar audiencia con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Agrega un participante a la audiencia
    /// </summary>
    [HttpPost("{id}/participantes")]
    public async Task<ActionResult> AgregarParticipante(Guid id, [FromBody] AgregarParticipanteRequest request)
    {
        try
        {
            // Crear el comando con los parámetros correctos
            var command = new AgregarParticipanteCommand(id, request.ParticipanteId, request.Nombre, request.RolParticipante);
            await _mediator.Send(command);
            return Ok($"Participante agregado a la audiencia {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar participante a audiencia con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Finaliza una audiencia
    /// </summary>
    [HttpPost("{id}/finalizar")]
    public async Task<ActionResult> FinalizarAudiencia(Guid id)
    {
        try
        {
            var command = new FinalizarAudienciaCommand(id);
            await _mediator.Send(command);
            return Ok($"Audiencia {id} finalizada correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al finalizar audiencia con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }    /// <summary>
    /// Registra una actividad en la audiencia
    /// </summary>
    [HttpPost("{id}/actividades")]
    public async Task<ActionResult> RegistrarActividad(Guid id, [FromBody] RegistrarActividadRequest request)
    {
        try
        {
            // Crear el comando con los parámetros correctos
            var command = new RegistrarActividadCommand(id, request.Descripcion, request.TipoActividad);
            await _mediator.Send(command);
            return Ok($"Actividad registrada en audiencia {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar actividad en audiencia con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}

// DTOs para las requests
public record AgregarParticipanteRequest(Guid ParticipanteId, string Nombre, int RolParticipante);
public record RegistrarActividadRequest(string Descripcion, int TipoActividad);
