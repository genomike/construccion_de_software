using EtapaDeJuicio.Domain.Entities.Sentencias;
using EtapaDeJuicio.Domain.Exceptions;
using FluentAssertions;

namespace EtapaDeJuicioTests;

public class DeliberacionJudicialTests
{
    [Fact]
    public void Deliberacion_CrearConDatosValidos_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var casoNumero = "CASO-2025-001";

        // Act
        var deliberacion = Deliberacion.Crear(id, casoNumero);

        // Assert
        deliberacion.Should().NotBeNull();
        deliberacion.Id.Should().Be(id);
        deliberacion.CasoNumero.Should().Be(casoNumero);
        deliberacion.FechaInicio.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        deliberacion.FechaFinalizacion.Should().BeNull();
        deliberacion.EstaFinalizada.Should().BeFalse();
        deliberacion.Considerandos.Should().BeEmpty();
        deliberacion.ValoracionesPruebas.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Deliberacion_CrearConCasoNumeroInvalido_DeberiaLanzarExcepcion(string casoNumero)
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act & Assert
        var act = () => Deliberacion.Crear(id, casoNumero);
        act.Should().Throw<DomainException>()
           .WithMessage("*número de caso*obligatorio*");
    }

    [Fact]
    public void Deliberacion_CrearConIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var casoNumero = "CASO-2025-001";

        // Act & Assert
        var act = () => Deliberacion.Crear(Guid.Empty, casoNumero);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID*deliberación*vacío*");
    }

    [Fact]
    public void Deliberacion_AgregarValoracionPrueba_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "CASO-2025-001");
        var pruebaId = Guid.NewGuid();
        var valor = 0.8m;
        var justificacion = "Prueba muy convincente";

        // Act
        deliberacion.AgregarValoracionPrueba(pruebaId, valor, justificacion);

        // Assert
        deliberacion.ValoracionesPruebas.Should().HaveCount(1);
        var valoracion = deliberacion.ValoracionesPruebas[0];
        valoracion.PruebaId.Should().Be(pruebaId);
        valoracion.Valor.Should().Be(valor);
        valoracion.Justificacion.Should().Be(justificacion);
    }

    [Fact]
    public void Deliberacion_AgregarValoracionPruebaEnDeliberacionFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "CASO-2025-001");
        deliberacion.AgregarConsiderando("Considerando único para la prueba");
        deliberacion.Finalizar();

        // Act & Assert
        var act = () => deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.7m, "Justificación");
        act.Should().Throw<DomainException>()
           .WithMessage("*No se pueden agregar valoraciones*finalizada*");
    }

    [Fact]
    public void Deliberacion_AgregarConsiderando_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "CASO-2025-001");
        var contenido = "CONSIDERANDO que según el artículo 123 del Código Penal...";

        // Act
        deliberacion.AgregarConsiderando(contenido);

        // Assert
        deliberacion.Considerandos.Should().HaveCount(1);
        var considerando = deliberacion.Considerandos[0];
        considerando.Contenido.Should().Be(contenido);
        considerando.Orden.Should().Be(1);
    }

    [Fact]
    public void Deliberacion_AgregarConsiderandoEnDeliberacionFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "CASO-2025-001");
        deliberacion.AgregarConsiderando("Primer considerando");
        deliberacion.Finalizar();

        // Act & Assert
        var act = () => deliberacion.AgregarConsiderando("Otro considerando");
        act.Should().Throw<DomainException>()
           .WithMessage("*No se pueden agregar considerandos*finalizada*");
    }

    [Fact]
    public void Deliberacion_GenerarConsiderandosSinValoraciones_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "CASO-2025-001");

        // Act & Assert
        var act = () => deliberacion.GenerarConsiderandos();
        act.Should().Throw<DomainException>()
           .WithMessage("*No se puede generar considerandos sin pruebas valoradas*");
    }

    [Fact]
    public void Deliberacion_GenerarConsiderandosConValoraciones_DeberiaGenerarConsiderandos()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "CASO-2025-001");
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.8m, "Prueba convincente");

        // Act
        deliberacion.GenerarConsiderandos();

        // Assert
        deliberacion.Considerandos.Should().HaveCount(1);
        deliberacion.Considerandos[0].Contenido.Should().Contain("pruebas aportadas");
    }

    [Fact]
    public void Deliberacion_FinalizarSinConsiderandos_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "CASO-2025-001");

        // Act & Assert
        var act = () => deliberacion.Finalizar();
        act.Should().Throw<DomainException>()
           .WithMessage("*No se puede finalizar*sin considerandos*");
    }

    [Fact]
    public void Deliberacion_FinalizarConConsiderandos_DeberiaFinalizarCorrectamente()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "CASO-2025-001");
        deliberacion.AgregarConsiderando("Considerando de prueba");

        // Act
        deliberacion.Finalizar();

        // Assert
        deliberacion.EstaFinalizada.Should().BeTrue();
        deliberacion.FechaFinalizacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }
}