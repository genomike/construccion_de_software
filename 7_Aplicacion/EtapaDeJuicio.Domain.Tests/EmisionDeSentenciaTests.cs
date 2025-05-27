using EtapaDeJuicio.Domain.Entities.Sentencias;
using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.Exceptions;
using FluentAssertions;

namespace EtapaDeJuicioTests;

public class EmisionDeSentenciaTests
{
    #region Pruebas de Emisión de Sentencia

    [Fact]
    public void EmisionSentencia_ConDeliberacionCompleta_DeberiaEmitirSentenciaCorrectamente()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "CASO-2025-001");
        deliberacion.AgregarConsiderando("CONSIDERANDO que las pruebas aportadas han sido debidamente valoradas");
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.8m, "Prueba documental confiable");
        deliberacion.Finalizar();

        var juezId = Guid.NewGuid();
        var descripcion = "Sentencia en el caso 001/2025";

        // Act
        var sentencia = Sentencia.Crear(Guid.NewGuid(), juezId, descripcion, TipoSentencia.Condenatoria);
        sentencia.AgregarResolutivo("Se condena al acusado a 5 años de prisión");

        // Assert
        sentencia.Should().NotBeNull();
        sentencia.JuezId.Should().Be(juezId);
        sentencia.Descripcion.Should().Be(descripcion);
        sentencia.Tipo.Should().Be(TipoSentencia.Condenatoria);
        sentencia.Resolutivos.Should().HaveCount(1);
        sentencia.FechaEmision.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void EmisionSentencia_ConDeliberacionIncompleta_DeberiaLanzarExcepcion()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "CASO-2025-001");
        // No se agregan considerandos ni valoraciones

        // Act & Assert
        var act = () => deliberacion.Finalizar();
        act.Should().Throw<DomainException>()
           .WithMessage("*No se puede finalizar*sin considerandos*");
    }

    [Fact]
    public void EmisionSentencia_ConMultiplesResolutivos_DeberiaAsignarNumeracionCorrectamente()
    {
        // Arrange
        var sentencia = Sentencia.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sentencia caso múltiple",
            TipoSentencia.Condenatoria
        );

        // Act
        sentencia.AgregarResolutivo("Se declara probada la responsabilidad penal del acusado");
        sentencia.AgregarResolutivo("Se condena al acusado a 8 años de prisión");
        sentencia.AgregarResolutivo("Se ordena el pago de reparación civil por $50,000");

        // Assert
        sentencia.Resolutivos.Should().HaveCount(3);
        sentencia.Resolutivos[0].Numeracion.Should().Be("PRIMERO");
        sentencia.Resolutivos[1].Numeracion.Should().Be("SEGUNDO");
        sentencia.Resolutivos[2].Numeracion.Should().Be("TERCERO");
    }

    [Theory]
    [InlineData(TipoSentencia.Absolutoria)]
    [InlineData(TipoSentencia.Condenatoria)]
    [InlineData(TipoSentencia.Definitiva)]
    [InlineData(TipoSentencia.Interlocutoria)]
    public void EmisionSentencia_ConDiferentesTipos_DeberiaCrearCorrectamente(TipoSentencia tipo)
    {
        // Arrange
        var juezId = Guid.NewGuid();
        var descripcion = $"Sentencia {tipo} en caso 001/2025";

        // Act
        var sentencia = Sentencia.Crear(Guid.NewGuid(), juezId, descripcion, tipo);

        // Assert
        sentencia.Should().NotBeNull();
        sentencia.Tipo.Should().Be(tipo);
        sentencia.JuezId.Should().Be(juezId);
        sentencia.Descripcion.Should().Be(descripcion);
    }

    #endregion

    #region Pruebas de Notificación

    [Fact]
    public void NotificacionSentencia_ConDatosValidos_DeberiaCrearCorrectamente()
    {
        // Arrange
        var sentenciaId = Guid.NewGuid();
        var parteProcesal = Guid.NewGuid();
        var contenido = "Se le notifica la sentencia condenatoria emitida";
        var fechaNotificacion = DateTime.Now;

        // Act
        var notificacion = Notificacion.Crear(
            Guid.NewGuid(),
            sentenciaId,
            parteProcesal,
            TipoParteProcesalEnum.Demandado,
            contenido,
            fechaNotificacion
        );

        // Assert
        notificacion.Should().NotBeNull();
        notificacion.SentenciaId.Should().Be(sentenciaId);
        notificacion.ParteProcesal.Should().Be(parteProcesal);
        notificacion.TipoParte.Should().Be(TipoParteProcesalEnum.Demandado);
        notificacion.Contenido.Should().Be(contenido);
        notificacion.FechaNotificacion.Should().Be(fechaNotificacion);
        notificacion.Entregada.Should().BeFalse();
    }

    [Fact]
    public void NotificacionSentencia_MarcarComoEntregada_DeberiaActualizarEstado()
    {
        // Arrange
        var notificacion = Notificacion.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            TipoParteProcesalEnum.Demandante,
            "Notificación de sentencia",
            DateTime.Now
        );

        // Act
        notificacion.MarcarComoEntregada();

        // Assert
        notificacion.Entregada.Should().BeTrue();
    }

    [Theory]
    [InlineData(TipoParteProcesalEnum.Demandante)]
    [InlineData(TipoParteProcesalEnum.Demandado)]
    [InlineData(TipoParteProcesalEnum.Ministerio_Publico)]
    [InlineData(TipoParteProcesalEnum.Tercero_Interviniente)]
    public void NotificacionSentencia_ConDiferentesTiposParteProcesalm_DeberiaCrearCorrectamente(TipoParteProcesalEnum tipoParte)
    {
        // Arrange
        var sentenciaId = Guid.NewGuid();
        var parteProcesal = Guid.NewGuid();
        var contenido = $"Notificación para {tipoParte}";

        // Act
        var notificacion = Notificacion.Crear(
            Guid.NewGuid(),
            sentenciaId,
            parteProcesal,
            tipoParte,
            contenido,
            DateTime.Now
        );

        // Assert
        notificacion.Should().NotBeNull();
        notificacion.TipoParte.Should().Be(tipoParte);
    }

    [Fact]
    public void NotificacionSentencia_ConSentenciaIdVacio_DeberiaLanzarExcepcion()
    {
        // Act & Assert
        var act = () => Notificacion.Crear(
            Guid.NewGuid(),
            Guid.Empty,
            Guid.NewGuid(),
            TipoParteProcesalEnum.Demandante,
            "Contenido",
            DateTime.Now
        );

        act.Should().Throw<DomainException>()
           .WithMessage("*ID de la sentencia*obligatorio*");
    }

    [Fact]
    public void NotificacionSentencia_ConContenidoVacio_DeberiaLanzarExcepcion()
    {
        // Act & Assert
        var act = () => Notificacion.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            TipoParteProcesalEnum.Demandante,
            "",
            DateTime.Now
        );

        act.Should().Throw<DomainException>()
           .WithMessage("*contenido*notificación*obligatorio*");
    }

    #endregion

    #region Pruebas de Integración Emisión-Deliberación

    [Fact]
    public void EmisionCompleta_DeliberacionAEmision_DeberiaProcesarCorrectamente()
    {
        // Arrange
        var deliberacion = Deliberacion.Crear(Guid.NewGuid(), "CASO-COMPLETO-2025");
        
        // Simular proceso de deliberación
        deliberacion.AgregarConsiderando("CONSIDERANDO los hechos probados");
        deliberacion.AgregarConsiderando("CONSIDERANDO la tipificación del delito");
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.9m, "Testimonio creíble");
        deliberacion.AgregarValoracionPrueba(Guid.NewGuid(), 0.8m, "Prueba documental fehaciente");
        deliberacion.Finalizar();

        // Act - Emisión de sentencia basada en deliberación
        var sentencia = Sentencia.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            $"Sentencia definitiva - {deliberacion.CasoNumero}",
            TipoSentencia.Definitiva
        );

        sentencia.AgregarResolutivo("Se declara culpable al acusado del delito de robo agravado");
        sentencia.AgregarResolutivo("Se condena a 10 años de prisión efectiva");
        sentencia.AgregarResolutivo("Se ordena reparación civil por $100,000");

        // Crear notificaciones
        var notificacionDemandado = Notificacion.Crear(
            Guid.NewGuid(),
            sentencia.Id,
            Guid.NewGuid(),
            TipoParteProcesalEnum.Demandado,
            "Se le notifica la sentencia condenatoria",
            DateTime.Now
        );

        var notificacionMinisterio = Notificacion.Crear(
            Guid.NewGuid(),
            sentencia.Id,
            Guid.NewGuid(),
            TipoParteProcesalEnum.Ministerio_Publico,
            "Se le notifica la sentencia favorable",
            DateTime.Now
        );

        // Assert
        deliberacion.EstaFinalizada.Should().BeTrue();
        deliberacion.Considerandos.Should().HaveCount(2);
        deliberacion.Valoraciones.Should().HaveCount(2);

        sentencia.Resolutivos.Should().HaveCount(3);
        sentencia.Tipo.Should().Be(TipoSentencia.Definitiva);

        notificacionDemandado.TipoParte.Should().Be(TipoParteProcesalEnum.Demandado);
        notificacionMinisterio.TipoParte.Should().Be(TipoParteProcesalEnum.Ministerio_Publico);
    }

    #endregion
}