using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EtapaDeJuicio.Domain.Entities.Audiencias;

namespace EtapaDeJuicio.Infraestructure.Persistence.Context;

public class EtapaDeJuicioDbContext : DbContext
{
    public EtapaDeJuicioDbContext(DbContextOptions<EtapaDeJuicioDbContext> options) : base(options)
    {
    }

    public DbSet<Audiencia> Audiencias { get; set; }
    public DbSet<ParticipanteAudiencia> ParticipantesAudiencia { get; set; }
    public DbSet<ActividadAudiencia> ActividadesAudiencia { get; set; }
    public DbSet<ActaAudiencia> ActasAudiencia { get; set; }    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configuraciones
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EtapaDeJuicioDbContext).Assembly);

        // Configurar esquema por defecto solo para bases de datos que lo soporten (no InMemory)
        var providerName = Database.ProviderName;
        if (providerName != null && !providerName.Contains("InMemory"))
        {
            modelBuilder.HasDefaultSchema("EtapaDeJuicio");
        }
    }
}
