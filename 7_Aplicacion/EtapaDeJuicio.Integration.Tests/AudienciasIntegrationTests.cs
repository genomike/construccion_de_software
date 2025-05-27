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
    private readonly string _databaseName = $"InMemoryDbForTesting_{Guid.NewGuid()}";

    public AudienciasIntegrationTests(WebApplicationFactory<Program> factory)
    {
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
                });
            });
        });

        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<EtapaDeJuicioDbContext>();
        
        // Asegurar que la base de datos esté creada
        _context.Database.EnsureCreated();    }

    private async Task LimpiarBaseDeDatos()
    {
        // Limpiar todas las entidades para cada test
        _context.Audiencias.RemoveRange(_context.Audiencias);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task CrearAudiencia_ConDatosValidos_DeberiaCrearAudienciaCorrectamente()
    {
        // Arrange
        await LimpiarBaseDeDatos();
        var command = new CrearAudienciaCommand(
            "Audiencia de Juicio Oral - Caso 001",
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
    }    [Fact]
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
    public async Task IniciarAudiencia_ConAudienciaValida_DeberiaIniciarCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(),
            "Audiencia para Iniciar",
            DateTime.Now.AddDays(1),
            TipoAudiencia.JuicioOral
        );

        _context.Audiencias.Add(audiencia);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.PostAsync($"/api/audiencias/{audiencia.Id}/iniciar", null);

        // Assert
        response.Should().BeSuccessful();

        // Verificar que la audiencia se actualizó en la base de datos
        var audienciaActualizada = await _context.Audiencias.FirstOrDefaultAsync(a => a.Id == audiencia.Id);
        audienciaActualizada.Should().NotBeNull();
        audienciaActualizada!.Estado.Should().Be(EstadoAudiencia.EnCurso);
        audienciaActualizada.FechaHoraInicio.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
    }    [Fact]
    public async Task AgregarParticipante_ConDatosValidos_DeberiaAgregarParticipante()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(),
            "Audiencia con Participantes",
            DateTime.Now.AddDays(3),
            TipoAudiencia.JuicioOral
        );

        _context.Audiencias.Add(audiencia);
        await _context.SaveChangesAsync();

        var participanteRequest = new
        {
            ParticipanteId = Guid.NewGuid(),
            Nombre = "Juan Pérez",
            RolParticipante = (int)RolParticipante.Defensor
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/audiencias/{audiencia.Id}/participantes", participanteRequest);

        // Assert
        response.Should().BeSuccessful();

        // Verificar que el participante se agregó
        var audienciaConParticipantes = await _context.Audiencias
            .Include(a => a.Participantes)
            .FirstOrDefaultAsync(a => a.Id == audiencia.Id);

        audienciaConParticipantes.Should().NotBeNull();
        audienciaConParticipantes!.Participantes.Should().HaveCount(1);
        audienciaConParticipantes.Participantes.First().Nombre.Should().Be("Juan Pérez");
        audienciaConParticipantes.Participantes.First().Rol.Should().Be(RolParticipante.Defensor);
    }

    [Fact]
    public async Task RegistrarActividad_ConDatosValidos_DeberiaRegistrarActividad()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(),
            "Audiencia con Actividades",
            DateTime.Now.AddDays(2),
            TipoAudiencia.JuicioOral
        );

        // Iniciar la audiencia para poder registrar actividades
        audiencia.Iniciar();

        _context.Audiencias.Add(audiencia);
        await _context.SaveChangesAsync();

        var actividadRequest = new
        {
            Descripcion = "Presentación de evidencia documental",
            TipoActividad = (int)TipoActividad.PresentacionPruebas
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/audiencias/{audiencia.Id}/actividades", actividadRequest);

        // Assert
        response.Should().BeSuccessful();

        // Verificar que la actividad se registró
        var audienciaConActividades = await _context.Audiencias
            .Include(a => a.Actividades)
            .FirstOrDefaultAsync(a => a.Id == audiencia.Id);

        audienciaConActividades.Should().NotBeNull();
        audienciaConActividades!.Actividades.Should().HaveCount(1);
        audienciaConActividades.Actividades.First().Descripcion.Should().Be("Presentación de evidencia documental");
        audienciaConActividades.Actividades.First().Tipo.Should().Be(TipoActividad.PresentacionPruebas);
    }    [Fact]
    public async Task FinalizarAudiencia_ConAudienciaEnCurso_DeberiaFinalizarCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(),
            "Audiencia para Finalizar",
            DateTime.Now.AddDays(1),
            TipoAudiencia.JuicioOral
        );

        audiencia.Iniciar();
        _context.Audiencias.Add(audiencia);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.PostAsync($"/api/audiencias/{audiencia.Id}/finalizar", null);

        // Assert
        response.Should().BeSuccessful();

        // Verificar que la audiencia se finalizó
        var audienciaFinalizada = await _context.Audiencias.FirstOrDefaultAsync(a => a.Id == audiencia.Id);
        audienciaFinalizada.Should().NotBeNull();
        audienciaFinalizada!.Estado.Should().Be(EstadoAudiencia.Finalizada);
        audienciaFinalizada.FechaHoraFin.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
    }    [Fact]
    public async Task ObtenerAudienciasProgramadas_ConFiltros_DeberiaRetornarAudienciasFiltradas()
    {
        // Arrange
        var fechaBase = DateTime.Today.AddDays(5);
        
        var audiencia1 = Audiencia.Crear(Guid.NewGuid(), "Audiencia 1", fechaBase, TipoAudiencia.JuicioOral);
        var audiencia2 = Audiencia.Crear(Guid.NewGuid(), "Audiencia 2", fechaBase.AddDays(1), TipoAudiencia.AudienciaPreliminar);
        var audiencia3 = Audiencia.Crear(Guid.NewGuid(), "Audiencia 3", fechaBase.AddDays(10), TipoAudiencia.JuicioOral);

        _context.Audiencias.AddRange(audiencia1, audiencia2, audiencia3);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/audiencias?fechaInicio={fechaBase:yyyy-MM-dd}&fechaFin={fechaBase.AddDays(2):yyyy-MM-dd}&page=1&pageSize=10");

        // Assert
        response.Should().BeSuccessful();
        var audiencias = await response.Content.ReadFromJsonAsync<List<AudienciaDto>>();
        
        audiencias.Should().NotBeNull();
        audiencias!.Should().HaveCount(2);
        audiencias.Should().Contain(a => a.Titulo == "Audiencia 1");
        audiencias.Should().Contain(a => a.Titulo == "Audiencia 2");
        audiencias.Should().NotContain(a => a.Titulo == "Audiencia 3");
    }

    public void Dispose()
    {
        _context?.Dispose();
        _scope?.Dispose();
        _client?.Dispose();
    }
}
