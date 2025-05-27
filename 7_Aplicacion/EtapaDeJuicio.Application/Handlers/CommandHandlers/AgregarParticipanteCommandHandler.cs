using EtapaDeJuicio.Application.Commands.Audiencias;
using EtapaDeJuicio.Application.Interfaces;
using EtapaDeJuicio.Domain.Entities.Audiencias;
using MediatR;

namespace EtapaDeJuicio.Application.Handlers.CommandHandlers;

public class AgregarParticipanteCommandHandler : IRequestHandler<AgregarParticipanteCommand, AgregarParticipanteResult>
{
    private readonly IAudienciaRepository _audienciaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AgregarParticipanteCommandHandler(
        IAudienciaRepository audienciaRepository,
        IUnitOfWork unitOfWork)
    {
        _audienciaRepository = audienciaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AgregarParticipanteResult> Handle(
        AgregarParticipanteCommand request, 
        CancellationToken cancellationToken)
    {
        var audiencia = await _audienciaRepository.ObtenerPorIdAsync(request.AudienciaId, cancellationToken);
        
        if (audiencia == null)
            throw new InvalidOperationException($"Audiencia con ID {request.AudienciaId} no encontrada");

        var rol = (RolParticipante)request.RolParticipante;
        audiencia.AgregarParticipante(request.ParticipanteId, request.Nombre, rol);

        await _audienciaRepository.ActualizarAsync(audiencia, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AgregarParticipanteResult(
            audiencia.Id,
            request.ParticipanteId,
            request.Nombre,
            rol.ToString()
        );
    }
}
