using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.Exceptions;
using EtapaDeJuicio.Domain.ValueObjects;
using EtapaDeJuicio.Domain.Interfaces.Services;
using FluentAssertions;
using TipoPruebaValueObject = EtapaDeJuicio.Domain.ValueObjects.TipoPrueba;
using TipoPruebaEntity = EtapaDeJuicio.Domain.Entities.Pruebas.TipoPrueba;

namespace EtapaDeJuicioTests;

public class GestionDePruebasJudicialesTests
{
    #region Pruebas de Value Objects

    [Theory]
    [InlineData("archivo.pdf", true)]
    [InlineData("archivo.exe", false)]
    [InlineData("", false)]
    [InlineData("documento.doc", true)]
    [InlineData("imagen.jpg", true)]
    [InlineData("malware.bat", false)]
    public void FormatoArchivo_ValidaCorrectamente(string nombreArchivo, bool esperado)
    {
        // Act
        var formatoValido = FormatoArchivo.EsValido(nombreArchivo);
        
        // Assert
        formatoValido.Should().Be(esperado);
    }

    [Fact]
    public void FormatoArchivo_CrearConArchivoValido_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var nombreArchivo = "contrato.pdf";
        
        // Act
        var formato = FormatoArchivo.Create(nombreArchivo);
        
        // Assert
        formato.Extension.Should().Be(".pdf");
    }    [Fact]
    public void FormatoArchivo_CrearConArchivoInvalido_DeberiaLanzarExcepcion()
    {
        // Arrange
        var nombreArchivo = "virus.exe";
        
        // Act & Assert
        Assert.Throws<DomainException>(() => FormatoArchivo.Create(nombreArchivo));
    }

    #endregion

    #region Pruebas de Entidades de Dominio - PruebaDocumental

    [Fact]
    public void PruebaDocumental_SinDescripcion_LanzaExcepcion()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            PruebaDocumental.Crear(Guid.NewGuid(), string.Empty, "archivo.pdf"));
        
        exception.Message.Should().Contain("descripción");
    }

    [Fact]
    public void PruebaDocumental_ConDatosValidos_CreaCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Contrato de arrendamiento";
        var rutaArchivo = "contrato.pdf";
        
        // Act
        var prueba = PruebaDocumental.Crear(id, descripcion, rutaArchivo);
        
        // Assert
        prueba.Id.Should().Be(id);
        prueba.Descripcion.Should().Be(descripcion);
        prueba.RutaArchivo.Should().Be(rutaArchivo);
        prueba.Tipo.Should().Be(TipoPruebaEntity.Documental);
        prueba.EsValida.Should().BeTrue();
    }    [Fact]
    public void PruebaDocumental_ConFormatoInvalido_LanzaExcepcion()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            PruebaDocumental.Crear(Guid.NewGuid(), "Descripción", "archivo.exe"));
        
        exception.Message.Should().Contain("no está permitido");
    }

    #endregion

    #region Pruebas de Entidades de Dominio - PruebaTestimonial

    [Fact]
    public void PruebaTestimonial_ConCredibilidadInvalida_LanzaExcepcion()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            PruebaTestimonial.Crear(Guid.NewGuid(), "Testimonio", Guid.Empty, CredibilidadTestigo.Alta));
        
        exception.Message.Should().Contain("ID del testigo es obligatorio");
    }

    [Fact]
    public void PruebaTestimonial_ConDatosValidos_CreaCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Testimonio del testigo presencial";
        var testigoId = Guid.NewGuid();
        var credibilidad = CredibilidadTestigo.Alta;
        
        // Act
        var prueba = PruebaTestimonial.Crear(id, descripcion, testigoId, credibilidad);
        
        // Assert
        prueba.Id.Should().Be(id);
        prueba.Descripcion.Should().Be(descripcion);
        prueba.TestigoId.Should().Be(testigoId);
        prueba.CredibilidadTestigo.Should().Be(credibilidad);
        prueba.Tipo.Should().Be(TipoPruebaEntity.Testimonial);
    }

    #endregion

    #region Pruebas de Entidades de Dominio - PruebaPericial

    [Fact]
    public void PruebaPericial_SinEspecialidad_LanzaExcepcion()
    {
        // Arrange & Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            PruebaPericial.Crear(Guid.NewGuid(), "Análisis forense", string.Empty));
        
        exception.Message.Should().Be("El nombre del experto certificado es obligatorio");
    }

    [Fact]
    public void PruebaPericial_ConDatosValidos_CreaCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Análisis balístico";
        var experto = "Dr. Juan Pérez";
        
        // Act
        var prueba = PruebaPericial.Crear(id, descripcion, experto);
        
        // Assert
        prueba.Id.Should().Be(id);
        prueba.Descripcion.Should().Be(descripcion);
        prueba.ExpertoCertificado.Should().Be(experto);
        prueba.Tipo.Should().Be(TipoPruebaEntity.Pericial);
    }

    #endregion

    #region Pruebas de Servicios de Dominio

    [Fact]
    public void ServicioValoracionPruebas_CalculaCorrectamente()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();        var pruebas = new List<PruebaJudicial>
        {
            PruebaDocumental.Crear(Guid.NewGuid(), "Documento oficial", "doc1.pdf"),
            PruebaTestimonial.Crear(Guid.NewGuid(), "Testimonio testigo presencial", Guid.NewGuid(), CredibilidadTestigo.Alta),
            PruebaPericial.Crear(Guid.NewGuid(), "Análisis pericial", "Experto certificado")
        };

        // Act
        decimal valorTotal = servicio.CalcularValorProbatorio(pruebas);

        // Assert
        valorTotal.Should().BeGreaterThan(0);
        valorTotal.Should().BeLessOrEqualTo(1); // Valor normalizado entre 0 y 1
    }

    [Fact]
    public void ServicioValoracionPruebas_SinPruebas_RetornaCero()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var pruebas = new List<PruebaJudicial>();

        // Act
        decimal valorTotal = servicio.CalcularValorProbatorio(pruebas);

        // Assert
        valorTotal.Should().Be(0);
    }

    [Fact]
    public void ServicioValoracionPruebas_ConPruebasInvalidas_RetornaCero()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var prueba = PruebaTestimonial.Crear(Guid.NewGuid(), "Testimonio", Guid.NewGuid(), CredibilidadTestigo.Alta);
        prueba.MarcarComoInvalida("Motivo de prueba");
        var pruebas = new List<PruebaJudicial> { prueba };

        // Act
        decimal valorTotal = servicio.CalcularValorProbatorio(pruebas);

        // Assert
        valorTotal.Should().Be(0);
    }

    [Fact]
    public void ServicioValoracionPruebas_ObtenerValoracionDetallada_RetornaDetalles()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();        var pruebas = new List<PruebaJudicial>
        {
            PruebaDocumental.Crear(Guid.NewGuid(), "Documento oficial", "doc1.pdf"),
            PruebaTestimonial.Crear(Guid.NewGuid(), "Testimonio", Guid.NewGuid(), CredibilidadTestigo.Media)
        };

        // Act
        var valoracionDetallada = servicio.ObtenerValoracionDetallada(pruebas);

        // Assert
        valoracionDetallada.Should().NotBeEmpty();
        valoracionDetallada.Should().HaveCount(2);
    }

    #endregion
}
