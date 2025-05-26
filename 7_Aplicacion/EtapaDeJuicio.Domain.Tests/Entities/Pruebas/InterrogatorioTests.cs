using FluentAssertions;
using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Tests.Entities.Pruebas;

public class InterrogatorioTests
{
    [Fact]
    public void Crear_ConDatosValidos_DeberiaCrearCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Interrogatorio principal";
        var fechaHora = DateTime.Now;
        var tipo = TipoInterrogatorio.Directo;

        // Act
        var interrogatorio = Interrogatorio.Crear(id, descripcion, fechaHora, tipo);

        // Assert
        interrogatorio.Should().NotBeNull();
        interrogatorio.Id.Should().Be(id);
        interrogatorio.Descripcion.Should().Be(descripcion);
        interrogatorio.FechaHora.Should().Be(fechaHora);
        interrogatorio.Tipo.Should().Be(tipo);
        interrogatorio.Preguntas.Should().BeEmpty();
        interrogatorio.EstaCompleto.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Crear_ConDescripcionVacia_DeberiaLanzarExcepcion(string descripcion)
    {
        // Arrange
        var id = Guid.NewGuid();
        var fechaHora = DateTime.Now;
        var tipo = TipoInterrogatorio.Directo;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            Interrogatorio.Crear(id, descripcion, fechaHora, tipo));
        
        exception.Message.Should().Contain("descripción");
    }

    [Fact]
    public void AgregarPregunta_ConPreguntaValida_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);
        
        var textoPregunta = "¿Qué observó usted en el lugar de los hechos?";

        // Act
        interrogatorio.AgregarPregunta(textoPregunta);

        // Assert
        interrogatorio.Preguntas.Should().HaveCount(1);
        interrogatorio.Preguntas.First().Texto.Should().Be(textoPregunta);
    }

    [Fact]
    public void AgregarPregunta_ConPreguntaSugestiva_DeberiaLanzarExcepcion()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);
        
        var preguntaSugestiva = "¿No es cierto que usted estaba en el lugar el día 15?";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            interrogatorio.AgregarPregunta(preguntaSugestiva));
        
        exception.Message.Should().Contain("sugestiva");
    }

    [Fact]
    public void AgregarPregunta_ConPreguntaCapciosa_DeberiaLanzarExcepcion()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);
        
        var preguntaCapciosa = "¿Usted miente frecuentemente?";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            interrogatorio.AgregarPregunta(preguntaCapciosa));
        
        exception.Message.Should().Contain("capciosa");
    }

    [Fact]
    public void AgregarPregunta_EnInterrogatorioCompleto_DeberiaLanzarExcepcion()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);
        
        interrogatorio.AgregarPregunta("¿Qué observó?");
        interrogatorio.MarcarComoCompleto();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            interrogatorio.AgregarPregunta("¿Cuándo fue?"));
        
        exception.Message.Should().Contain("completo");
    }

    [Fact]
    public void ResponderPregunta_ConRespuestaValida_DeberiaResponderCorrectamente()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);
        
        interrogatorio.AgregarPregunta("¿Qué observó?");
        var respuesta = "Vi al sospechoso salir del edificio";

        // Act
        interrogatorio.ResponderPregunta(0, respuesta);

        // Assert
        interrogatorio.Preguntas.First().Respuesta.Should().NotBeNull();
        interrogatorio.Preguntas.First().Respuesta.Texto.Should().Be(respuesta);
    }

    [Fact]
    public void ResponderPregunta_ConIndiceInvalido_DeberiaLanzarExcepcion()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);
        
        var respuesta = "Respuesta";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            interrogatorio.ResponderPregunta(0, respuesta));
        
        exception.Message.Should().Contain("índice");
    }

    [Fact]
    public void MarcarComoCompleto_ConTodasPreguntasRespondidas_DeberiaCompletarCorrectamente()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);
        
        interrogatorio.AgregarPregunta("¿Qué observó?");
        interrogatorio.ResponderPregunta(0, "Vi al sospechoso");

        // Act
        interrogatorio.MarcarComoCompleto();

        // Assert
        interrogatorio.EstaCompleto.Should().BeTrue();
        interrogatorio.FechaFinalizacion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void MarcarComoCompleto_SinPreguntas_DeberiaLanzarExcepcion()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            interrogatorio.MarcarComoCompleto());
        
        exception.Message.Should().Contain("al menos una pregunta");
    }

    [Fact]
    public void MarcarComoCompleto_ConPreguntasSinResponder_DeberiaLanzarExcepcion()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);
        
        interrogatorio.AgregarPregunta("¿Qué observó?");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            interrogatorio.MarcarComoCompleto());
        
        exception.Message.Should().Contain("sin responder");
    }

    [Fact]
    public void ObtenerPreguntasSinResponder_DeberiaRetornarPreguntasCorrects()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);
        
        interrogatorio.AgregarPregunta("¿Qué observó?");
        interrogatorio.AgregarPregunta("¿Cuándo fue?");
        interrogatorio.ResponderPregunta(0, "Vi algo");

        // Act
        var preguntasSinResponder = interrogatorio.ObtenerPreguntasSinResponder();

        // Assert
        preguntasSinResponder.Should().HaveCount(1);
        preguntasSinResponder.First().Texto.Should().Be("¿Cuándo fue?");
    }

    [Fact]
    public void CalcularPorcentajeCompletado_DeberiaCalcularCorrectamente()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);
        
        interrogatorio.AgregarPregunta("¿Qué observó?");
        interrogatorio.AgregarPregunta("¿Cuándo fue?");
        interrogatorio.ResponderPregunta(0, "Vi algo");

        // Act
        var porcentaje = interrogatorio.CalcularPorcentajeCompletado();

        // Assert
        porcentaje.Should().Be(50);
    }

    [Fact]
    public void CalcularDuracion_ConFechaFinalizacion_DeberiaCalcularCorrectamente()
    {
        // Arrange
        var fechaInicio = DateTime.Now;
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            fechaInicio, 
            TipoInterrogatorio.Directo);
        
        interrogatorio.AgregarPregunta("¿Qué observó?");
        interrogatorio.ResponderPregunta(0, "Vi algo");
        interrogatorio.MarcarComoCompleto();

        // Act
        var duracion = interrogatorio.CalcularDuracion();

        // Assert
        duracion.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public void CalcularDuracion_SinFechaFinalizacion_DeberiaRetornarNull()
    {
        // Arrange
        var interrogatorio = Interrogatorio.Crear(
            Guid.NewGuid(), 
            "Interrogatorio", 
            DateTime.Now, 
            TipoInterrogatorio.Directo);

        // Act
        var duracion = interrogatorio.CalcularDuracion();

        // Assert
        duracion.Should().BeNull();
    }
}
