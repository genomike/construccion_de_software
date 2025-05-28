using Microsoft.EntityFrameworkCore;
using EtapaDeJuicio.Application.Interfaces;
using EtapaDeJuicio.Domain.Entities.Audiencias;
using EtapaDeJuicio.Infraestructure.Persistence.Context;

namespace EtapaDeJuicio.Infraestructure.Persistence.Repositories;

public class AudienciaRepository : IAudienciaRepository
{
    private readonly EtapaDeJuicioDbContext _context;

    public AudienciaRepository(EtapaDeJuicioDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Audiencia?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Audiencias
            .Include(a => a.Participantes)
            .Include(a => a.Actividades)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Audiencia>> ObtenerTodosAsync(
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Audiencias
            .Include(a => a.Participantes)
            .Include(a => a.Actividades)
            .OrderBy(a => a.FechaHoraProgramada)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Audiencia>> ObtenerAudienciasProgramadasAsync(
        DateTime? fechaInicio, 
        DateTime? fechaFin, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Audiencias
            .Include(a => a.Participantes)
            .Include(a => a.Actividades)
            .AsQueryable();

        if (fechaInicio.HasValue)
        {
            query = query.Where(a => a.FechaHoraProgramada >= fechaInicio.Value);
        }

        if (fechaFin.HasValue)
        {
            query = query.Where(a => a.FechaHoraProgramada <= fechaFin.Value);
        }

        return await query
            .OrderBy(a => a.FechaHoraProgramada)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> ContarAudienciasProgramadasAsync(
        DateTime? fechaInicio, 
        DateTime? fechaFin, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Audiencias.AsQueryable();

        if (fechaInicio.HasValue)
        {
            query = query.Where(a => a.FechaHoraProgramada >= fechaInicio.Value);
        }

        if (fechaFin.HasValue)
        {
            query = query.Where(a => a.FechaHoraProgramada <= fechaFin.Value);
        }

        return await query.CountAsync(cancellationToken);
    }    public async Task<Guid> CrearAsync(Audiencia audiencia, CancellationToken cancellationToken = default)
    {
        if (audiencia == null)
            throw new ArgumentNullException(nameof(audiencia));
            
        await _context.Audiencias.AddAsync(audiencia, cancellationToken);
        return audiencia.Id;
    }    public Task ActualizarAsync(Audiencia audiencia, CancellationToken cancellationToken = default)
    {
        if (audiencia == null)
            throw new ArgumentNullException(nameof(audiencia));

        // Para EF InMemory, simplemente usar Update
        // Este enfoque funciona porque EF InMemory puede acceder a los backing fields
        _context.Audiencias.Update(audiencia);
        
        return Task.CompletedTask;
    }

    public async Task EliminarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var audiencia = await _context.Audiencias
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (audiencia != null)
        {
            _context.Audiencias.Remove(audiencia);
        }
    }
}
