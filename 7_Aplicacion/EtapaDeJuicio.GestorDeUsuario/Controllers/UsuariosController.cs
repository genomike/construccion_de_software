using MediatR;
using Microsoft.AspNetCore.Mvc;
using EtapaDeJuicio.Domain.Entities;
using EtapaDeJuicio.Domain.ValueObjects;

namespace EtapaDeJuicio.GestorDeUsuario.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(IMediator mediator, ILogger<UsuariosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Autentica un usuario en el sistema
    /// </summary>
    [HttpPost("autenticar")]
    public async Task<ActionResult<AutenticacionDto>> AutenticarUsuario([FromBody] AutenticarUsuarioRequest request)
    {
        try
        {
            _logger.LogInformation("Intento de autenticación para usuario: {Usuario}", request.Usuario);

            // TODO: Implementar autenticación real
            var token = Guid.NewGuid().ToString();
            var autenticacion = new AutenticacionDto(
                true,
                token,
                DateTime.UtcNow.AddHours(8),
                request.Usuario,
                "Juez" // Rol por defecto para ejemplo
            );

            return Ok(autenticacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en autenticación para usuario: {Usuario}", request.Usuario);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene los participantes de una audiencia
    /// </summary>
    [HttpGet("audiencia/{audienciaId}/participantes")]
    public async Task<ActionResult<IEnumerable<ParticipanteDto>>> ObtenerParticipantesPorAudiencia(Guid audienciaId)
    {
        try
        {
            _logger.LogInformation("Solicitud de participantes para audiencia: {AudienciaId}", audienciaId);

            // TODO: Implementar query para obtener participantes
            var participantes = new List<ParticipanteDto>
            {
                new ParticipanteDto(Guid.NewGuid(), "Juan Pérez", "Juez", "juan.perez@juzgado.gov"),
                new ParticipanteDto(Guid.NewGuid(), "María García", "Fiscal", "maria.garcia@fiscalia.gov"),
                new ParticipanteDto(Guid.NewGuid(), "Carlos López", "Defensor", "carlos.lopez@abogados.com")
            };

            return Ok(participantes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener participantes para audiencia: {AudienciaId}", audienciaId);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Registra un nuevo usuario en el sistema
    /// </summary>
    [HttpPost("registro")]
    public async Task<ActionResult<Guid>> RegistrarUsuario([FromBody] RegistrarUsuarioRequest request)
    {
        try
        {
            var usuarioId = Guid.NewGuid();
            
            _logger.LogInformation(
                "Registrando nuevo usuario: {Nombre} con rol {Rol}",
                request.Nombre, request.Rol);

            // TODO: Implementar comando para registrar usuario

            return CreatedAtAction(nameof(ObtenerUsuario), new { id = usuarioId }, usuarioId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar usuario");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene un usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioDto>> ObtenerUsuario(Guid id)
    {
        try
        {
            _logger.LogInformation("Solicitud de usuario con ID: {Id}", id);
            
            // TODO: Implementar query para obtener usuario
            return NotFound($"Usuario con ID {id} no encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza los datos de un usuario
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> ActualizarUsuario(Guid id, [FromBody] ActualizarUsuarioRequest request)
    {
        try
        {
            _logger.LogInformation("Actualizando usuario: {Id}", id);

            // TODO: Implementar comando para actualizar usuario

            return Ok($"Usuario {id} actualizado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Asigna un rol a un usuario para una audiencia específica
    /// </summary>
    [HttpPost("{id}/audiencia/{audienciaId}/rol")]
    public async Task<ActionResult> AsignarRolEnAudiencia(
        Guid id, 
        Guid audienciaId, 
        [FromBody] AsignarRolRequest request)
    {
        try
        {
            _logger.LogInformation(
                "Asignando rol {Rol} a usuario {UserId} en audiencia {AudienciaId}",
                request.Rol, id, audienciaId);

            return Ok($"Rol {request.Rol} asignado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar rol en audiencia");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene todos los usuarios por rol
    /// </summary>
    [HttpGet("rol/{rol}")]
    public async Task<ActionResult<IEnumerable<UsuarioDto>>> ObtenerUsuariosPorRol(string rol)
    {
        try
        {
            _logger.LogInformation("Solicitud de usuarios con rol: {Rol}", rol);

            // TODO: Implementar query para obtener usuarios por rol
            return Ok(new List<UsuarioDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios por rol: {Rol}", rol);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Desactiva un usuario del sistema
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DesactivarUsuario(Guid id)
    {
        try
        {
            _logger.LogInformation("Desactivando usuario: {Id}", id);

            // TODO: Implementar comando para desactivar usuario

            return Ok($"Usuario {id} desactivado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar usuario con ID: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el historial de participación de un usuario
    /// </summary>
    [HttpGet("{id}/historial")]
    public async Task<ActionResult<IEnumerable<HistorialParticipacionDto>>> ObtenerHistorialParticipacion(Guid id)
    {
        try
        {
            _logger.LogInformation("Solicitud de historial para usuario: {Id}", id);

            return Ok(new List<HistorialParticipacionDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial para usuario: {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}

// DTOs para las requests
public record AutenticarUsuarioRequest(string Usuario, string Contraseña);
public record RegistrarUsuarioRequest(string Nombre, string Email, string Rol, string Institucion);
public record ActualizarUsuarioRequest(string Nombre, string Email, string Telefono, string Institucion);
public record AsignarRolRequest(string Rol, DateTime FechaAsignacion);

// DTOs para respuestas
public record AutenticacionDto(
    bool EsExitosa,
    string Token,
    DateTime Expiracion,
    string Usuario,
    string Rol);

public record ParticipanteDto(Guid Id, string Nombre, string Rol, string Email);

public record UsuarioDto(
    Guid Id,
    string Nombre,
    string Email,
    string Rol,
    string Institucion,
    bool EstaActivo,
    DateTime FechaRegistro);

public record HistorialParticipacionDto(
    Guid AudienciaId,
    string TipoAudiencia,
    string Rol,
    DateTime Fecha,
    string Estado);
