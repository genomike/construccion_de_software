using EtapaDeJuicio.Domain.Entities.Sentencias;
using EtapaDeJuicio.Domain.Exceptions;
using FluentAssertions;

namespace EtapaDeJuicio.Domain.Tests.Entities.Sentencias;

public class SentenciaTests
{
    [Fact]
    public void Sentencia_CrearConDatosValidos_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var juezId = Guid.NewGuid();
        var descripcion = "Sentencia en el caso 001/2025";
        var tipo = TipoSentencia.Condenatoria;

        // Act
        var sentencia = Sentencia.Crear(id, juezId, descripcion, tipo);

        // Assert
        sentencia.Should().NotBeNull();
        sentencia.Id.Should().Be(id);
        sentencia.JuezId.Should().Be(juezId);
        sentencia.Descripcion.Should().Be(descripcion);
        sentencia.Tipo.Should().Be(tipo);
        sentencia.FechaEmision.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        sentencia.Resolutivos.Should().BeEmpty();
    }

    [Fact]
    public void Sentencia_CrearConIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var juezId = Guid.NewGuid();
        var descripcion = "Sentencia en el caso 001/2025";
        var tipo = TipoSentencia.Condenatoria;

        // Act & Assert
        var act = () => Sentencia.Crear(Guid.Empty, juezId, descripcion, tipo);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID de la sentencia*vacío*");
    }

    [Fact]
    public void Sentencia_CrearConJuezIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Sentencia en el caso 001/2025";
        var tipo = TipoSentencia.Condenatoria;

        // Act & Assert
        var act = () => Sentencia.Crear(id, Guid.Empty, descripcion, tipo);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID del juez*obligatorio*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Sentencia_CrearConDescripcionInvalida_DeberiaLanzarExcepcion(string descripcion)
    {
        // Arrange
        var id = Guid.NewGuid();
        var juezId = Guid.NewGuid();
        var tipo = TipoSentencia.Condenatoria;

        // Act & Assert
        var act = () => Sentencia.Crear(id, juezId, descripcion, tipo);
        act.Should().Throw<DomainException>()
           .WithMessage("*descripción*sentencia*obligatoria*");
    }

    [Fact]
    public void Sentencia_AgregarResolutivo_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var sentencia = Sentencia.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sentencia en el caso 001/2025",
            TipoSentencia.Condenatoria
        );
        var contenido = "Se condena al acusado a 5 años de prisión";

        // Act
        sentencia.AgregarResolutivo(contenido);

        // Assert
        sentencia.Resolutivos.Should().HaveCount(1);
        var resolutivo = sentencia.Resolutivos[0];
        resolutivo.Numeracion.Should().Be("PRIMERO");
        resolutivo.Contenido.Should().Be(contenido);
        resolutivo.Orden.Should().Be(1);
    }

    [Fact]
    public void Sentencia_AgregarMultiplesResolutivos_DeberiaAsignarNumeracionCorrectamente()
    {
        // Arrange
        var sentencia = Sentencia.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sentencia en el caso 001/2025",
            TipoSentencia.Condenatoria
        );

        // Act
        sentencia.AgregarResolutivo("Primer resolutivo");
        sentencia.AgregarResolutivo("Segundo resolutivo");
        sentencia.AgregarResolutivo("Tercer resolutivo");

        // Assert
        sentencia.Resolutivos.Should().HaveCount(3);
        sentencia.Resolutivos[0].Numeracion.Should().Be("PRIMERO");
        sentencia.Resolutivos[1].Numeracion.Should().Be("SEGUNDO");
        sentencia.Resolutivos[2].Numeracion.Should().Be("TERCERO");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Sentencia_AgregarResolutivoInvalido_DeberiaLanzarExcepcion(string contenido)
    {
        // Arrange
        var sentencia = Sentencia.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sentencia en el caso 001/2025",
            TipoSentencia.Condenatoria
        );

        // Act & Assert
        var act = () => sentencia.AgregarResolutivo(contenido);
        act.Should().Throw<DomainException>()
           .WithMessage("*contenido*resolutivo*vacío*");
    }
}

public class ResolutivoTests
{
    [Fact]
    public void Resolutivo_CrearConDatosValidos_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var numeracion = "PRIMERO";
        var contenido = "Se condena al acusado";
        var orden = 1;

        // Act
        var resolutivo = Resolutivo.Crear(numeracion, contenido, orden);

        // Assert
        resolutivo.Should().NotBeNull();
        resolutivo.Id.Should().NotBe(Guid.Empty);
        resolutivo.Numeracion.Should().Be(numeracion);
        resolutivo.Contenido.Should().Be(contenido);
        resolutivo.Orden.Should().Be(orden);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Resolutivo_CrearConNumeracionInvalida_DeberiaLanzarExcepcion(string numeracion)
    {
        // Arrange
        var contenido = "Se condena al acusado";
        var orden = 1;

        // Act & Assert
        var act = () => Resolutivo.Crear(numeracion, contenido, orden);
        act.Should().Throw<DomainException>()
           .WithMessage("*numeración*resolutivo*obligatoria*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Resolutivo_CrearConContenidoInvalido_DeberiaLanzarExcepcion(string contenido)
    {
        // Arrange
        var numeracion = "PRIMERO";
        var orden = 1;

        // Act & Assert
        var act = () => Resolutivo.Crear(numeracion, contenido, orden);
        act.Should().Throw<DomainException>()
           .WithMessage("*contenido*resolutivo*obligatorio*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Resolutivo_CrearConOrdenInvalido_DeberiaLanzarExcepcion(int orden)
    {
        // Arrange
        var numeracion = "PRIMERO";
        var contenido = "Se condena al acusado";

        // Act & Assert
        var act = () => Resolutivo.Crear(numeracion, contenido, orden);
        act.Should().Throw<DomainException>()
           .WithMessage("*orden*resolutivo*mayor a cero*");
    }
}
