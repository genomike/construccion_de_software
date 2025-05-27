using EtapaDeJuicio.Application.DTOs;
using MediatR;

namespace EtapaDeJuicio.Application.Queries;

public record ObtenerAudienciaPorIdQuery(
    Guid AudienciaId
) : IRequest<AudienciaDto?>;

public record ObtenerAudienciasProgramadasQuery(
    DateTime? FechaInicio = null,
    DateTime? FechaFin = null,
    int PageSize = 10,
    int Page = 1
) : IRequest<ObtenerAudienciasResult>;

public record ObtenerAudienciasResult(
    List<AudienciaDto> Audiencias,
    int TotalCount,
    int PageSize,
    int CurrentPage
);
