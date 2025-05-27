using MediatR;

namespace EtapaDeJuicio.Application.Commands.Audiencias;

public record IniciarAudienciaCommand(
    Guid AudienciaId
) : IRequest<IniciarAudienciaResult>;

public record IniciarAudienciaResult(
    Guid AudienciaId,
    DateTime FechaInicio,
    string Estado
);
