using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.Exceptions;
using FluentAssertions;

namespace EtapaDeJuicio.Domain.Tests.Entities.Pruebas;

public class PruebaPericialTests
{
    [Fact]
    public void PruebaPericial_CrearConDatosValidos_DeberiaCrearseCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Análisis de huellas dactilares";
        var peritoId = Guid.NewGuid();
        var expertoCertificado = "Dr. Juan Pérez";
        var especialidad = "Dactiloscopía";

        // Act
        var prueba = PruebaPericial.Crear(id, descripcion, peritoId, expertoCertificado, especialidad);

        // Assert
        prueba.Should().NotBeNull();
        prueba.Id.Should().Be(id);
        prueba.Descripcion.Should().Be(descripcion);
        prueba.PeritoId.Should().Be(peritoId);
        prueba.ExpertoCertificado.Should().Be(expertoCertificado);
        prueba.Especialidad.Should().Be(especialidad);
        prueba.InformePericial.Should().BeEmpty();
    }

    [Fact]
    public void PruebaPericial_CrearConPeritoIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Análisis de huellas dactilares";
        var expertoCertificado = "Dr. Juan Pérez";
        var especialidad = "Dactiloscopía";

        // Act & Assert
        var act = () => PruebaPericial.Crear(id, descripcion, Guid.Empty, expertoCertificado, especialidad);
        act.Should().Throw<DomainException>()
           .WithMessage("*perito*obligatorio*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void PruebaPericial_CrearConExpertoCertificadoInvalido_DeberiaLanzarExcepcion(string expertoCertificado)
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Análisis de huellas dactilares";
        var peritoId = Guid.NewGuid();
        var especialidad = "Dactiloscopía";

        // Act & Assert
        var act = () => PruebaPericial.Crear(id, descripcion, peritoId, expertoCertificado, especialidad);
        act.Should().Throw<DomainException>()
           .WithMessage("*experto certificado*obligatorio*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void PruebaPericial_CrearConEspecialidadInvalida_DeberiaLanzarExcepcion(string especialidad)
    {
        // Arrange
        var id = Guid.NewGuid();
        var descripcion = "Análisis de huellas dactilares";
        var peritoId = Guid.NewGuid();
        var expertoCertificado = "Dr. Juan Pérez";

        // Act & Assert
        var act = () => PruebaPericial.Crear(id, descripcion, peritoId, expertoCertificado, especialidad);
        act.Should().Throw<DomainException>()
           .WithMessage("*especialidad*obligatoria*");
    }

    [Fact]
    public void PruebaPericial_AgregarInformeValido_DeberiaActualizarInforme()
    {
        // Arrange
        var prueba = PruebaPericial.Crear(
            Guid.NewGuid(),
            "Análisis de huellas dactilares",
            "Dr. Juan Pérez"
        );
        var informe = "Las huellas encontradas coinciden con el sospechoso";

        // Act
        prueba.AgregarInforme(informe);

        // Assert
        prueba.InformePericial.Should().Be(informe);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void PruebaPericial_AgregarInformeInvalido_DeberiaLanzarExcepcion(string informe)
    {
        // Arrange
        var prueba = PruebaPericial.Crear(
            Guid.NewGuid(),
            "Análisis de huellas dactilares",
            "Dr. Juan Pérez"
        );

        // Act & Assert
        var act = () => prueba.AgregarInforme(informe);
        act.Should().Throw<DomainException>()
           .WithMessage("*informe*vacío*");
    }

    [Fact]
    public void PruebaPericial_ValidarSinInforme_DeberiaMarcarComoInvalida()
    {
        // Arrange
        var prueba = PruebaPericial.Crear(
            Guid.NewGuid(),
            "Análisis de huellas dactilares",
            "Dr. Juan Pérez"
        );

        // Act
        prueba.Validar();

        // Assert
        prueba.EsValida.Should().BeFalse();
    }

    [Fact]
    public void PruebaPericial_ValidarConInforme_DeberiaMarcarComoValida()
    {
        // Arrange
        var prueba = PruebaPericial.Crear(
            Guid.NewGuid(),
            "Análisis de huellas dactilares",
            "Dr. Juan Pérez"
        );
        prueba.AgregarInforme("Informe detallado del análisis");

        // Act
        prueba.Validar();

        // Assert
        prueba.EsValida.Should().BeTrue();
    }
}
