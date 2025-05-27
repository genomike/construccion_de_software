using EtapaDeJuicio.Application.Commands.Audiencias;
using EtapaDeJuicio.Application.Interfaces;
using EtapaDeJuicio.Domain.Entities.Audiencias;
using MediatR;

namespace EtapaDeJuicio.Application.Handlers.CommandHandlers;

public class CrearAudienciaCommandHandler : IRequestHandler<CrearAudienciaCommand, CrearAudienciaResult>
{
    private readonly IAudienciaRepository _audienciaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CrearAudienciaCommandHandler(
        IAudienciaRepository audienciaRepository,
        IUnitOfWork unitOfWork)
    {
        _audienciaRepository = audienciaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CrearAudienciaResult> Handle(
        CrearAudienciaCommand request, 
        CancellationToken cancellationToken)
    {
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(),
            request.Titulo,
            request.FechaProgramada,
            (TipoAudiencia)request.TipoAudiencia
        );

        var audienciaId = await _audienciaRepository.CrearAsync(audiencia, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CrearAudienciaResult(
            audienciaId,
            audiencia.Titulo,
            audiencia.FechaProgramada,
            audiencia.Estado.ToString()
        );
    }
}
