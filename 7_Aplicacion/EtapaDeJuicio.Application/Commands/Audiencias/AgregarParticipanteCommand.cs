using MediatR;

namespace EtapaDeJuicio.Application.Commands.Audiencias;

public record AgregarParticipanteCommand(
    Guid AudienciaId,
    Guid ParticipanteId,
    string Nombre,
    int RolParticipante
) : IRequest<AgregarParticipanteResult>;

public record AgregarParticipanteResult(
    Guid AudienciaId,
    Guid ParticipanteId,
    string Nombre,
    string Rol
);
