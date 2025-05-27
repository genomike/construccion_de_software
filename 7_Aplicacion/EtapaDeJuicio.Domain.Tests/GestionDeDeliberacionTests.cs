using EtapaDeJuicio.Domain.Entities.Sentencias;
using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.Exceptions;
using FluentAssertions;

namespace EtapaDeJuicio.Domain.Tests;

public class GestionDeDeliberacionTests
{
    #region Tests de Creación de Deliberación

    [Fact]
    public void Deliberacion_CrearConDatosValidos_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Deliberación del caso 001/2025";
        var fechaInicio = DateTime.Now;

        // Act
        var deliberacion = Deliberacion.Crear(id, descripcion, fechaInicio);

        // Assert
        deliberacion.Should().NotBeNull();
        deliberacion.Id.Should().Be(id);
        deliberacion.Descripcion.Should().Be(descripcion);
        deliberacion.FechaInicio.Should().Be(fechaInicio);
        deliberacion.EstaFinalizada.Should().BeFalse();
        deliberacion.Estado.Should().Be(EstadoDeliberacion.Iniciada);
        deliberacion.Considerandos.Should().BeEmpty();
        deliberacion.Valoraciones.Should().BeEmpty();
    }

    [Fact]
    public void Deliberacion_CrearConIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var descripcion = "Deliberación caso test";
        var fechaInicio = DateTime.Now;

        // Act & Assert
        var act = () => Deliberacion.Crear(Guid.Empty, descripcion, fechaInicio);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID*deliberación*no puede estar vacío*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Deliberacion_CrearConDescripcionInvalida_DeberiaLanzarExcepcion(string descripcion)
    {
        // Arrange
        var id = Guid.NewGuid();
        var fechaInicio = DateTime.Now;

        // Act & Assert
        var act = () => Deliberacion.Crear(id, descripcion, fechaInicio);
        act.Should().Throw<DomainException>()
           .WithMessage("*descripción*deliberación*obligatoria*");
    }

    [Fact]
    public void Deliberacion_CrearConCasoNumero_DeberiaCrearseCorrectamente()
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
        deliberacion.Estado.Should().Be(EstadoDeliberacion.Iniciada);
    }

    #endregion

    #region Tests de Estados de Deliberación

    [Fact]
    public void Deliberacion_IniciarAnalisis_DeberiaActualizarEstado()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();

        // Act
        deliberacion.IniciarAnalisis();

        // Assert
        deliberacion.Estado.Should().Be(EstadoDeliberacion.EnAnalisis);
    }

    [Fact]
    public void Deliberacion_Pausar_DeberiaActualizarEstado()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.IniciarAnalisis();

        // Act
        deliberacion.Pausar();

        // Assert
        deliberacion.Estado.Should().Be(EstadoDeliberacion.Pausada);
    }

    [Fact]
    public void Deliberacion_Reanudar_DeberiaActualizarEstado()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.IniciarAnalisis();
        deliberacion.Pausar();

        // Act
        deliberacion.Reanudar();

        // Assert
        deliberacion.Estado.Should().Be(EstadoDeliberacion.EnAnalisis);
    }

    [Fact]
    public void Deliberacion_IniciarAnalisisEnDeliberacionFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarConsiderando("Considerando básico");
        deliberacion.Finalizar();

        // Act & Assert
        var act = () => deliberacion.IniciarAnalisis();
        act.Should().Throw<DomainException>()
           .WithMessage("*No se puede modificar*deliberación finalizada*");
    }

    [Fact]
    public void Deliberacion_PausarDeliberacionFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarConsiderando("Considerando básico");
        deliberacion.Finalizar();

        // Act & Assert
        var act = () => deliberacion.Pausar();
        act.Should().Throw<DomainException>()
           .WithMessage("*No se puede pausar*deliberación finalizada*");
    }

    [Fact]
    public void Deliberacion_ReanudarDeliberacionFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarConsiderando("Considerando básico");
        deliberacion.Finalizar();

        // Act & Assert
        var act = () => deliberacion.Reanudar();
        act.Should().Throw<DomainException>()
           .WithMessage("*No se puede reanudar*deliberación finalizada*");
    }

    #endregion

    #region Tests de Gestión de Considerandos

    [Fact]
    public void Deliberacion_AgregarConsiderando_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        var contenido = "CONSIDERANDO que las pruebas son fehacientes";

        // Act
        deliberacion.AgregarConsiderando(contenido);

        // Assert
        deliberacion.Considerandos.Should().HaveCount(1);
        deliberacion.Considerandos[0].Contenido.Should().Be(contenido);
        deliberacion.Considerandos[0].Orden.Should().Be(1);
    }

    [Fact]
    public void Deliberacion_AgregarMultiplesConsiderandos_DeberiaAsignarOrdenCorrectamente()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();

        // Act
        deliberacion.AgregarConsiderando("Primer considerando");
        deliberacion.AgregarConsiderando("Segundo considerando");
        deliberacion.AgregarConsiderando("Tercer considerando");

        // Assert
        deliberacion.Considerandos.Should().HaveCount(3);
        deliberacion.Considerandos[0].Orden.Should().Be(1);
        deliberacion.Considerandos[1].Orden.Should().Be(2);
        deliberacion.Considerandos[2].Orden.Should().Be(3);
    }

    [Fact]
    public void Deliberacion_AgregarConsiderandoConFundamentoYAnalisis_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        var fundamentoLegal = "Artículo 123 del Código Penal";
        var analisis = "Análisis detallado del fundamento";

        // Act
        deliberacion.AgregarConsiderando(fundamentoLegal, analisis);

        // Assert
        deliberacion.Considerandos.Should().HaveCount(1);
        var considerando = deliberacion.Considerandos[0];
        considerando.FundamentoLegal.Should().Be(fundamentoLegal);
        considerando.Analisis.Should().Be(analisis);
        considerando.Orden.Should().Be(1);
    }

    [Fact]
    public void Deliberacion_AgregarConsiderandoEnDeliberacionFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarConsiderando("Considerando inicial");
        deliberacion.Finalizar();

        // Act & Assert
        var act = () => deliberacion.AgregarConsiderando("Nuevo considerando");
        act.Should().Throw<DomainException>()
           .WithMessage("*No se pueden agregar considerandos*deliberación finalizada*");
    }

    #endregion

    #region Tests de Valoración de Pruebas

    [Fact]
    public void Deliberacion_AgregarValoracionPrueba_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        var pruebaId = Guid.NewGuid();
        var valor = 0.85m;
        var justificacion = "Testimonio creíble y coherente";

        // Act
        deliberacion.AgregarValoracionPrueba(pruebaId, valor, justificacion);

        // Assert
        deliberacion.Valoraciones.Should().HaveCount(1);
        var valoracion = deliberacion.Valoraciones[0];
        valoracion.PruebaId.Should().Be(pruebaId);
        valoracion.Valor.Should().Be(valor);
        valoracion.Justificacion.Should().Be(justificacion);
    }

    [Fact]
    public void Deliberacion_AgregarValoracionConPruebaJudicial_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Prueba documental","ContenidoDelDocumento.pdf");
        var valoracion = 0.90m;
        var justificacion = "Documento auténtico y relevante";

        // Act
        deliberacion.AgregarValoracion(prueba, valoracion, justificacion);

        // Assert
        deliberacion.ValoracionesPruebas.Should().HaveCount(1);
        var valoracionAgregada = deliberacion.ValoracionesPruebas[0];
        valoracionAgregada.Valor.Should().Be(valoracion);
        valoracionAgregada.Justificacion.Should().Be(justificacion);
    }

    [Fact]
    public void Deliberacion_AgregarValoracionEnDeliberacionFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarConsiderando("Considerando inicial");
        deliberacion.Finalizar();

        // Act & Assert
        var act = () => deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.5m, "Test");
        act.Should().Throw<DomainException>()
           .WithMessage("*No se pueden agregar valoraciones*deliberación finalizada*");
    }

    #endregion

    #region Tests de Generación de Considerandos

    [Fact]
    public void Deliberacion_GenerarConsiderandos_ConPruebasValoradas_DeberiaGenerarCorrectamente()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.8m, "Prueba relevante");

        // Act
        deliberacion.GenerarConsiderandos();

        // Assert
        deliberacion.Considerandos.Should().HaveCount(1);
        deliberacion.Considerandos[0].Contenido.Should().Contain("pruebas aportadas han sido debidamente valoradas");
    }

    [Fact]
    public void Deliberacion_GenerarConsiderandosSinPruebasValoradas_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();

        // Act & Assert
        var act = () => deliberacion.GenerarConsiderandos();
        act.Should().Throw<DomainException>()
           .WithMessage("*No se puede generar considerandos sin pruebas valoradas*");
    }

    [Fact]
    public void Deliberacion_GenerarConsiderandosEnDeliberacionFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.8m, "Prueba relevante");
        deliberacion.AgregarConsiderando("Considerando inicial");
        deliberacion.Finalizar();

        // Act & Assert
        var act = () => deliberacion.GenerarConsiderandos();
        act.Should().Throw<DomainException>()
           .WithMessage("*No se pueden generar considerandos*deliberación finalizada*");
    }

    #endregion

    #region Tests de Cálculos y Métricas

    [Fact]
    public void Deliberacion_CalcularValorTotalPruebas_SinPruebas_DeberiaRetornarCero()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();

        // Act
        var valorTotal = deliberacion.CalcularValorTotalPruebas();

        // Assert
        valorTotal.Should().Be(0m);
    }

    [Fact]
    public void Deliberacion_CalcularValorTotalPruebas_ConVariasPruebas_DeberiaCalcularPromedio()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.8m, "Primera prueba");
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.6m, "Segunda prueba");
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 1.0m, "Tercera prueba");

        // Act
        var valorTotal = deliberacion.CalcularValorTotalPruebas();

        // Assert
        var promedioEsperado = (0.8m + 0.6m + 1.0m) / 3;
        valorTotal.Should().BeApproximately(promedioEsperado, 0.01m);
    }

    [Fact]
    public void Deliberacion_ObtenerDuracion_DeliberacionSinFinalizar_DeberiaCalcularDuracionHastaAhora()
    {
        // Arrange
        var fechaInicio = DateTime.Now.AddHours(-2);
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", fechaInicio);

        // Act
        var duracion = deliberacion.ObtenerDuracion();

        // Assert
        duracion.Should().BeCloseTo(TimeSpan.FromHours(2), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Deliberacion_ObtenerDuracion_DeliberacionFinalizada_DeberiaCalcularDuracionTotal()
    {
        // Arrange
        var fechaInicio = DateTime.Now.AddHours(-3);
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "Test", fechaInicio);
        deliberacion.AgregarConsiderando("Considerando test");
        deliberacion.Finalizar();

        // Act
        var duracion = deliberacion.ObtenerDuracion();

        // Assert
        duracion.Should().BeCloseTo(TimeSpan.FromHours(3), TimeSpan.FromMinutes(1));
        deliberacion.FechaFinalizacion.Should().NotBeNull();
    }

    #endregion

    #region Tests de Validación y Finalización

    [Fact]
    public void Deliberacion_ValidarCompletitud_ConConsiderandosYValoraciones_DeberiaRetornarTrue()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarConsiderando("Considerando legal");
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.8m, "Valoración prueba");

        // Act
        var esCompleta = deliberacion.ValidarCompletitud();

        // Assert
        esCompleta.Should().BeTrue();
    }

    [Fact]
    public void Deliberacion_ValidarCompletitud_SinConsiderandos_DeberiaRetornarFalse()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.8m, "Valoración prueba");

        // Act
        var esCompleta = deliberacion.ValidarCompletitud();

        // Assert
        esCompleta.Should().BeFalse();
    }

    [Fact]
    public void Deliberacion_ValidarCompletitud_SinValoraciones_DeberiaRetornarFalse()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarConsiderando("Considerando legal");

        // Act
        var esCompleta = deliberacion.ValidarCompletitud();

        // Assert
        esCompleta.Should().BeFalse();
    }

    [Fact]
    public void Deliberacion_Finalizar_ConConsiderandos_DeberiaFinalizarCorrectamente()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarConsiderando("Considerando necesario");

        // Act
        deliberacion.Finalizar();

        // Assert
        deliberacion.EstaFinalizada.Should().BeTrue();
        deliberacion.Estado.Should().Be(EstadoDeliberacion.Finalizada);
        deliberacion.FechaFin.Should().NotBeNull();
        deliberacion.FechaFin.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Deliberacion_Finalizar_SinConsiderandos_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();

        // Act & Assert
        var act = () => deliberacion.Finalizar();
        act.Should().Throw<DomainException>()
           .WithMessage("*No se puede finalizar*deliberación sin considerandos*");
    }

    [Fact]
    public void Deliberacion_PuedeModificar_DeliberacionActiva_DeberiaRetornarTrue()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();

        // Act
        var puedeModificar = deliberacion.PuedeModificar();

        // Assert
        puedeModificar.Should().BeTrue();
    }

    [Fact]
    public void Deliberacion_PuedeModificar_DeliberacionFinalizada_DeberiaRetornarFalse()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarConsiderando("Considerando necesario");
        deliberacion.Finalizar();

        // Act
        var puedeModificar = deliberacion.PuedeModificar();

        // Assert
        puedeModificar.Should().BeFalse();
    }

    #endregion

    #region Tests de Utilidades y Resumen

    [Fact]
    public void Deliberacion_ObtenerResumen_DeberiaGenerarResumenCompleto()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();
        deliberacion.AgregarConsiderando("Primer considerando");
        deliberacion.AgregarConsiderando("Segundo considerando");
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.8m, "Primera valoración");
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.9m, "Segunda valoración");

        // Act
        var resumen = deliberacion.ObtenerResumen();

        // Assert
        resumen.Should().Contain(deliberacion.Descripcion);
        resumen.Should().Contain("Considerandos: 2");
        resumen.Should().Contain("Valoraciones: 2");
        resumen.Should().Contain("Valor promedio: 0.85");
        resumen.Should().Contain($"Estado: {deliberacion.Estado}");
    }

    [Fact]
    public void Deliberacion_ObtenerResumen_DeliberacionVacia_DeberiaGenerarResumenBasico()
    {
        // Arrange
        var deliberacion = CrearDeliberacionBasica();

        // Act
        var resumen = deliberacion.ObtenerResumen();

        // Assert
        resumen.Should().Contain(deliberacion.Descripcion);
        resumen.Should().Contain("Considerandos: 0");
        resumen.Should().Contain("Valoraciones: 0");
        resumen.Should().Contain("Valor promedio: 0.00");
        resumen.Should().Contain("Estado: Iniciada");
    }

    #endregion

    #region Métodos Auxiliares

    private static Deliberacion CrearDeliberacionBasica()
    {
        return Deliberacion.Crear(
            Guid.NewGuid(),
            "Deliberación de prueba",
            DateTime.Now
        );
    }    private static PruebaJudicial CrearPruebaBasica()
    {
        return PruebaDocumental.Crear(
            Guid.NewGuid(),
            "Prueba de test",
            "documento_test.pdf"
        );
    }

    #endregion
}