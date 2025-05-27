using MediatR;

namespace EtapaDeJuicio.Application.Commands.Audiencias;

public record CrearAudienciaCommand(
    string Titulo,
    DateTime FechaProgramada,
    int TipoAudiencia,
    int DuracionMinutos = 120
) : IRequest<CrearAudienciaResult>;

public record CrearAudienciaResult(
    Guid AudienciaId,
    string Titulo,
    DateTime FechaProgramada,
    string Estado
);
