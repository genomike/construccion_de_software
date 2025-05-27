using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EtapaDeJuicio.Infraestructure.Persistence.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EtapaDeJuicioDbContext>
{
    public EtapaDeJuicioDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EtapaDeJuicioDbContext>();
        
        // Usar la misma cadena de conexión por defecto que está en DependencyInjection.cs
        var connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=EtapaDeJuicioDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";
        
        optionsBuilder.UseSqlServer(connectionString, options =>
        {
            options.MigrationsAssembly(typeof(EtapaDeJuicioDbContext).Assembly.FullName);
            options.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        });
        
        return new EtapaDeJuicioDbContext(optionsBuilder.Options);
    }
}
