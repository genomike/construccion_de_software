using EtapaDeJuicio.Application.Commands.Audiencias;
using EtapaDeJuicio.Application.Interfaces;
using MediatR;

namespace EtapaDeJuicio.Application.Handlers.CommandHandlers;

public class IniciarAudienciaCommandHandler : IRequestHandler<IniciarAudienciaCommand, IniciarAudienciaResult>
{
    private readonly IAudienciaRepository _audienciaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public IniciarAudienciaCommandHandler(
        IAudienciaRepository audienciaRepository,
        IUnitOfWork unitOfWork)
    {
        _audienciaRepository = audienciaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IniciarAudienciaResult> Handle(
        IniciarAudienciaCommand request, 
        CancellationToken cancellationToken)
    {
        var audiencia = await _audienciaRepository.ObtenerPorIdAsync(request.AudienciaId, cancellationToken);
        
        if (audiencia == null)
            throw new InvalidOperationException($"Audiencia con ID {request.AudienciaId} no encontrada");

        audiencia.Iniciar();

        await _audienciaRepository.ActualizarAsync(audiencia, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new IniciarAudienciaResult(
            audiencia.Id,
            audiencia.FechaInicio!.Value,
            audiencia.Estado.ToString()
        );
    }
}
