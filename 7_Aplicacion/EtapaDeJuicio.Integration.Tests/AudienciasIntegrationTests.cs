using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using FluentAssertions;
using EtapaDeJuicio.Application.Commands.Audiencias;
using EtapaDeJuicio.Application.DTOs;
using EtapaDeJuicio.Infraestructure.Persistence.Context;
using EtapaDeJuicio.Domain.Entities.Audiencias;

namespace EtapaDeJuicio.Integration.Tests;

public class AudienciasIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly EtapaDeJuicioDbContext _context;
    private readonly string _databaseName;

    public AudienciasIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // Generar un nombre único para cada instancia de prueba
        _databaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Reemplazar el contexto de base de datos con uno en memoria para las pruebas
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<EtapaDeJuicioDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<EtapaDeJuicioDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                    options.EnableSensitiveDataLogging();
                });
            });
        });

        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<EtapaDeJuicioDbContext>();
        
        // Asegurar que la base de datos esté creada
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CrearAudiencia_ConDatosValidos_DeberiaCrearAudienciaCorrectamente()
    {
        // Arrange
        var command = new CrearAudienciaCommand(
            "Audiencia de Prueba",
            DateTime.Now.AddDays(7),
            (int)TipoAudiencia.JuicioOral,
            120
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/audiencias", command);

        // Assert
        response.Should().BeSuccessful();
        var audienciaId = await response.Content.ReadFromJsonAsync<Guid>();
        audienciaId.Should().NotBeEmpty();

        // Verificar que la audiencia se guardó en la base de datos
        var audienciaEnBd = await _context.Audiencias.FirstOrDefaultAsync(a => a.Id == audienciaId);
        audienciaEnBd.Should().NotBeNull();
        audienciaEnBd!.Titulo.Should().Be(command.Titulo);
        audienciaEnBd.Tipo.Should().Be(TipoAudiencia.JuicioOral);
        audienciaEnBd.Estado.Should().Be(EstadoAudiencia.Programada);
    }

    [Fact]
    public async Task ObtenerAudienciaPorId_ConIdValido_DeberiaRetornarAudiencia()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(),
            "Audiencia de Prueba",
            DateTime.Now.AddDays(5),
            TipoAudiencia.AudienciaPreliminar
        );

        _context.Audiencias.Add(audiencia);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/audiencias/{audiencia.Id}");

        // Assert
        response.Should().BeSuccessful();
        var audienciaDto = await response.Content.ReadFromJsonAsync<AudienciaDto>();
        
        audienciaDto.Should().NotBeNull();
        audienciaDto!.Id.Should().Be(audiencia.Id);
        audienciaDto.Titulo.Should().Be("Audiencia de Prueba");
        audienciaDto.Estado.Should().Be("Programada");
    }

    [Fact]
    public async Task ObtenerAudiencias_DeberiaRetornarListaDeAudiencias()
    {
        // Arrange
        var audiencia1 = Audiencia.Crear(Guid.NewGuid(), "Audiencia 1", DateTime.Now.AddDays(1), TipoAudiencia.JuicioOral);
        var audiencia2 = Audiencia.Crear(Guid.NewGuid(), "Audiencia 2", DateTime.Now.AddDays(2), TipoAudiencia.AudienciaPreliminar);
        
        _context.Audiencias.AddRange(audiencia1, audiencia2);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/audiencias?page=1&pageSize=10");

        // Assert
        response.Should().BeSuccessful();
        var audiencias = await response.Content.ReadFromJsonAsync<List<AudienciaDto>>();
        audiencias.Should().NotBeNull();
        audiencias!.Should().HaveCountGreaterOrEqualTo(2);
    }

    public void Dispose()
    {
        _context?.Dispose();
        _scope?.Dispose();
        _client?.Dispose();
    }
}
