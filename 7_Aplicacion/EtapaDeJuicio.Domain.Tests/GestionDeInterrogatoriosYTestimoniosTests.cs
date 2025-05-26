using EtapaDeJuicio.Domain.Entities.Audiencias;
using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using TipoInterrogatorioAudiencias = EtapaDeJuicio.Domain.Entities.Audiencias.TipoInterrogatorio;
using TipoInterrogatorioPruebas = EtapaDeJuicio.Domain.Entities.Pruebas.TipoInterrogatorio;
using ValidadorPreguntasAudiencias = EtapaDeJuicio.Domain.Entities.Audiencias.ValidadorPreguntas;
using ValidadorPreguntasPruebas = EtapaDeJuicio.Domain.Entities.Pruebas.ValidadorPreguntas;
using TestimonioPruebas = EtapaDeJuicio.Domain.Entities.Pruebas.Testimonio;
using PreguntaAudiencias = EtapaDeJuicio.Domain.Entities.Audiencias.Pregunta;
using InterrogatorioAudiencias = EtapaDeJuicio.Domain.Entities.Audiencias.Interrogatorio;
using InterrogatorioPruebas = EtapaDeJuicio.Domain.Entities.Pruebas.Interrogatorio;

namespace EtapaDeJuicio.Domain.Tests;

public class GestionDeInterrogatoriosYTestimoniosTests
{
    #region Pruebas de Validador de Preguntas

    [Fact]
    public void Pregunta_Sugestiva_NoEsPermitida()
    {        // Arrange
        var interrogatorio = InterrogatorioAudiencias.Crear(
            Guid.NewGuid(),
            "Testimonio principal",
            DateTime.Now,
            TipoInterrogatorioAudiencias.Directo
        );

        // Act & Assert
        var act = () => interrogatorio.AgregarPregunta("¿No es cierto que usted estaba en el lugar el día 15?");
        act.Should().Throw<DomainException>()
           .WithMessage("*sugestiva*improcedente*");
    }

    [Theory]
    [InlineData("¿Qué observó ese día?", true)]
    [InlineData("¿No cree que el acusado actuó mal?", false)]
    [InlineData("¿Podría decirme qué sucedió?", true)]
    [InlineData("¿No es verdad que usted mintió?", false)]
    public void ValidadorPreguntas_EvaluaDiferentesTipos(string textoPregunta, bool esperado)
    {
        // Act
        var esValida = ValidadorPreguntasPruebas.EsValida(textoPregunta, TipoInterrogatorioPruebas.Directo);
        
        // Assert
        esValida.Should().Be(esperado);
    }

    [Fact]
    public void ValidadorPreguntas_PreguntaSugestiva_EsDetectada()
    {
        // Arrange
        var preguntaSugestiva = "¿No es verdad que usted estaba allí?";
        
        // Act
        var esValida = ValidadorPreguntasPruebas.EsValida(preguntaSugestiva, TipoInterrogatorioPruebas.Directo);
        
        // Assert
        esValida.Should().BeFalse();
    }

    #endregion

    #region Pruebas de Entidad Interrogatorio

    [Fact]
    public void Interrogatorio_ConDatosValidos_CreaCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var titulo = "Interrogatorio del testigo principal";
        var fechaHora = DateTime.Now;
        var tipo = TipoInterrogatorioAudiencias.Directo;        // Act
        var interrogatorio = InterrogatorioAudiencias.Crear(id, titulo, fechaHora, tipo);

