using FluentAssertions;
using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Tests.Entities.Pruebas;

public class PruebaDocumentalTests
{
    [Fact]
    public void Crear_ConDatosValidos_DeberiaCrearCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Contrato de arrendamiento";
        var rutaArchivo = "contratos/contrato.pdf";

        // Act
        var prueba = PruebaDocumental.Crear(id, descripcion, rutaArchivo);

        // Assert
        prueba.Should().NotBeNull();
        prueba.Id.Should().Be(id);
        prueba.Descripcion.Should().Be(descripcion);
        prueba.RutaArchivo.Should().Be(rutaArchivo);
        prueba.TipoPrueba.Should().Be(TipoPrueba.Documental);
        prueba.FechaRegistro.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Crear_ConDescripcionVacia_DeberiaLanzarExcepcion(string descripcion)
    {
        // Arrange
        var id = Guid.NewGuid();
        var rutaArchivo = "test.pdf";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            PruebaDocumental.Crear(id, descripcion, rutaArchivo));
        
        exception.Message.Should().Contain("descripción");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Crear_ConRutaArchivoVacia_DeberiaLanzarExcepcion(string rutaArchivo)
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Descripción válida";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            PruebaDocumental.Crear(id, descripcion, rutaArchivo));
        
        exception.Message.Should().Contain("ruta del archivo");
    }

    [Fact]
    public void Crear_ConIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var id = Guid.Empty;
        var descripcion = "Descripción válida";
        var rutaArchivo = "test.pdf";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            PruebaDocumental.Crear(id, descripcion, rutaArchivo));
        
        exception.Message.Should().Contain("Id");
    }

    [Theory]
    [InlineData("archivo.pdf")]
    [InlineData("documento.docx")]
    [InlineData("imagen.jpg")]
    [InlineData("imagen.png")]
    public void Crear_ConFormatosValidos_DeberiaCrearCorrectamente(string rutaArchivo)
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Descripción válida";

        // Act
        var prueba = PruebaDocumental.Crear(id, descripcion, rutaArchivo);

        // Assert
        prueba.Should().NotBeNull();
        prueba.RutaArchivo.Should().Be(rutaArchivo);
    }

    [Theory]
    [InlineData("archivo.exe")]
    [InlineData("script.bat")]
    [InlineData("programa.com")]
    public void Crear_ConFormatosInvalidos_DeberiaLanzarExcepcion(string rutaArchivo)
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Descripción válida";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            PruebaDocumental.Crear(id, descripcion, rutaArchivo));
        
        exception.Message.Should().Contain("formato no está permitido");
    }

    [Fact]
    public void ActualizarDescripcion_ConDescripcionValida_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Descripción inicial", "test.pdf");
        var nuevaDescripcion = "Descripción actualizada";

        // Act
        prueba.ActualizarDescripcion(nuevaDescripcion);

        // Assert
        prueba.Descripcion.Should().Be(nuevaDescripcion);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void ActualizarDescripcion_ConDescripcionVacia_DeberiaLanzarExcepcion(string descripcion)
    {
        // Arrange
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Descripción inicial", "test.pdf");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            prueba.ActualizarDescripcion(descripcion));
        
        exception.Message.Should().Contain("descripción");
    }

    [Fact]
    public void MarcarComoVerificada_DeberiaActualizarEstado()
    {
        // Arrange
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Descripción", "test.pdf");

        // Act
        prueba.MarcarComoVerificada();

        // Assert
        prueba.EstaVerificada.Should().BeTrue();
        prueba.FechaVerificacion.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CalcularValorProbatorio_ConPruebaVerificada_DeberiaRetornarValorAlto()
    {
        // Arrange
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Descripción", "test.pdf");
        prueba.MarcarComoVerificada();

        // Act
        var valorProbatorio = prueba.CalcularValorProbatorio();

        // Assert
        valorProbatorio.Should().BeGreaterThan(0.7m);
    }

    [Fact]
    public void CalcularValorProbatorio_ConPruebaNoVerificada_DeberiaRetornarValorBajo()
    {
        // Arrange
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Descripción", "test.pdf");

        // Act
        var valorProbatorio = prueba.CalcularValorProbatorio();

        // Assert
        valorProbatorio.Should().BeLessOrEqualTo(0.5m);
    }

    [Fact]
    public void ToString_DeberiaRetornarDescripcionFormateada()
    {
        // Arrange
        var descripcion = "Contrato de arrendamiento";
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), descripcion, "test.pdf");

        // Act
        var resultado = prueba.ToString();

        // Assert
        resultado.Should().Contain(descripcion);
        resultado.Should().Contain("Documental");
    }
}
