using MediatR;

namespace EtapaDeJuicio.Application.Commands.Audiencias;

public record RegistrarActividadCommand(
    Guid AudienciaId,
    string Descripcion,
    int TipoActividad,
    string? Observaciones = null
) : IRequest<RegistrarActividadResult>;

public record RegistrarActividadResult(
    Guid AudienciaId,
    Guid ActividadId,
    string Descripcion,
    string TipoActividad,
    DateTime Timestamp
);
