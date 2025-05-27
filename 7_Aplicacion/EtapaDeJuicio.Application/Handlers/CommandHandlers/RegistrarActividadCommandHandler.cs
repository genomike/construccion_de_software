using EtapaDeJuicio.Application.Commands.Audiencias;
using EtapaDeJuicio.Application.Interfaces;
using EtapaDeJuicio.Domain.Entities.Audiencias;
using MediatR;

namespace EtapaDeJuicio.Application.Handlers.CommandHandlers;

public class RegistrarActividadCommandHandler : IRequestHandler<RegistrarActividadCommand, RegistrarActividadResult>
{
    private readonly IAudienciaRepository _audienciaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarActividadCommandHandler(
        IAudienciaRepository audienciaRepository,
        IUnitOfWork unitOfWork)
    {
        _audienciaRepository = audienciaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegistrarActividadResult> Handle(
        RegistrarActividadCommand request, 
        CancellationToken cancellationToken)
    {
        var audiencia = await _audienciaRepository.ObtenerPorIdAsync(request.AudienciaId, cancellationToken);
        
        if (audiencia == null)
            throw new InvalidOperationException($"Audiencia con ID {request.AudienciaId} no encontrada");

        var tipoActividad = (TipoActividad)request.TipoActividad;
        var actividadId = audiencia.RegistrarActividad(request.Descripcion, tipoActividad, request.Observaciones);

        await _audienciaRepository.ActualizarAsync(audiencia, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegistrarActividadResult(
            audiencia.Id,
            actividadId,
            request.Descripcion,
            tipoActividad.ToString(),
            DateTime.UtcNow
        );
    }
}
