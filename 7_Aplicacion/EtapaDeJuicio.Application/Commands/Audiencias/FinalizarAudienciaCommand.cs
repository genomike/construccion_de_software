using MediatR;

namespace EtapaDeJuicio.Application.Commands.Audiencias;

public record FinalizarAudienciaCommand(
    Guid AudienciaId
) : IRequest<FinalizarAudienciaResult>;

public record FinalizarAudienciaResult(
    Guid AudienciaId,
    DateTime FechaFin,
    string Estado,
    TimeSpan? Duracion
);
