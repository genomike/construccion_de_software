using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EtapaDeJuicio.Infraestructure.Persistence.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EtapaDeJuicioDbContext>
{    public EtapaDeJuicioDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EtapaDeJuicioDbContext>();
        
        // Usar la nueva cadena de conexión de SQL Server
        var connectionString = "Server=localhost;Database=EtapaDeJuicioDB;User Id=sa;Password=123456;TrustServerCertificate=true;MultipleActiveResultSets=true";
        
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
