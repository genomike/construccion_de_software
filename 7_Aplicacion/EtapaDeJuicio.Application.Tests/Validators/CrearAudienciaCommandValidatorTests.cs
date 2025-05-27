using EtapaDeJuicio.Application.Commands.Audiencias;
using EtapaDeJuicio.Application.Validators;
using EtapaDeJuicio.Domain.Entities.Audiencias;
using FluentAssertions;
using FluentValidation;

namespace EtapaDeJuicio.Application.Tests;

public class CrearAudienciaCommandValidatorTests
{
    private readonly CrearAudienciaCommandValidator _validator;

    public CrearAudienciaCommandValidatorTests()
    {
        _validator = new CrearAudienciaCommandValidator();
    }

    [Fact]
    public void Validate_ConDatosValidos_DeberiaSerValido()
    {
        // Arrange
        var command = new CrearAudienciaCommand(
            "Audiencia de Juicio Oral",
            DateTime.Now.AddDays(7),
            (int)TipoAudiencia.JuicioOral,
            120
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ConTituloInvalido_DeberiaFallar(string titulo)
    {
        // Arrange
        var command = new CrearAudienciaCommand(
            titulo,
            DateTime.Now.AddDays(7),
            (int)TipoAudiencia.JuicioOral,
            120
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Titulo));
    }

    [Fact]
    public void Validate_ConFechaPasada_DeberiaFallar()
    {
        // Arrange
        var command = new CrearAudienciaCommand(
            "Audiencia válida",
            DateTime.Now.AddDays(-1),
            (int)TipoAudiencia.JuicioOral,
            120
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.FechaProgramada));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-30)]
    [InlineData(500)]
    public void Validate_ConDuracionInvalida_DeberiaFallar(int duracion)
    {
        // Arrange
        var command = new CrearAudienciaCommand(
            "Audiencia válida",
            DateTime.Now.AddDays(7),
            (int)TipoAudiencia.JuicioOral,
            duracion
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.DuracionMinutos));
    }
}