        // Assert
        interrogatorio.Id.Should().Be(id);
        interrogatorio.Titulo.Should().Be(titulo);
        interrogatorio.FechaHora.Should().BeCloseTo(fechaHora, TimeSpan.FromSeconds(1));
        interrogatorio.Tipo.Should().Be(tipo);
        interrogatorio.Preguntas.Should().BeEmpty();
        interrogatorio.EstaFinalizado.Should().BeFalse();
    }

    [Fact]
    public void Interrogatorio_SinTitulo_LanzaExcepcion()
    {
        // Arrange & Act & Assert
        var act = () => InterrogatorioAudiencias.Crear(
            Guid.NewGuid(), 
            "", 
            DateTime.Now, 
            TipoInterrogatorioAudiencias.Directo);
        act.Should().Throw<DomainException>()
           .WithMessage("*título*interrogatorio*obligatorio*");
    }

    [Fact]
    public void Interrogatorio_ConIdVacio_LanzaExcepcion()
    {
        // Arrange & Act & Assert
        var act = () => InterrogatorioAudiencias.Crear(
            Guid.Empty, 
            "Título válido", 
            DateTime.Now, 
            TipoInterrogatorioAudiencias.Directo);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID*interrogatorio*no puede estar vacío*");
    }

    [Fact]
    public void Interrogatorio_AgregarPreguntaValida_FuncionaCorrectamente()
    {
        // Arrange
        var interrogatorio = InterrogatorioAudiencias.Crear(
            Guid.NewGuid(),
            "Interrogatorio de prueba",
            DateTime.Now,
            TipoInterrogatorioAudiencias.Directo
        );
        var textoPregunta = "¿Qué observó en la escena del crimen?";

        // Act
        interrogatorio.AgregarPregunta(textoPregunta);

        // Assert
        interrogatorio.Preguntas.Should().HaveCount(1);
        interrogatorio.Preguntas.First().Texto.Should().Be(textoPregunta);
    }

    [Fact]
    public void Interrogatorio_AgregarPreguntaVacia_LanzaExcepcion()
    {
        // Arrange
        var interrogatorio = InterrogatorioAudiencias.Crear(
            Guid.NewGuid(),
            "Interrogatorio de prueba",
            DateTime.Now,
            TipoInterrogatorioAudiencias.Directo
        );

        // Act & Assert
        var act = () => interrogatorio.AgregarPregunta("");
        act.Should().Throw<DomainException>()
           .WithMessage("*texto de la pregunta*no puede estar vacío*");
    }

    [Fact]
    public void Interrogatorio_FinalizarInterrogatorio_FuncionaCorrectamente()
    {
        // Arrange
        var interrogatorio = InterrogatorioAudiencias.Crear(
            Guid.NewGuid(),
            "Interrogatorio de prueba",
            DateTime.Now,
            TipoInterrogatorioAudiencias.Directo
        );
        interrogatorio.AgregarPregunta("¿Qué vio ese día?");

        // Act
        interrogatorio.Finalizar();

        // Assert
        interrogatorio.EstaFinalizado.Should().BeTrue();
    }

    [Fact]
    public void Interrogatorio_AgregarPreguntaDespuesDeFinalizar_LanzaExcepcion()
    {
        // Arrange
        var interrogatorio = InterrogatorioAudiencias.Crear(
            Guid.NewGuid(),
            "Interrogatorio de prueba",
            DateTime.Now,
            TipoInterrogatorioAudiencias.Directo
        );
        interrogatorio.Finalizar();

        // Act & Assert
        var act = () => interrogatorio.AgregarPregunta("¿Nueva pregunta?");
        act.Should().Throw<DomainException>()
           .WithMessage("*No se pueden agregar preguntas*interrogatorio finalizado*");
    }

    #endregion

    #region Pruebas de Entidad Pregunta

    [Fact]
    public void Pregunta_ConDatosValidos_CreaCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var texto = "¿Qué observó en la escena?";

        // Act
        var pregunta = PreguntaAudiencias.Crear(id, texto, TipoPregunta.Abierta);

        // Assert
        pregunta.Id.Should().Be(id);
        pregunta.Texto.Should().Be(texto);
        pregunta.Respuesta.Should().BeNull();
    }

    [Fact]
    public void Pregunta_ConTextoVacio_LanzaExcepcion()
    {
        // Arrange & Act & Assert
        var act = () => PreguntaAudiencias.Crear(Guid.NewGuid(), "", TipoPregunta.Abierta);
        act.Should().Throw<DomainException>()
           .WithMessage("*texto de la pregunta*obligatorio*");
    }

    [Fact]
    public void Pregunta_AgregarRespuesta_FuncionaCorrectamente()
    {
        // Arrange
        var pregunta = PreguntaAudiencias.Crear(Guid.NewGuid(), "¿Qué vio?", TipoPregunta.Abierta);
        var respuesta = "Vi al acusado salir del edificio";

        // Act
        pregunta.Responder(respuesta);

        // Assert
        pregunta.Respuesta.Should().Be(respuesta);
    }

    [Fact]
    public void Pregunta_AgregarRespuestaVacia_LanzaExcepcion()
    {
        // Arrange
        var pregunta = PreguntaAudiencias.Crear(Guid.NewGuid(), "¿Qué vio?", TipoPregunta.Abierta);

        // Act & Assert
        var act = () => pregunta.Responder("");
        act.Should().Throw<DomainException>()
           .WithMessage("*respuesta*no puede estar vacía*");
    }

    #endregion

    #region Pruebas de Entidad Testimonio

    [Fact]
    public void Testimonio_ConDatosValidos_CreaCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var testigoId = Guid.NewGuid();
        var descripcion = "Testimonio del testigo presencial";
        var fechaHora = DateTime.Now;

        // Act
        var testimonio = TestimonioPruebas.Crear(id, testigoId, descripcion, fechaHora);

        // Assert
        testimonio.Id.Should().Be(id);
        testimonio.TestigoId.Should().Be(testigoId);
        testimonio.Descripcion.Should().Be(descripcion);
        testimonio.FechaHora.Should().BeCloseTo(fechaHora, TimeSpan.FromSeconds(1));
        testimonio.Declaraciones.Should().BeEmpty();
        testimonio.EstaCompleto.Should().BeFalse();
    }

    [Fact]
    public void Testimonio_SinTestigoId_LanzaExcepcion()
    {
        // Arrange & Act & Assert
        var act = () => TestimonioPruebas.Crear(
            Guid.NewGuid(), 
            Guid.Empty, 
            "Descripción válida", 
            DateTime.Now);
        act.Should().Throw<DomainException>()
           .WithMessage("*testigo*obligatorio*");
    }

    [Fact]
    public void Testimonio_SinDescripcion_LanzaExcepcion()
    {
        // Arrange & Act & Assert
        var act = () => TestimonioPruebas.Crear(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            "", 
            DateTime.Now);
        act.Should().Throw<DomainException>()
           .WithMessage("*descripción*testimonio*obligatoria*");
    }

    [Fact]
    public void Testimonio_AgregarDeclaracion_FuncionaCorrectamente()
    {
        // Arrange
        var testimonio = TestimonioPruebas.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Testimonio de prueba",
            DateTime.Now
        );
        var declaracion = "Vi al acusado en el lugar de los hechos";

        // Act
        testimonio.AgregarDeclaracion(declaracion);

        // Assert
        testimonio.Declaraciones.Should().HaveCount(1);
        testimonio.Declaraciones.Should().Contain(declaracion);
    }

    [Fact]
    public void Testimonio_AgregarDeclaracionVacia_LanzaExcepcion()
    {
        // Arrange
        var testimonio = TestimonioPruebas.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Testimonio de prueba",
            DateTime.Now
        );

        // Act & Assert
        var act = () => testimonio.AgregarDeclaracion("");
        act.Should().Throw<DomainException>()
           .WithMessage("*declaración*no puede estar vacía*");
    }

    [Fact]
    public void Testimonio_AgregarContenido_FuncionaCorrectamente()
    {
        // Arrange
        var testimonio = TestimonioPruebas.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Testimonio de prueba",
            DateTime.Now
        );
        var contenido = "Testimonio completo del testigo";

        // Act
        testimonio.AgregarContenido(contenido);

        // Assert
        testimonio.ContenidoTestimonio.Should().Be(contenido);
        testimonio.EstaCompleto.Should().BeTrue();
    }

    #endregion

    #region Pruebas de Contrainterrogatorio

    [Fact]
    public void Contrainterrogatorio_PermitePreguntasSugestivas()
    {
        // Arrange
        var interrogatorio = InterrogatorioAudiencias.Crear(
            Guid.NewGuid(),
            "Contrainterrogatorio",
            DateTime.Now,
            TipoInterrogatorioAudiencias.Contrainterrogatorio
        );

        // Act & Assert - No debería lanzar excepción en contrainterrogatorio
        var act = () => interrogatorio.AgregarPregunta("¿No es verdad que usted no estaba allí?");
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(TipoInterrogatorioPruebas.Directo, "¿No es cierto que mintió?", false)]
    [InlineData(TipoInterrogatorioPruebas.Contrainterrogatorio, "¿No es cierto que mintió?", true)]
    [InlineData(TipoInterrogatorioPruebas.Redirect, "¿Qué observó?", true)]
    [InlineData(TipoInterrogatorioPruebas.Recross, "¿No está confundido?", true)]
    public void ValidadorPreguntas_SegunTipoInterrogatorio_ValidaCorrectamente(
        TipoInterrogatorioPruebas tipo, string pregunta, bool esperado)
    {
        // Act
        var esValida = ValidadorPreguntasPruebas.EsValida(pregunta, tipo);

        // Assert
        esValida.Should().Be(esperado);
    }

    #endregion
}
