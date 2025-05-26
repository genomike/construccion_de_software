using FluentAssertions;
using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Tests.Entities.Pruebas;

public class PruebaTestimonialTests
{
    [Fact]
    public void Crear_ConDatosValidos_DeberiaCrearCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Testimonio del testigo presencial";
        var idTestigo = Guid.NewGuid();
        var credibilidad = CredibilidadTestigo.Alta;

        // Act
        var prueba = PruebaTestimonial.Crear(id, descripcion, idTestigo, credibilidad);

        // Assert
        prueba.Should().NotBeNull();
        prueba.Id.Should().Be(id);
        prueba.Descripcion.Should().Be(descripcion);
        prueba.IdTestigo.Should().Be(idTestigo);
        prueba.Credibilidad.Should().Be(credibilidad);
        prueba.TipoPrueba.Should().Be(TipoPrueba.Testimonial);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Crear_ConDescripcionVacia_DeberiaLanzarExcepcion(string descripcion)
    {
        // Arrange
        var id = Guid.NewGuid();
        var idTestigo = Guid.NewGuid();
        var credibilidad = CredibilidadTestigo.Media;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            PruebaTestimonial.Crear(id, descripcion, idTestigo, credibilidad));
        
        exception.Message.Should().Contain("descripción");
    }

    [Fact]
    public void Crear_ConIdTestigoVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Descripción válida";
        var idTestigo = Guid.Empty;
        var credibilidad = CredibilidadTestigo.Media;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            PruebaTestimonial.Crear(id, descripcion, idTestigo, credibilidad));
        
        exception.Message.Should().Contain("ID del testigo");
    }

    [Theory]
    [InlineData(CredibilidadTestigo.Alta, 0.9)]
    [InlineData(CredibilidadTestigo.Media, 0.6)]
    [InlineData(CredibilidadTestigo.Baja, 0.3)]
    public void CalcularValorProbatorio_SegunCredibilidad_DeberiaRetornarValorCorrespondiente(
        CredibilidadTestigo credibilidad, decimal valorEsperado)
    {
        // Arrange
        var prueba = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Testimonio", 
            Guid.NewGuid(), 
            credibilidad);

        // Act
        var valorProbatorio = prueba.CalcularValorProbatorio();

        // Assert
        valorProbatorio.Should().BeApproximately(valorEsperado, 0.01m);
    }

    [Fact]
    public void ActualizarCredibilidad_ConNuevaCredibilidad_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var prueba = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Testimonio", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Media);
        
        var nuevaCredibilidad = CredibilidadTestigo.Alta;

        // Act
        prueba.ActualizarCredibilidad(nuevaCredibilidad);

        // Assert
        prueba.Credibilidad.Should().Be(nuevaCredibilidad);
    }

    [Fact]
    public void AgregarObservacion_ConObservacionValida_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var prueba = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Testimonio", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Media);
        
        var observacion = "El testigo mostró inconsistencias menores";

        // Act
        prueba.AgregarObservacion(observacion);

        // Assert
        prueba.Observaciones.Should().Contain(observacion);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void AgregarObservacion_ConObservacionVacia_DeberiaLanzarExcepcion(string observacion)
    {
        // Arrange
        var prueba = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Testimonio", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Media);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            prueba.AgregarObservacion(observacion));
        
        exception.Message.Should().Contain("observación");
    }

    [Fact]
    public void AgregarMultiplesObservaciones_DeberiaManenerOrden()
    {
        // Arrange
        var prueba = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Testimonio", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Media);
        
        var observacion1 = "Primera observación";
        var observacion2 = "Segunda observación";

        // Act
        prueba.AgregarObservacion(observacion1);
        prueba.AgregarObservacion(observacion2);

        // Assert
        prueba.Observaciones.Should().HaveCount(2);
        prueba.Observaciones.First().Should().Be(observacion1);
        prueba.Observaciones.Last().Should().Be(observacion2);
    }

    [Fact]
    public void EsConfiable_ConCredibilidadAlta_DeberiaRetornarTrue()
    {
        // Arrange
        var prueba = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Testimonio", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Alta);

        // Act
        var esConfiable = prueba.EsConfiable();

        // Assert
        esConfiable.Should().BeTrue();
    }

    [Fact]
    public void EsConfiable_ConCredibilidadBaja_DeberiaRetornarFalse()
    {
        // Arrange
        var prueba = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Testimonio", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Baja);

        // Act
        var esConfiable = prueba.EsConfiable();

        // Assert
        esConfiable.Should().BeFalse();
    }

    [Fact]
    public void ToString_DeberiaIncluirCredibilidad()
    {
        // Arrange
        var descripcion = "Testimonio del testigo";
        var credibilidad = CredibilidadTestigo.Alta;
        var prueba = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            descripcion, 
            Guid.NewGuid(), 
            credibilidad);

        // Act
        var resultado = prueba.ToString();

        // Assert
        resultado.Should().Contain(descripcion);
        resultado.Should().Contain(credibilidad.ToString());
    }
}
