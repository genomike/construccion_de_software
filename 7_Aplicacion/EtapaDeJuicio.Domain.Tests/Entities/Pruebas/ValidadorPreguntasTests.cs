using FluentAssertions;
using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.Exceptions;
using Xunit;

namespace EtapaDeJuicio.Domain.Tests.Entities.Pruebas;

public class ValidadorPreguntasTests
{    [Theory]
    [InlineData("¿Qué observó ese día?", TipoInterrogatorio.Directo, true)]
    [InlineData("¿Podría decirme qué sucedió?", TipoInterrogatorio.Directo, true)]
    [InlineData("¿Dónde se encontraba usted?", TipoInterrogatorio.Directo, true)]
    [InlineData("¿Cuándo ocurrieron los hechos?", TipoInterrogatorio.Directo, true)]
    public void EsValida_ConPreguntasAbiertas_DeberiaRetornarTrue(
        string textoPregunta, TipoInterrogatorio tipo, bool esperado)
    {
        // Act
        var esValida = ValidadorPreguntas.EsValida(textoPregunta, tipo);

        // Assert
        esValida.Should().Be(esperado);
    }

    [Theory]
    [InlineData("¿No es cierto que usted estaba allí?", TipoInterrogatorio.Directo, false)]
    [InlineData("¿No cree que el acusado actuó mal?", TipoInterrogatorio.Directo, false)]
    [InlineData("¿Verdad que vio al sospechoso?", TipoInterrogatorio.Directo, false)]
    [InlineData("¿No le parece extraño que...?", TipoInterrogatorio.Directo, false)]
    public void EsValida_ConPreguntasSugestivas_DeberiaRetornarFalse(
        string textoPregunta, TipoInterrogatorio tipo, bool esperado)
    {
        // Act
        var esValida = ValidadorPreguntas.EsValida(textoPregunta, tipo);

        // Assert
        esValida.Should().Be(esperado);
    }

    [Theory]
    [InlineData("¿Usted miente frecuentemente?", TipoInterrogatorio.Directo, false)]
    [InlineData("¿Es usted una persona deshonesta?", TipoInterrogatorio.Directo, false)]
    [InlineData("¿Ha sido condenado por perjurio?", TipoInterrogatorio.Directo, false)]
    public void EsValida_ConPreguntasCapciosas_DeberiaRetornarFalse(
        string textoPregunta, TipoInterrogatorio tipo, bool esperado)
    {
        // Act
        var esValida = ValidadorPreguntas.EsValida(textoPregunta, tipo);

        // Assert
        esValida.Should().Be(esperado);
    }

    [Theory]
    [InlineData("¿Está usted seguro de que...?", TipoInterrogatorio.Contrainterrogatorio, true)]
    [InlineData("¿No recuerda haber dicho antes que...?", TipoInterrogatorio.Contrainterrogatorio, true)]
    [InlineData("¿Es correcto que usted declaró...?", TipoInterrogatorio.Contrainterrogatorio, true)]
    public void EsValida_ConPreguntasContrainterrogatorio_DeberiaPermitirMayorFlexibilidad(
        string textoPregunta, TipoInterrogatorio tipo, bool esperado)
    {
        // Act
        var esValida = ValidadorPreguntas.EsValida(textoPregunta, tipo);

        // Assert
        esValida.Should().Be(esperado);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void EsValida_ConPreguntaVacia_DeberiaRetornarFalse(string textoPregunta)
    {
        // Act
        var esValida = ValidadorPreguntas.EsValida(textoPregunta, TipoInterrogatorio.Directo);

        // Assert
        esValida.Should().BeFalse();
    }

    [Fact]
    public void EsValida_ConPreguntaMuyLarga_DeberiaRetornarFalse()
    {
        // Arrange
        var preguntaLarga = new string('a', 501); // Más de 500 caracteres

        // Act
        var esValida = ValidadorPreguntas.EsValida(preguntaLarga, TipoInterrogatorio.Directo);

        // Assert
        esValida.Should().BeFalse();
    }

    [Theory]
    [InlineData("¿Qué?", false)]
    [InlineData("¿Si?", false)]
    [InlineData("¿No?", false)]
    public void EsValida_ConPreguntasMuyCortas_DeberiaRetornarFalse(string textoPregunta, bool esperado)
    {
        // Act
        var esValida = ValidadorPreguntas.EsValida(textoPregunta, TipoInterrogatorio.Directo);

        // Assert
        esValida.Should().Be(esperado);
    }

    [Theory]
    [InlineData("Dígame qué pasó", false)] // No es pregunta
    [InlineData("Explíquenos los hechos", false)] // No es pregunta
    [InlineData("Cuéntenos sobre ese día", false)] // No es pregunta
    public void EsValida_ConAfirmacionesNoPreguntas_DeberiaRetornarFalse(string texto, bool esperado)
    {
        // Act
        var esValida = ValidadorPreguntas.EsValida(texto, TipoInterrogatorio.Directo);

        // Assert
        esValida.Should().Be(esperado);
    }

    [Fact]
    public void ValidarPregunta_ConPreguntaValida_NoDeberiaLanzarExcepcion()
    {
        // Arrange
        var pregunta = "¿Qué observó usted en el lugar de los hechos?";

        // Act & Assert
        var exception = Record.Exception(() => 
            ValidadorPreguntas.ValidarPregunta(pregunta, TipoInterrogatorio.Directo));
        
        exception.Should().BeNull();
    }

    [Fact]
    public void ValidarPregunta_ConPreguntaSugestiva_DeberiaLanzarExcepcion()
    {
        // Arrange
        var pregunta = "¿No es cierto que usted vio al acusado?";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            ValidadorPreguntas.ValidarPregunta(pregunta, TipoInterrogatorio.Directo));
        
        exception.Message.Should().Contain("sugestiva");
    }

    [Fact]
    public void ValidarPregunta_ConPreguntaCapciosa_DeberiaLanzarExcepcion()
    {
        // Arrange
        var pregunta = "¿Usted miente frecuentemente?";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            ValidadorPreguntas.ValidarPregunta(pregunta, TipoInterrogatorio.Directo));
        
        exception.Message.Should().Contain("capciosa");
    }

    [Fact]
    public void ObtenerTipoInvalidez_ConPreguntaSugestiva_DeberiaRetornarTipoCorrect()
    {
        // Arrange
        var pregunta = "¿No es cierto que usted estaba allí?";

        // Act
        var tipoInvalidez = ValidadorPreguntas.ObtenerTipoInvalidez(pregunta, TipoInterrogatorio.Directo);

        // Assert
        tipoInvalidez.Should().Be("Pregunta sugestiva");
    }

    [Fact]
    public void ObtenerTipoInvalidez_ConPreguntaCapciosa_DeberiaRetornarTipoCorrect()
    {
        // Arrange
        var pregunta = "¿Usted es una persona deshonesta?";

        // Act
        var tipoInvalidez = ValidadorPreguntas.ObtenerTipoInvalidez(pregunta, TipoInterrogatorio.Directo);

        // Assert
        tipoInvalidez.Should().Be("Pregunta capciosa");
    }

    [Fact]
    public void ObtenerTipoInvalidez_ConPreguntaValida_DeberiaRetornarNull()
    {
        // Arrange
        var pregunta = "¿Qué observó usted?";

        // Act
        var tipoInvalidez = ValidadorPreguntas.ObtenerTipoInvalidez(pregunta, TipoInterrogatorio.Directo);

        // Assert
        tipoInvalidez.Should().BeNull();
    }
}
