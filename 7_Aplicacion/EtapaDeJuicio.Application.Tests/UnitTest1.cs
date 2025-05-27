using EtapaDeJuicio.Application.Commands.Audiencias;
using EtapaDeJuicio.Application.Handlers.CommandHandlers;
using EtapaDeJuicio.Application.Interfaces;
using EtapaDeJuicio.Domain.Entities.Audiencias;
using FluentAssertions;
using NSubstitute;

namespace EtapaDeJuicio.Application.Tests;

public class CrearAudienciaCommandHandlerTests
{
    private readonly IAudienciaRepository _audienciaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CrearAudienciaCommandHandler _handler;

    public CrearAudienciaCommandHandlerTests()
    {
        _audienciaRepository = Substitute.For<IAudienciaRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _handler = new CrearAudienciaCommandHandler(_audienciaRepository, _unitOfWork);
    }    [Fact]
    public async Task Handle_ConDatosValidos_DeberiaCrearAudiencia()
    {
        // Arrange
        var command = new CrearAudienciaCommand(
            "Audiencia de Juicio Oral",
            DateTime.Now.AddDays(7),
            (int)TipoAudiencia.JuicioOral,
            120
        );

        var audienciaId = Guid.NewGuid();
        _audienciaRepository.CrearAsync(Arg.Any<Audiencia>(), Arg.Any<CancellationToken>())
            .Returns(audienciaId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AudienciaId.Should().Be(audienciaId);
        result.Titulo.Should().Be(command.Titulo);
        result.Estado.Should().Be("Programada");

        await _audienciaRepository.Received(1).CrearAsync(Arg.Any<Audiencia>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}