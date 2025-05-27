using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EtapaDeJuicio.Application.Interfaces;
using EtapaDeJuicio.Infraestructure.Persistence.Context;
using EtapaDeJuicio.Infraestructure.Persistence.Repositories;
using EtapaDeJuicio.Infraestructure.Persistence.UnitOfWork;

namespace EtapaDeJuicio.Infraestructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraestructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Configurar Entity Framework
        services.AddDbContext<EtapaDeJuicioDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                // Configuración por defecto para desarrollo con LocalDB
                connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=EtapaDeJuicioDB;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true";
            }
            
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(EtapaDeJuicioDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            });

            // Configuraciones adicionales para desarrollo
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        // Registrar repositorios
        services.AddScoped<IAudienciaRepository, AudienciaRepository>();
        
        // Registrar Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
