using EtapaDeJuicio.Domain.Entities.Pruebas;
using Audiencias = EtapaDeJuicio.Domain.Entities.Audiencias;
using EtapaDeJuicio.Domain.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace EtapaDeJuicio.Domain.Tests;

public class GestionDeInterrogatoriosTests
{
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
        var testimonio = Testimonio.Crear(id, testigoId, descripcion, fechaHora);

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
        var act = () => Testimonio.Crear(
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
        var act = () => Testimonio.Crear(
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
        var testimonio = Testimonio.Crear(
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
        var testimonio = Testimonio.Crear(
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
        var testimonio = Testimonio.Crear(
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
    public void Contrainterrogatorio_ConDatosValidos_CreaCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var interrogatorioOriginalId = Guid.NewGuid();
        var descripcion = "Contrainterrogatorio al testigo de la defensa";
        var fechaHora = DateTime.Now;        // Act
        var contrainterrogatorio = Audiencias.Contrainterrogatorio.Crear(id, interrogatorioOriginalId, descripcion, fechaHora);

        // Assert
        contrainterrogatorio.Id.Should().Be(id);
        contrainterrogatorio.InterrogatorioOriginalId.Should().Be(interrogatorioOriginalId);
        contrainterrogatorio.Descripcion.Should().Be(descripcion);
        contrainterrogatorio.FechaHora.Should().BeCloseTo(fechaHora, TimeSpan.FromSeconds(1));
        contrainterrogatorio.Preguntas.Should().BeEmpty();
        contrainterrogatorio.EstaCompleto.Should().BeFalse();
    }    [Fact]
    public void Contrainterrogatorio_SinInterrogatorioOriginalId_LanzaExcepcion()
    {
        // Arrange & Act & Assert
        var act = () => Audiencias.Contrainterrogatorio.Crear(
            Guid.NewGuid(),
            Guid.Empty,
            "Descripción válida",
            DateTime.Now);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID del interrogatorio original*vacío*");
    }    [Fact]
    public void Contrainterrogatorio_SinDescripcion_LanzaExcepcion()
    {
        // Arrange & Act & Assert
        var act = () => Audiencias.Contrainterrogatorio.Crear(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "",
            DateTime.Now);
        act.Should().Throw<DomainException>()
           .WithMessage("*descripción*contrainterrogatorio*obligatoria*");
    }

    [Fact]
    public void Contrainterrogatorio_AgregarPreguntaValida_FuncionaCorrectamente()
    {
        // Arrange
        var contrainterrogatorio = Audiencias.Contrainterrogatorio.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        var preguntaTexto = "¿Podría aclarar su declaración anterior?"; // Pregunta abierta, válida para contrainterrogatorio

        // Act
        contrainterrogatorio.AgregarPregunta(preguntaTexto);

        // Assert
        contrainterrogatorio.Preguntas.Should().HaveCount(1);
        contrainterrogatorio.Preguntas.First().Texto.Should().Be(preguntaTexto);
    }

    [Fact]
    public void Contrainterrogatorio_AgregarPreguntaSugestiva_LanzaExcepcion()
    {
        // Arrange
        var contrainterrogatorio = Audiencias.Contrainterrogatorio.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        var preguntaTexto = "¿No es cierto que usted estaba nervioso?"; // Pregunta sugestiva

        // Act & Assert
        var act = () => contrainterrogatorio.AgregarPregunta(preguntaTexto);
        act.Should().Throw<DomainException>()
           .WithMessage("*pregunta no es válida*contrainterrogatorio*");
    }

    [Fact]
    public void Contrainterrogatorio_MarcarComoCompleto_SinPreguntas_LanzaExcepcion()
    {
        // Arrange
        var contrainterrogatorio = Audiencias.Contrainterrogatorio.Crear(Guid.NewGuid(), "Test", DateTime.Now);

        // Act & Assert
        var act = () => contrainterrogatorio.MarcarComoCompleto();
        act.Should().Throw<DomainException>()
           .WithMessage("*al menos una pregunta*completado*");
    }

    [Fact]
    public void Contrainterrogatorio_MarcarComoCompleto_ConPreguntas_FuncionaCorrectamente()
    {
        // Arrange
        var contrainterrogatorio = Audiencias.Contrainterrogatorio.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        contrainterrogatorio.AgregarPregunta("¿Alguna otra cosa que agregar?");

        // Act
        contrainterrogatorio.MarcarComoCompleto();

        // Assert
        contrainterrogatorio.EstaCompleto.Should().BeTrue();
    }

    [Fact]
    public void Contrainterrogatorio_AgregarPregunta_AInterrogatorioCompleto_LanzaExcepcion()
    {
        // Arrange
        var contrainterrogatorio = Audiencias.Contrainterrogatorio.Crear(Guid.NewGuid(), "Test", DateTime.Now);
        contrainterrogatorio.AgregarPregunta("¿Está seguro de su respuesta?");
        contrainterrogatorio.MarcarComoCompleto();

        // Act & Assert
        var act = () => contrainterrogatorio.AgregarPregunta("¿Otra pregunta?");
        act.Should().Throw<DomainException>()
            .WithMessage("*preguntas*contrainterrogatorio completo*");
    }

    #endregion
}
