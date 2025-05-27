using MediatR;
using EtapaDeJuicio.Application.DTOs;

namespace EtapaDeJuicio.Application.Queries.Audiencias;

public record ObtenerAudienciaPorIdQuery(
    Guid AudienciaId
) : IRequest<AudienciaDto?>;

public record ObtenerAudienciasQuery(
    DateTime? FechaDesde = null,
    DateTime? FechaHasta = null,
    string? Estado = null,
    int Pagina = 1,
    int TamanoPagina = 10
) : IRequest<ObtenerAudienciasResult>;

public record ObtenerAudienciasResult(
    List<AudienciaDto> Audiencias,
    int TotalRegistros,
    int Pagina,
    int TamanoPagina
);
