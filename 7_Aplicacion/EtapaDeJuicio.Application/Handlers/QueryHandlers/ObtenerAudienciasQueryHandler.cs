using EtapaDeJuicio.Application.DTOs;
using EtapaDeJuicio.Application.Interfaces;
using EtapaDeJuicio.Application.Queries.Audiencias;
using EtapaDeJuicio.Domain.Entities.Audiencias;
using MediatR;

namespace EtapaDeJuicio.Application.Handlers.QueryHandlers;

public class ObtenerAudienciasQueryHandler : IRequestHandler<ObtenerAudienciasQuery, ObtenerAudienciasResult>
{
    private readonly IAudienciaRepository _audienciaRepository;

    public ObtenerAudienciasQueryHandler(IAudienciaRepository audienciaRepository)
    {
        _audienciaRepository = audienciaRepository;
    }    public async Task<ObtenerAudienciasResult> Handle(
        ObtenerAudienciasQuery request, 
        CancellationToken cancellationToken)
    {
        var audiencias = await _audienciaRepository.ObtenerTodosAsync(
            request.Pagina, 
            request.TamanoPagina, 
            cancellationToken);
        
        // Aplicar filtros si están presentes
        if (request.FechaDesde.HasValue)
        {
            audiencias = audiencias.Where(a => a.FechaProgramada >= request.FechaDesde.Value).ToList();
        }
        
        if (request.FechaHasta.HasValue)
        {
            audiencias = audiencias.Where(a => a.FechaProgramada <= request.FechaHasta.Value).ToList();
        }
        
        if (!string.IsNullOrEmpty(request.Estado))
        {
            audiencias = audiencias.Where(a => a.Estado.ToString().Equals(request.Estado, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Nota: La paginación ya se aplica en el repositorio
        var totalRegistros = audiencias.Count;
        var audienciasDto = audiencias.Select(MapearAudienciaDto).ToList();

        return new ObtenerAudienciasResult(
            audienciasDto,
            totalRegistros,
            request.Pagina,
            request.TamanoPagina
        );
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
