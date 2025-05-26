using FluentAssertions;
using EtapaDeJuicio.Domain.ValueObjects;
using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Tests.ValueObjects;

public class FormatoArchivoTests
{    [Fact]
    public void Crear_ConFormatoValido_DeberiaCrearCorrectamente()
    {
        // Arrange
        var archivo = "documento.pdf";

        // Act
        var formato = FormatoArchivo.Crear(archivo);

        // Assert
        formato.Should().NotBeNull();
        formato.NombreArchivo.Should().Be(archivo);
        formato.Extension.Should().Be(".pdf"); // Incluir el punto
    }

    [Theory]
    [InlineData("archivo.pdf")]
    [InlineData("documento.docx")]
    [InlineData("imagen.jpg")]
    [InlineData("imagen.png")]
    [InlineData("video.mp4")]
    [InlineData("audio.mp3")]
    public void Crear_ConFormatosPermitidos_DeberiaCrearCorrectamente(string nombreArchivo)
    {
        // Act
        var formato = FormatoArchivo.Crear(nombreArchivo);

        // Assert
        formato.Should().NotBeNull();
        formato.NombreArchivo.Should().Be(nombreArchivo);
    }

    [Theory]
    [InlineData("archivo.exe")]
    [InlineData("script.bat")]
    [InlineData("programa.com")]
    [InlineData("virus.scr")]
    public void Crear_ConFormatosProhibidos_DeberiaLanzarExcepcion(string nombreArchivo)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => FormatoArchivo.Crear(nombreArchivo));
        exception.Message.Should().Contain("formato no permitido");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Crear_ConNombreVacio_DeberiaLanzarExcepcion(string nombreArchivo)
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => FormatoArchivo.Crear(nombreArchivo));
        exception.Message.Should().Contain("nombre del archivo");
    }

    [Fact]
    public void Crear_ConArchivoSinExtension_DeberiaLanzarExcepcion()
    {
        // Arrange
        var archivo = "archivo_sin_extension";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => FormatoArchivo.Crear(archivo));
        exception.Message.Should().Contain("extensión válida");
    }

    [Theory]
    [InlineData("archivo.PDF")]
    [InlineData("documento.DOCX")]
    [InlineData("imagen.JPG")]
    public void Crear_ConExtensionEnMayusculas_DeberiaCrearCorrectamente(string nombreArchivo)
    {
        // Act
        var formato = FormatoArchivo.Crear(nombreArchivo);

        // Assert
        formato.Should().NotBeNull();
        formato.Extension.Should().Be(Path.GetExtension(nombreArchivo).TrimStart('.').ToLower());
    }

    [Fact]
    public void EsValido_ConFormatoPermitido_DeberiaRetornarTrue()
    {
        // Arrange
        var nombreArchivo = "test.pdf";

        // Act
        var esValido = FormatoArchivo.EsValido(nombreArchivo);

        // Assert
        esValido.Should().BeTrue();
    }

    [Fact]
    public void EsValido_ConFormatoProhibido_DeberiaRetornarFalse()
    {
        // Arrange
        var nombreArchivo = "test.exe";

        // Act
        var esValido = FormatoArchivo.EsValido(nombreArchivo);

        // Assert
        esValido.Should().BeFalse();
    }

    [Fact]
    public void Equals_ConMismoArchivo_DeberiaRetornarTrue()
    {
        // Arrange
        var formato1 = FormatoArchivo.Crear("test.pdf");
        var formato2 = FormatoArchivo.Crear("test.pdf");

        // Act & Assert
        formato1.Should().Be(formato2);
        formato1.GetHashCode().Should().Be(formato2.GetHashCode());
    }

    [Fact]
    public void Equals_ConDiferenteArchivo_DeberiaRetornarFalse()
    {
        // Arrange
        var formato1 = FormatoArchivo.Crear("test1.pdf");
        var formato2 = FormatoArchivo.Crear("test2.pdf");

        // Act & Assert
        formato1.Should().NotBe(formato2);
    }
}
