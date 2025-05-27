using EtapaDeJuicio.Application.Commands.Audiencias;
using EtapaDeJuicio.Application.Interfaces;
using MediatR;

namespace EtapaDeJuicio.Application.Handlers.CommandHandlers;

public class FinalizarAudienciaCommandHandler : IRequestHandler<FinalizarAudienciaCommand, FinalizarAudienciaResult>
{
    private readonly IAudienciaRepository _audienciaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FinalizarAudienciaCommandHandler(
        IAudienciaRepository audienciaRepository,
        IUnitOfWork unitOfWork)
    {
        _audienciaRepository = audienciaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FinalizarAudienciaResult> Handle(
        FinalizarAudienciaCommand request, 
        CancellationToken cancellationToken)
    {
        var audiencia = await _audienciaRepository.ObtenerPorIdAsync(request.AudienciaId, cancellationToken);
        
        if (audiencia == null)
            throw new InvalidOperationException($"Audiencia con ID {request.AudienciaId} no encontrada");

        audiencia.Finalizar();

        await _audienciaRepository.ActualizarAsync(audiencia, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new FinalizarAudienciaResult(
            audiencia.Id,
            audiencia.FechaFin!.Value,
            audiencia.Estado.ToString(),
            audiencia.CalcularDuracion()
        );
    }
}
