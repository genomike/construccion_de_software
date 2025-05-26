using EtapaDeJuicio.Domain.Entities.Sentencias;
using EtapaDeJuicio.Domain.Exceptions;
using FluentAssertions;

namespace EtapaDeJuicio.Domain.Tests.Entities.Sentencias;

public class ValoracionPruebaTests
{
    [Fact]
    public void ValoracionPrueba_CrearConDatosValidos_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var pruebaId = Guid.NewGuid();
        var valor = 0.8m;
        var justificacion = "La prueba es consistente y confiable";

        // Act
        var valoracion = ValoracionPrueba.Crear(pruebaId, valor, justificacion);

        // Assert
        valoracion.Should().NotBeNull();
        valoracion.PruebaId.Should().Be(pruebaId);
        valoracion.Valor.Should().Be(valor);
        valoracion.Justificacion.Should().Be(justificacion);
    }

    [Fact]
    public void ValoracionPrueba_CrearConPruebaIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var valor = 0.8m;
        var justificacion = "La prueba es consistente y confiable";

        // Act & Assert
        var act = () => ValoracionPrueba.Crear(Guid.Empty, valor, justificacion);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID de la prueba*vacío*");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void ValoracionPrueba_CrearConValorFueraDeRango_DeberiaLanzarExcepcion(decimal valor)
    {
        // Arrange
        var pruebaId = Guid.NewGuid();
        var justificacion = "La prueba es consistente y confiable";

        // Act & Assert
        var act = () => ValoracionPrueba.Crear(pruebaId, valor, justificacion);
        act.Should().Throw<DomainException>()
           .WithMessage("*valor*entre 0 y 1*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ValoracionPrueba_CrearConJustificacionInvalida_DeberiaLanzarExcepcion(string justificacion)
    {
        // Arrange
        var pruebaId = Guid.NewGuid();
        var valor = 0.8m;

        // Act & Assert
        var act = () => ValoracionPrueba.Crear(pruebaId, valor, justificacion);
        act.Should().Throw<DomainException>()
           .WithMessage("*justificación*obligatoria*");
    }
}
