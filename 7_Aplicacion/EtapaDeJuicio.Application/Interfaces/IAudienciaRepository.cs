using EtapaDeJuicio.Domain.Entities.Audiencias;

namespace EtapaDeJuicio.Application.Interfaces;

public interface IAudienciaRepository
{
    Task<Audiencia?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Audiencia>> ObtenerTodosAsync(
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    Task<List<Audiencia>> ObtenerAudienciasProgramadasAsync(
        DateTime? fechaInicio, 
        DateTime? fechaFin, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default);
    Task<int> ContarAudienciasProgramadasAsync(
        DateTime? fechaInicio, 
        DateTime? fechaFin, 
        CancellationToken cancellationToken = default);
    Task<Guid> CrearAsync(Audiencia audiencia, CancellationToken cancellationToken = default);
    Task ActualizarAsync(Audiencia audiencia, CancellationToken cancellationToken = default);
    Task EliminarAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
