namespace EtapaDeJuicio.Application.DTOs;

public record AudienciaDto(
    Guid Id,
    string Titulo,
    DateTime FechaProgramada,
    string TipoAudiencia,
    string Estado,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    TimeSpan? Duracion,
    List<ParticipanteDto> Participantes,
    List<ActividadDto> Actividades
);

public record ParticipanteDto(
    Guid Id,
    string Nombre,
    string Rol,
    DateTime FechaRegistro
);

public record ActividadDto(
    Guid Id,
    string Descripcion,
    string TipoActividad,
    DateTime Timestamp,
    string? Observaciones
);
