using FluentAssertions;
using EtapaDeJuicio.Domain.Entities.Sentencias;
using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Tests.Entities.Sentencias;

public class ServicioValoracionPruebasTests
{
    [Fact]
    public void CalcularValorTotal_ConListaVacia_DeberiaRetornarCero()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var pruebas = new List<PruebaJudicial>();

        // Act
        var valorTotal = servicio.CalcularValorTotal(pruebas);

        // Assert
        valorTotal.Should().Be(0m);
    }

    [Fact]
    public void CalcularValorTotal_ConUnaPrueba_DeberiaRetornarValorPrueba()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Test", "test.pdf");
        prueba.MarcarComoVerificada();
        var pruebas = new List<PruebaJudicial> { prueba };

        // Act
        var valorTotal = servicio.CalcularValorTotal(pruebas);

        // Assert
        valorTotal.Should().BeGreaterThan(0);
        valorTotal.Should().Be(prueba.CalcularValorProbatorio());
    }

    [Fact]
    public void CalcularValorTotal_ConMultiplesPruebas_DeberiaAplicarFormula()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var prueba1 = PruebaDocumental.Crear(Guid.NewGuid(), "Test1", "test1.pdf");
        prueba1.MarcarComoVerificada();
        
        var prueba2 = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Test2", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Alta);
        
        var pruebas = new List<PruebaJudicial> { prueba1, prueba2 };

        // Act
        var valorTotal = servicio.CalcularValorTotal(pruebas);

        // Assert
        valorTotal.Should().BeGreaterThan(0);
        valorTotal.Should().BeLessOrEqualTo(1.0m);
        
        // Verificar que no es simplemente suma
        var sumaSimple = prueba1.CalcularValorProbatorio() + prueba2.CalcularValorProbatorio();
        valorTotal.Should().BeLessThan(sumaSimple);
    }

    [Fact]
    public void CalcularValorPonderado_ConPesosPersonalizados_DeberiaAplicarPesos()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var prueba1 = PruebaDocumental.Crear(Guid.NewGuid(), "Test1", "test1.pdf");
        prueba1.MarcarComoVerificada();
        
        var prueba2 = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Test2", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Media);

        var pruebasConPesos = new Dictionary<PruebaJudicial, decimal>
        {
            { prueba1, 0.8m },
            { prueba2, 0.2m }
        };

        // Act
        var valorPonderado = servicio.CalcularValorPonderado(pruebasConPesos);

        // Assert
        valorPonderado.Should().BeGreaterThan(0);
        valorPonderado.Should().BeLessOrEqualTo(1.0m);
    }

    [Fact]
    public void CalcularValorPonderado_ConPesosSumaMayorAUno_DeberiaLanzarExcepcion()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var prueba1 = PruebaDocumental.Crear(Guid.NewGuid(), "Test1", "test1.pdf");
        var prueba2 = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Test2", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Media);

        var pruebasConPesos = new Dictionary<PruebaJudicial, decimal>
        {
            { prueba1, 0.8m },
            { prueba2, 0.7m } // Total = 1.5 > 1.0
        };

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            servicio.CalcularValorPonderado(pruebasConPesos));
        
        exception.Message.Should().Contain("pesos no puede exceder 1.0");
    }

    [Fact]
    public void ObtenerPruebasRelevantes_ConUmbralMinimo_DeberiaFiltrarCorrectamente()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var pruebaAlta = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "TestAlta", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Alta);
        
        var pruebaBaja = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "TestBaja", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Baja);

        var pruebas = new List<PruebaJudicial> { pruebaAlta, pruebaBaja };
        var umbral = 0.5m;

        // Act
        var pruebasRelevantes = servicio.ObtenerPruebasRelevantes(pruebas, umbral);

        // Assert
        pruebasRelevantes.Should().HaveCount(1);
        pruebasRelevantes.First().Should().Be(pruebaAlta);
    }

    [Fact]
    public void GenerarReporteValoracion_DeberiaIncluirDetalles()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var prueba1 = PruebaDocumental.Crear(Guid.NewGuid(), "Contrato", "contrato.pdf");
        prueba1.MarcarComoVerificada();
        
        var prueba2 = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Testimonio", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Alta);

        var pruebas = new List<PruebaJudicial> { prueba1, prueba2 };

        // Act
        var reporte = servicio.GenerarReporteValoracion(pruebas);

        // Assert
        reporte.Should().NotBeNull();
        reporte.Should().Contain("Contrato");
        reporte.Should().Contain("Testimonio");
        reporte.Should().Contain("Valor total");
    }

    [Fact]
    public void AplicarFactorConfiabilidad_ConPruebasConfiables_DeberiaAumentarValor()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Test", "test.pdf");
        prueba.MarcarComoVerificada();
        
        var valorOriginal = prueba.CalcularValorProbatorio();

        // Act
        var valorConFactor = servicio.AplicarFactorConfiabilidad(prueba, 1.2m);

        // Assert
        valorConFactor.Should().BeGreaterThan(valorOriginal);
        valorConFactor.Should().BeLessOrEqualTo(1.0m); // No debe exceder 1.0
    }

    [Fact]
    public void AplicarFactorConfiabilidad_ConFactorNegativo_DeberiaLanzarExcepcion()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var prueba = PruebaDocumental.Crear(Guid.NewGuid(), "Test", "test.pdf");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            servicio.AplicarFactorConfiabilidad(prueba, -0.5m));
        
        exception.Message.Should().Contain("factor de confiabilidad");
    }

    [Theory]
    [InlineData(0.0, "Nula")]
    [InlineData(0.3, "Baja")]
    [InlineData(0.6, "Media")]
    [InlineData(0.85, "Alta")]
    [InlineData(1.0, "Plena")]
    public void ClasificarFuerzaProbatoria_DeberiaClasicarCorrectamente(decimal valor, string clasificacionEsperada)
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();

        // Act
        var clasificacion = servicio.ClasificarFuerzaProbatoria(valor);

        // Assert
        clasificacion.Should().Be(clasificacionEsperada);
    }

    [Fact]
    public void EvaluarCoherenciaInterna_ConPruebasConsistentes_DeberiaRetornarAlto()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var prueba1 = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Testimonio consistente", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Alta);
        
        var prueba2 = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Otro testimonio consistente", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Alta);

        var pruebas = new List<PruebaJudicial> { prueba1, prueba2 };

        // Act
        var coherencia = servicio.EvaluarCoherenciaInterna(pruebas);

        // Assert
        coherencia.Should().BeGreaterThan(0.7m);
    }

    [Fact]
    public void CalcularIndiceCredibilidad_DeberiaConsiderarTiposYCalidades()
    {
        // Arrange
        var servicio = new ServicioValoracionPruebas();
        var pruebaDocumental = PruebaDocumental.Crear(Guid.NewGuid(), "Documento", "doc.pdf");
        pruebaDocumental.MarcarComoVerificada();
        
        var pruebaTestimonial = PruebaTestimonial.Crear(
            Guid.NewGuid(), 
            "Testimonio", 
            Guid.NewGuid(), 
            CredibilidadTestigo.Baja);

        var pruebas = new List<PruebaJudicial> { pruebaDocumental, pruebaTestimonial };

        // Act
        var indiceCredibilidad = servicio.CalcularIndiceCredibilidad(pruebas);

        // Assert
        indiceCredibilidad.Should().BeGreaterThan(0);
        indiceCredibilidad.Should().BeLessOrEqualTo(1.0m);
    }
}
