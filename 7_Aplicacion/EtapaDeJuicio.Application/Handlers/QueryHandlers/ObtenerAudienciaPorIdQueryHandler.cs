using EtapaDeJuicio.Application.DTOs;
using EtapaDeJuicio.Application.Interfaces;
using EtapaDeJuicio.Application.Queries.Audiencias;
using EtapaDeJuicio.Domain.Entities.Audiencias;
using MediatR;

namespace EtapaDeJuicio.Application.Handlers.QueryHandlers;

public class ObtenerAudienciaPorIdQueryHandler : IRequestHandler<ObtenerAudienciaPorIdQuery, AudienciaDto?>
{
    private readonly IAudienciaRepository _audienciaRepository;

    public ObtenerAudienciaPorIdQueryHandler(IAudienciaRepository audienciaRepository)
    {
        _audienciaRepository = audienciaRepository;
    }

    public async Task<AudienciaDto?> Handle(
        ObtenerAudienciaPorIdQuery request, 
        CancellationToken cancellationToken)
    {
        var audiencia = await _audienciaRepository.ObtenerPorIdAsync(request.AudienciaId, cancellationToken);
        
        if (audiencia == null)
            return null;

        return MapearAudienciaDto(audiencia);
    }

    private static AudienciaDto MapearAudienciaDto(Audiencia audiencia)
    {
        return new AudienciaDto(
            audiencia.Id,
            audiencia.Titulo,
            audiencia.FechaProgramada,
            audiencia.Tipo.ToString(),
            audiencia.Estado.ToString(),
            audiencia.FechaInicio,
            audiencia.FechaFin,
            audiencia.CalcularDuracion(),
            audiencia.Participantes.Select(p => new ParticipanteDto(
                p.Id,
                p.Nombre,
                p.Rol.ToString(),
                p.FechaRegistro
            )).ToList(),
            audiencia.Actividades.Select(a => new ActividadDto(
                a.Id,
                a.Descripcion,
                a.Tipo.ToString(),
                a.Timestamp,
                a.Observaciones
            )).ToList()
        );
    }
}
