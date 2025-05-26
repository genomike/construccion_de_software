using FluentAssertions;
using EtapaDeJuicio.Domain.Entities.Sentencias;
using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Tests.Entities.Sentencias;

public class DeliberacionTests
{
    [Fact]
    public void Crear_ConDatosValidos_DeberiaCrearCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Deliberación sobre el caso principal";
        var fechaInicio = DateTime.Now;

        // Act
        var deliberacion = Deliberacion.Crear(id, descripcion, fechaInicio);

        // Assert
        deliberacion.Should().NotBeNull();
        deliberacion.Id.Should().Be(id);
        deliberacion.Descripcion.Should().Be(descripcion);
        deliberacion.FechaInicio.Should().Be(fechaInicio);
        deliberacion.Estado.Should().Be(EstadoDeliberacion.Iniciada);
        deliberacion.Considerandos.Should().BeEmpty();
        deliberacion.Valoraciones.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Crear_ConDescripcionVacia_DeberiaLanzarExcepcion(string descripcion)
    {
        // Arrange
        var id = Guid.NewGuid();
        var fechaInicio = DateTime.Now;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            Deliberacion.Crear(id, descripcion, fechaInicio));
        
        exception.Message.Should().Contain("descripción");
    }

    [Fact]
    public void AgregarConsiderando_ConDatosValidos_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        var fundamentoLegal = "Artículo 123 del Código Civil";
        var analisis = "El artículo establece claramente...";

        // Act
        deliberacion.AgregarConsiderando(fundamentoLegal, analisis);

        // Assert
        deliberacion.Considerandos.Should().HaveCount(1);
        var considerando = deliberacion.Considerandos.First();
        considerando.FundamentoLegal.Should().Be(fundamentoLegal);
        considerando.Analisis.Should().Be(analisis);
    }

    [Fact]
    public void AgregarConsiderando_EnDeliberacionFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        deliberacion.Finalizar();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            deliberacion.AgregarConsiderando("Art 123", "Análisis"));
        
        exception.Message.Should().Contain("finalizada");
    }

    [Fact]
    public void AgregarValoracion_ConPruebaValida_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Contrato", "contrato.pdf");
        var valoracion = 0.8m;
        var justificacion = "Documento auténtico y relevante";

        // Act
        deliberacion.AgregarValoracion(prueba, valoracion, justificacion);

        // Assert
        deliberacion.Valoraciones.Should().HaveCount(1);
        var valoracionAgregada = deliberacion.Valoraciones.First();
        valoracionAgregada.Prueba.Should().Be(prueba);
        valoracionAgregada.Valor.Should().Be(valoracion);
        valoracionAgregada.Justificacion.Should().Be(justificacion);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(2.0)]
    public void AgregarValoracion_ConValorFueraDeRango_DeberiaLanzarExcepcion(decimal valor)
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Test", "test.pdf");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            deliberacion.AgregarValoracion(prueba, valor, "Justificación"));
        
        exception.Message.Should().Contain("entre 0 y 1");
    }

    [Fact]
    public void CalcularValorTotalPruebas_ConMultiplesValoraciones_DeberiaCalcularCorrectamente()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        var prueba1 = PruebaDocumental.Crear(Guid.NewGuid(), "Doc1", "doc1.pdf");
        var prueba2 = PruebaTestimonial.Crear(Guid.NewGuid(), "Test1", Guid.NewGuid(), CredibilidadTestigo.Alta);
        
        deliberacion.AgregarValoracion(prueba1, 0.8m, "Justificación 1");
        deliberacion.AgregarValoracion(prueba2, 0.6m, "Justificación 2");

        // Act
        var valorTotal = deliberacion.CalcularValorTotalPruebas();

        // Assert
        valorTotal.Should().BeGreaterThan(0);
        valorTotal.Should().BeLessOrEqualTo(1.0m);
    }

    [Fact]
    public void IniciarAnalisis_DeberiaActualizarEstado()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);

        // Act
        deliberacion.IniciarAnalisis();

        // Assert
        deliberacion.Estado.Should().Be(EstadoDeliberacion.EnAnalisis);
    }

    [Fact]
    public void IniciarAnalisis_EnEstadoIncorrecto_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        deliberacion.Finalizar();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            deliberacion.IniciarAnalisis());
        
        exception.Message.Should().Contain("estado actual");
    }

    [Fact]
    public void Finalizar_ConDatosSuficientes_DeberiaFinalizarCorrectamente()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        deliberacion.AgregarConsiderando("Art 123", "Análisis legal");
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Doc", "doc.pdf");
        deliberacion.AgregarValoracion(prueba, 0.8m, "Justificación");

        // Act
        deliberacion.Finalizar();

        // Assert
        deliberacion.Estado.Should().Be(EstadoDeliberacion.Finalizada);
        deliberacion.FechaFin.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Finalizar_SinConsiderandos_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            deliberacion.Finalizar());
        
        exception.Message.Should().Contain("al menos un considerando");
    }

    [Fact]
    public void Pausar_DeberiaActualizarEstado()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        deliberacion.IniciarAnalisis();

        // Act
        deliberacion.Pausar();

        // Assert
        deliberacion.Estado.Should().Be(EstadoDeliberacion.Pausada);
    }

    [Fact]
    public void Reanudar_DesdePausada_DeberiaVolverAAnalisis()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        deliberacion.IniciarAnalisis();
        deliberacion.Pausar();

        // Act
        deliberacion.Reanudar();

        // Assert
        deliberacion.Estado.Should().Be(EstadoDeliberacion.EnAnalisis);
    }    [Fact]
    public void ObtenerDuracion_ConFechaFin_DeberiaCalcularCorrectamente()
    {
        // Arrange
        var fechaInicio = DateTime.Now.AddHours(-2);
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", fechaInicio);
        deliberacion.AgregarConsiderando("Art 123", "Análisis");
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Doc", "doc.pdf");
        deliberacion.AgregarValoracion(prueba, 0.8m, "Justificación");
        deliberacion.Finalizar();

        // Act
        var duracion = deliberacion.ObtenerDuracion();

        // Assert
        duracion.Should().BeGreaterThan(TimeSpan.Zero);
        duracion.Should().BeCloseTo(TimeSpan.FromHours(2), TimeSpan.FromMinutes(1));
    }    [Fact]
    public void ObtenerDuracion_SinFechaFin_DeberiaCalcularDuracionActual()
    {
        // Arrange
        var fechaInicio = DateTime.Now.AddMinutes(-30);
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", fechaInicio);

        // Act
        var duracion = deliberacion.ObtenerDuracion();

        // Assert
        duracion.Should().BeGreaterThan(TimeSpan.FromMinutes(25));
        duracion.Should().BeLessThan(TimeSpan.FromMinutes(35));
    }

    [Fact]
    public void ValidarCompletitud_ConDatosSuficientes_DeberiaRetornarTrue()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        deliberacion.AgregarConsiderando("Art 123", "Análisis legal");
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Doc", "doc.pdf");
        deliberacion.AgregarValoracion(prueba, 0.8m, "Justificación");

        // Act
        var esCompleta = deliberacion.ValidarCompletitud();

        // Assert
        esCompleta.Should().BeTrue();
    }

    [Fact]
    public void ValidarCompletitud_SinDatosSuficientes_DeberiaRetornarFalse()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        deliberacion.AgregarConsiderando("Art 123", "Análisis legal");
        // No agregar valoraciones

        // Act
        var esCompleta = deliberacion.ValidarCompletitud();

        // Assert
        esCompleta.Should().BeFalse();
    }

    [Fact]
    public void ObtenerResumen_DeberiaIncluirInformacionRelevante()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Deliberación importante", DateTime.Now);
        deliberacion.AgregarConsiderando("Art 123", "Análisis legal detallado");
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Contrato", "contrato.pdf");
        deliberacion.AgregarValoracion(prueba, 0.8m, "Documento válido");

        // Act
        var resumen = deliberacion.ObtenerResumen();

        // Assert
        resumen.Should().NotBeNull();
        resumen.Should().Contain("Deliberación importante");
        resumen.Should().Contain("1 considerando");
        resumen.Should().Contain("1 valoración");
    }

    [Theory]
    [InlineData(EstadoDeliberacion.Iniciada, true)]
    [InlineData(EstadoDeliberacion.EnAnalisis, true)]
    [InlineData(EstadoDeliberacion.Pausada, true)]
    [InlineData(EstadoDeliberacion.Finalizada, false)]
    public void PuedeModificar_SegunEstado_DeberiaRetornarResultadoCorrespondiente(
        EstadoDeliberacion estado, bool puedeModificar)
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        
        // Simular el estado requerido
        switch (estado)
        {
            case EstadoDeliberacion.EnAnalisis:
                deliberacion.IniciarAnalisis();
                break;
            case EstadoDeliberacion.Pausada:
                deliberacion.IniciarAnalisis();
                deliberacion.Pausar();
                break;
            case EstadoDeliberacion.Finalizada:
                deliberacion.AgregarConsiderando("Art 123", "Análisis");
                var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Doc", "doc.pdf");
                deliberacion.AgregarValoracion(prueba, 0.8m, "Justificación");
                deliberacion.Finalizar();
                break;
        }

        // Act
        var resultado = deliberacion.PuedeModificar();

        // Assert
        resultado.Should().Be(puedeModificar);
    }
}
