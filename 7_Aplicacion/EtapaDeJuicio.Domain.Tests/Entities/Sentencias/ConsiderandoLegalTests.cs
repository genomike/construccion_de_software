using EtapaDeJuicio.Domain.Entities.Sentencias;
using EtapaDeJuicio.Domain.Exceptions;
using FluentAssertions;

namespace EtapaDeJuicio.Domain.Tests.Entities.Sentencias;

public class ConsiderandoLegalTests
{
    [Fact]
    public void ConsiderandoLegal_CrearConDatosValidos_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var contenido = "CONSIDERANDO que el acusado actuó con premeditación";
        var orden = 1;

        // Act
        var considerando = ConsiderandoLegal.Crear(contenido, orden);

        // Assert
        considerando.Should().NotBeNull();
        considerando.Id.Should().NotBe(Guid.Empty);
        considerando.Contenido.Should().Be(contenido);
        considerando.Orden.Should().Be(orden);
        considerando.PruebasReferenciadas.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ConsiderandoLegal_CrearConContenidoInvalido_DeberiaLanzarExcepcion(string contenido)
    {
        // Arrange
        var orden = 1;

        // Act & Assert
        var act = () => ConsiderandoLegal.Crear(contenido, orden);
        act.Should().Throw<DomainException>()
           .WithMessage("*contenido*considerando*obligatorio*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ConsiderandoLegal_CrearConOrdenInvalido_DeberiaLanzarExcepcion(int orden)
    {
        // Arrange
        var contenido = "CONSIDERANDO válido";

        // Act & Assert
        var act = () => ConsiderandoLegal.Crear(contenido, orden);
        act.Should().Throw<DomainException>()
           .WithMessage("*orden*considerando*mayor a cero*");
    }

    [Fact]
    public void ConsiderandoLegal_AgregarReferenciaAPrueba_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var considerando = ConsiderandoLegal.Crear("CONSIDERANDO válido", 1);
        var pruebaId = Guid.NewGuid();

        // Act
        considerando.AgregarReferenciaAPrueba(pruebaId);

        // Assert
        considerando.PruebasReferenciadas.Should().HaveCount(1);
        considerando.PruebasReferenciadas.Should().Contain(pruebaId);
    }

    [Fact]
    public void ConsiderandoLegal_AgregarReferenciaDuplicada_NoDeberiaAgregarDuplicado()
    {
        // Arrange
        var considerando = ConsiderandoLegal.Crear("CONSIDERANDO válido", 1);
        var pruebaId = Guid.NewGuid();
        considerando.AgregarReferenciaAPrueba(pruebaId);

        // Act
        considerando.AgregarReferenciaAPrueba(pruebaId);

        // Assert
        considerando.PruebasReferenciadas.Should().HaveCount(1);
        considerando.PruebasReferenciadas.Should().Contain(pruebaId);
    }

    [Fact]
    public void ConsiderandoLegal_AgregarReferenciaConIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var considerando = ConsiderandoLegal.Crear("CONSIDERANDO válido", 1);

        // Act & Assert
        var act = () => considerando.AgregarReferenciaAPrueba(Guid.Empty);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID de la prueba*vacío*");
    }
}
