using EtapaDeJuicio.Domain.Entities.Sentencias;
using EtapaDeJuicio.Domain.Exceptions;
using FluentAssertions;

namespace EtapaDeJuicio.Domain.Tests;

public class GestionDeSentenciasTests
{
    #region Tests de Creación de Sentencia

    [Fact]
    public void Sentencia_CrearConDatosValidos_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var juezId = Guid.NewGuid();
        var descripcion = "Sentencia definitiva del caso 001/2025";
        var tipo = TipoSentencia.Definitiva;

        // Act
        var sentencia = Sentencia.Crear(id, juezId, descripcion, tipo);

        // Assert
        sentencia.Should().NotBeNull();
        sentencia.Id.Should().Be(id);
        sentencia.JuezId.Should().Be(juezId);
        sentencia.Descripcion.Should().Be(descripcion);
        sentencia.Tipo.Should().Be(tipo);
        sentencia.FechaEmision.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        sentencia.Resolutivos.Should().BeEmpty();
    }

    [Fact]
    public void Sentencia_CrearConIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var juezId = Guid.NewGuid();
        var descripcion = "Sentencia test";
        var tipo = TipoSentencia.Definitiva;

        // Act & Assert
        var act = () => Sentencia.Crear(Guid.Empty, juezId, descripcion, tipo);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID*sentencia*no puede estar vacío*");
    }

    [Fact]
    public void Sentencia_CrearConJuezIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Sentencia test";
        var tipo = TipoSentencia.Definitiva;

        // Act & Assert
        var act = () => Sentencia.Crear(id, Guid.Empty, descripcion, tipo);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID*juez*obligatorio*");
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
        var tipo = TipoSentencia.Definitiva;

        // Act & Assert
        var act = () => Sentencia.Crear(id, juezId, descripcion, tipo);
        act.Should().Throw<DomainException>()
           .WithMessage("*descripción*sentencia*obligatoria*");
    }

    [Theory]
    [InlineData(TipoSentencia.Definitiva)]
    [InlineData(TipoSentencia.Interlocutoria)]
    [InlineData(TipoSentencia.Absolutoria)]
    [InlineData(TipoSentencia.Condenatoria)]
    public void Sentencia_CrearConDiferentesTipos_DeberiaAsignarTipoCorrectamente(TipoSentencia tipo)
    {
        // Arrange
        var id = Guid.NewGuid();
        var juezId = Guid.NewGuid();
        var descripcion = $"Sentencia {tipo}";

        // Act
        var sentencia = Sentencia.Crear(id, juezId, descripcion, tipo);

        // Assert
        sentencia.Tipo.Should().Be(tipo);
    }

    #endregion

    #region Tests de Resolutivos

    [Fact]
    public void Resolutivo_CrearConDatosValidos_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var numeracion = "PRIMERO";
        var contenido = "SE DECLARA FUNDADA la demanda";
        var orden = 1;

        // Act
        var resolutivo = Resolutivo.Crear(numeracion, contenido, orden);

        // Assert
        resolutivo.Should().NotBeNull();
        resolutivo.Id.Should().NotBeEmpty();
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
        var contenido = "Contenido del resolutivo";
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
    [InlineData(-10)]
    public void Resolutivo_CrearConOrdenInvalido_DeberiaLanzarExcepcion(int orden)
    {
        // Arrange
        var numeracion = "PRIMERO";
        var contenido = "Contenido del resolutivo";

        // Act & Assert
        var act = () => Resolutivo.Crear(numeracion, contenido, orden);
        act.Should().Throw<DomainException>()
           .WithMessage("*orden*resolutivo*debe ser mayor a cero*");
    }

    #endregion

    #region Tests de Agregar Resolutivos a Sentencia

    [Fact]
    public void Sentencia_AgregarResolutivo_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var sentencia = CrearSentenciaBasica();
        var contenido = "SE DECLARA FUNDADA la demanda";

        // Act
        sentencia.AgregarResolutivo(contenido);

        // Assert
        sentencia.Resolutivos.Should().HaveCount(1);
        var resolutivo = sentencia.Resolutivos.First();
        resolutivo.Numeracion.Should().Be("PRIMERO");
        resolutivo.Contenido.Should().Be(contenido);
        resolutivo.Orden.Should().Be(1);
    }

    [Fact]
    public void Sentencia_AgregarMultiplesResolutivos_DeberiaAsignarNumeracionSecuencial()
    {
        // Arrange
        var sentencia = CrearSentenciaBasica();

        // Act
        sentencia.AgregarResolutivo("Primer resolutivo");
        sentencia.AgregarResolutivo("Segundo resolutivo");
        sentencia.AgregarResolutivo("Tercer resolutivo");

        // Assert
        sentencia.Resolutivos.Should().HaveCount(3);
        
        sentencia.Resolutivos[0].Numeracion.Should().Be("PRIMERO");
        sentencia.Resolutivos[0].Orden.Should().Be(1);
        
        sentencia.Resolutivos[1].Numeracion.Should().Be("SEGUNDO");
        sentencia.Resolutivos[1].Orden.Should().Be(2);
        
        sentencia.Resolutivos[2].Numeracion.Should().Be("TERCERO");
        sentencia.Resolutivos[2].Orden.Should().Be(3);
    }

    [Fact]
    public void Sentencia_AgregarMasDeDiezResolutivos_DeberiaUsarNumeracionArticulo()
    {
        // Arrange
        var sentencia = CrearSentenciaBasica();

        // Act - Agregar 11 resolutivos
        for (int i = 1; i <= 11; i++)
        {
            sentencia.AgregarResolutivo($"Resolutivo {i}");
        }

        // Assert
        sentencia.Resolutivos.Should().HaveCount(11);
        sentencia.Resolutivos[9].Numeracion.Should().Be("DÉCIMO");
        sentencia.Resolutivos[10].Numeracion.Should().Be("ARTÍCULO 11");
        sentencia.Resolutivos[10].Orden.Should().Be(11);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Sentencia_AgregarResolutivoConContenidoInvalido_DeberiaLanzarExcepcion(string contenido)
    {
        // Arrange
        var sentencia = CrearSentenciaBasica();

        // Act & Assert
        var act = () => sentencia.AgregarResolutivo(contenido);
        act.Should().Throw<DomainException>()
           .WithMessage("*contenido*resolutivo*no puede estar vacío*");
    }

    #endregion

    #region Tests de Notificación

    [Fact]
    public void Notificacion_CrearConDatosBasicos_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sentenciaId = Guid.NewGuid();
        var contenido = "Notificación de sentencia emitida";
        var fechaNotificacion = DateTime.UtcNow;

        // Act
        var notificacion = Notificacion.Crear(id, sentenciaId, contenido, fechaNotificacion);

        // Assert
        notificacion.Should().NotBeNull();
        notificacion.Id.Should().Be(id);
        notificacion.SentenciaId.Should().Be(sentenciaId);
        notificacion.ParteProcesal.Should().NotBeEmpty();
        notificacion.TipoParte.Should().Be(TipoParteProcesalEnum.Demandante);
        notificacion.Contenido.Should().Be(contenido);
        notificacion.FechaNotificacion.Should().Be(fechaNotificacion);
        notificacion.Entregada.Should().BeFalse();
    }

    [Fact]
    public void Notificacion_CrearConDatosCompletos_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sentenciaId = Guid.NewGuid();
        var parteProcesal = Guid.NewGuid();
        var tipoParte = TipoParteProcesalEnum.Demandado;
        var contenido = "Notificación de sentencia al demandado";
        var fechaNotificacion = DateTime.UtcNow;

        // Act
        var notificacion = Notificacion.Crear(id, sentenciaId, parteProcesal, tipoParte, contenido, fechaNotificacion);

        // Assert
        notificacion.Should().NotBeNull();
        notificacion.Id.Should().Be(id);
        notificacion.SentenciaId.Should().Be(sentenciaId);
        notificacion.ParteProcesal.Should().Be(parteProcesal);
        notificacion.TipoParte.Should().Be(tipoParte);
        notificacion.Contenido.Should().Be(contenido);
        notificacion.FechaNotificacion.Should().Be(fechaNotificacion);
        notificacion.Entregada.Should().BeFalse();
    }

    [Fact]
    public void Notificacion_CrearConIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var sentenciaId = Guid.NewGuid();
        var contenido = "Notificación test";
        var fechaNotificacion = DateTime.UtcNow;

        // Act & Assert
        var act = () => Notificacion.Crear(Guid.Empty, sentenciaId, contenido, fechaNotificacion);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID*notificación*no puede estar vacío*");
    }

    [Fact]
    public void Notificacion_CrearConSentenciaIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        var contenido = "Notificación test";
        var fechaNotificacion = DateTime.UtcNow;

        // Act & Assert
        var act = () => Notificacion.Crear(id, Guid.Empty, contenido, fechaNotificacion);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID*sentencia*obligatorio*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Notificacion_CrearConContenidoInvalido_DeberiaLanzarExcepcion(string contenido)
    {
        // Arrange
        var id = Guid.NewGuid();
        var sentenciaId = Guid.NewGuid();
        var fechaNotificacion = DateTime.UtcNow;

        // Act & Assert
        var act = () => Notificacion.Crear(id, sentenciaId, contenido, fechaNotificacion);
        act.Should().Throw<DomainException>()
           .WithMessage("*contenido*notificación*obligatorio*");
    }

    [Fact]
    public void Notificacion_CrearConParteProceseralVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        var sentenciaId = Guid.NewGuid();
        var tipoParte = TipoParteProcesalEnum.Demandado;
        var contenido = "Notificación test";
        var fechaNotificacion = DateTime.UtcNow;

        // Act & Assert
        var act = () => Notificacion.Crear(id, sentenciaId, Guid.Empty, tipoParte, contenido, fechaNotificacion);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID*parte procesal*obligatorio*");
    }

    [Theory]
    [InlineData(TipoParteProcesalEnum.Demandante)]
    [InlineData(TipoParteProcesalEnum.Demandado)]
    [InlineData(TipoParteProcesalEnum.Ministerio_Publico)]
    [InlineData(TipoParteProcesalEnum.Tercero_Interviniente)]
    public void Notificacion_CrearConDiferentesTiposDePartes_DeberiaAsignarTipoCorrectamente(TipoParteProcesalEnum tipoParte)
    {
        // Arrange
        var id = Guid.NewGuid();
        var sentenciaId = Guid.NewGuid();
        var parteProcesal = Guid.NewGuid();
        var contenido = $"Notificación para {tipoParte}";
        var fechaNotificacion = DateTime.UtcNow;

        // Act
        var notificacion = Notificacion.Crear(id, sentenciaId, parteProcesal, tipoParte, contenido, fechaNotificacion);

        // Assert
        notificacion.TipoParte.Should().Be(tipoParte);
    }

    #endregion

    #region Tests de Marcar Notificación como Entregada

    [Fact]
    public void Notificacion_MarcarComoEntregada_DeberiaActualizarEstado()
    {
        // Arrange
        var notificacion = CrearNotificacionBasica();
        notificacion.Entregada.Should().BeFalse();

        // Act
        notificacion.MarcarComoEntregada();

        // Assert
        notificacion.Entregada.Should().BeTrue();
    }

    [Fact]
    public void Notificacion_MarcarComoEntregadaVariasVeces_DeberiaMantenerse()
    {
        // Arrange
        var notificacion = CrearNotificacionBasica();

        // Act
        notificacion.MarcarComoEntregada();
        notificacion.MarcarComoEntregada();
        notificacion.MarcarComoEntregada();

        // Assert
        notificacion.Entregada.Should().BeTrue();
    }

    #endregion

    #region Tests de Integración - Sentencia con Notificaciones

    [Fact]
    public void Integracion_SentenciaConNotificacionesMultiples_DeberiaGestionarseCorrectamente()
    {
        // Arrange
        var sentencia = CrearSentenciaBasica();
        sentencia.AgregarResolutivo("SE DECLARA FUNDADA la demanda");
        sentencia.AgregarResolutivo("CONDÉNASE al demandado al pago de daños");

        var fechaNotificacion = DateTime.UtcNow.AddDays(1);

        // Act - Crear notificaciones para diferentes partes
        var notificacionDemandante = Notificacion.Crear(
            Guid.NewGuid(), 
            sentencia.Id, 
            Guid.NewGuid(), 
            TipoParteProcesalEnum.Demandante,
            "Notificación de sentencia favorable", 
            fechaNotificacion);

        var notificacionDemandado = Notificacion.Crear(
            Guid.NewGuid(), 
            sentencia.Id, 
            Guid.NewGuid(), 
            TipoParteProcesalEnum.Demandado,
            "Notificación de sentencia adversa", 
            fechaNotificacion);

        // Assert
        sentencia.Resolutivos.Should().HaveCount(2);
        
        notificacionDemandante.SentenciaId.Should().Be(sentencia.Id);
        notificacionDemandante.TipoParte.Should().Be(TipoParteProcesalEnum.Demandante);
        notificacionDemandante.Entregada.Should().BeFalse();

        notificacionDemandado.SentenciaId.Should().Be(sentencia.Id);
        notificacionDemandado.TipoParte.Should().Be(TipoParteProcesalEnum.Demandado);
        notificacionDemandado.Entregada.Should().BeFalse();

        // Act - Marcar una como entregada
        notificacionDemandante.MarcarComoEntregada();

        // Assert
        notificacionDemandante.Entregada.Should().BeTrue();
        notificacionDemandado.Entregada.Should().BeFalse();
    }

    [Fact]
    public void Integracion_SentenciaConMultiplesResolutivosYTipos_DeberiaManejarseTodosLosCasos()
    {
        // Arrange & Act
        var sentenciaDefinitiva = Sentencia.Crear(Guid.NewGuid(), Guid.NewGuid(), "Sentencia definitiva", TipoSentencia.Definitiva);
        var sentenciaInterlocutoria = Sentencia.Crear(Guid.NewGuid(), Guid.NewGuid(), "Sentencia interlocutoria", TipoSentencia.Interlocutoria);
        var sentenciaAbsolutoria = Sentencia.Crear(Guid.NewGuid(), Guid.NewGuid(), "Sentencia absolutoria", TipoSentencia.Absolutoria);
        var sentenciaCondenatoria = Sentencia.Crear(Guid.NewGuid(), Guid.NewGuid(), "Sentencia condenatoria", TipoSentencia.Condenatoria);

        // Agregar resolutivos a cada una
        sentenciaDefinitiva.AgregarResolutivo("Resolutivo definitivo");
        sentenciaInterlocutoria.AgregarResolutivo("Resolutivo interlocutorio");
        sentenciaAbsolutoria.AgregarResolutivo("SE ABSUELVE al acusado");
        sentenciaCondenatoria.AgregarResolutivo("SE CONDENA al acusado");

        // Assert
        sentenciaDefinitiva.Tipo.Should().Be(TipoSentencia.Definitiva);
        sentenciaDefinitiva.Resolutivos.Should().HaveCount(1);

        sentenciaInterlocutoria.Tipo.Should().Be(TipoSentencia.Interlocutoria);
        sentenciaInterlocutoria.Resolutivos.Should().HaveCount(1);

        sentenciaAbsolutoria.Tipo.Should().Be(TipoSentencia.Absolutoria);
        sentenciaAbsolutoria.Resolutivos.First().Contenido.Should().Contain("ABSUELVE");

        sentenciaCondenatoria.Tipo.Should().Be(TipoSentencia.Condenatoria);
        sentenciaCondenatoria.Resolutivos.First().Contenido.Should().Contain("CONDENA");
    }

    #endregion

    #region Métodos Auxiliares

    private static Sentencia CrearSentenciaBasica()
    {
        return Sentencia.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Sentencia de prueba",
            TipoSentencia.Definitiva);
    }

    private static Notificacion CrearNotificacionBasica()
    {
        return Notificacion.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Notificación de prueba",
            DateTime.UtcNow);
    }

    #endregion
}