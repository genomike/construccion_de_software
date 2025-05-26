using FluentAssertions;
using EtapaDeJuicio.Domain.Entities.Audiencias;
using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Tests.Entities.Audiencias;

public class AudienciaTests
{
    [Fact]
    public void Crear_ConDatosValidos_DeberiaCrearCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var titulo = "Audiencia de juicio oral";
        var fechaProgramada = DateTime.Now.AddDays(7);
        var tipo = TipoAudiencia.JuicioOral;

        // Act
        var audiencia = Audiencia.Crear(id, titulo, fechaProgramada, tipo);

        // Assert
        audiencia.Should().NotBeNull();
        audiencia.Id.Should().Be(id);
        audiencia.Titulo.Should().Be(titulo);
        audiencia.FechaProgramada.Should().Be(fechaProgramada);
        audiencia.Tipo.Should().Be(tipo);
        audiencia.Estado.Should().Be(EstadoAudiencia.Programada);
        audiencia.Participantes.Should().BeEmpty();
        audiencia.Actividades.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Crear_ConTituloVacio_DeberiaLanzarExcepcion(string titulo)
    {
        // Arrange
        var id = Guid.NewGuid();
        var fechaProgramada = DateTime.Now.AddDays(7);
        var tipo = TipoAudiencia.JuicioOral;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            Audiencia.Crear(id, titulo, fechaProgramada, tipo));
        
        exception.Message.Should().Contain("título");
    }

    [Fact]
    public void Crear_ConFechaPasada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        var titulo = "Audiencia";
        var fechaPasada = DateTime.Now.AddDays(-1);
        var tipo = TipoAudiencia.JuicioOral;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            Audiencia.Crear(id, titulo, fechaPasada, tipo));
        
        exception.Message.Should().Contain("fecha futura");
    }

    [Fact]
    public void Iniciar_DesdeEstadoProgramada_DeberiaIniciarCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);

        // Act
        audiencia.Iniciar();

        // Assert
        audiencia.Estado.Should().Be(EstadoAudiencia.EnCurso);
        audiencia.FechaInicio.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Iniciar_DesdeEstadoIncorrecto_DeberiaLanzarExcepcion()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        audiencia.Cancelar("Motivo de cancelación");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            audiencia.Iniciar());
        
        exception.Message.Should().Contain("estado actual");
    }

    [Fact]
    public void Finalizar_DesdeEstadoEnCurso_DeberiaFinalizarCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        audiencia.Iniciar();

        // Act
        audiencia.Finalizar();

        // Assert
        audiencia.Estado.Should().Be(EstadoAudiencia.Finalizada);
        audiencia.FechaFin.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Cancelar_ConMotivoValido_DeberiaCancelarCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        var motivo = "Falta de disponibilidad del juez";

        // Act
        audiencia.Cancelar(motivo);

        // Assert
        audiencia.Estado.Should().Be(EstadoAudiencia.Cancelada);
        audiencia.MotivoCancelacion.Should().Be(motivo);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Cancelar_ConMotivoVacio_DeberiaLanzarExcepcion(string motivo)
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            audiencia.Cancelar(motivo));
        
        exception.Message.Should().Contain("motivo");
    }

    [Fact]
    public void Suspender_DesdeEstadoEnCurso_DeberiaSuspenderCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        audiencia.Iniciar();
        var motivo = "Problema técnico";

        // Act
        audiencia.Suspender(motivo);

        // Assert
        audiencia.Estado.Should().Be(EstadoAudiencia.Suspendida);
        audiencia.MotivoSuspension.Should().Be(motivo);
    }

    [Fact]
    public void Reanudar_DesdeSuspendida_DeberiaReanudarCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        audiencia.Iniciar();
        audiencia.Suspender("Problema técnico");

        // Act
        audiencia.Reanudar();

        // Assert
        audiencia.Estado.Should().Be(EstadoAudiencia.EnCurso);
    }

    [Fact]
    public void AgregarParticipante_ConDatosValidos_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        var idParticipante = Guid.NewGuid();
        var nombre = "Juan Pérez";
        var rol = RolParticipante.Testigo;

        // Act
        audiencia.AgregarParticipante(idParticipante, nombre, rol);

        // Assert
        audiencia.Participantes.Should().HaveCount(1);
        var participante = audiencia.Participantes.First();
        participante.Id.Should().Be(idParticipante);
        participante.Nombre.Should().Be(nombre);
        participante.Rol.Should().Be(rol);
    }

    [Fact]
    public void AgregarParticipante_DuplicadoMismoRol_DeberiaLanzarExcepcion()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        var idParticipante = Guid.NewGuid();
        audiencia.AgregarParticipante(idParticipante, "Juan", RolParticipante.Juez);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            audiencia.AgregarParticipante(idParticipante, "Juan", RolParticipante.Juez));
        
        exception.Message.Should().Contain("ya está registrado");
    }

    [Fact]
    public void RegistrarActividad_ConDatosValidos_DeberiaRegistrarCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        audiencia.Iniciar();
        var descripcion = "Presentación de pruebas documentales";
        var tipo = TipoActividad.PresentacionPruebas;

        // Act
        audiencia.RegistrarActividad(descripcion, tipo);

        // Assert
        audiencia.Actividades.Should().HaveCount(1);
        var actividad = audiencia.Actividades.First();
        actividad.Descripcion.Should().Be(descripcion);
        actividad.Tipo.Should().Be(tipo);
        actividad.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void RegistrarActividad_AudienciaNoIniciada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            audiencia.RegistrarActividad("Actividad", TipoActividad.PresentacionPruebas));
        
        exception.Message.Should().Contain("en curso");
    }

    [Fact]
    public void CalcularDuracion_ConFechaInicioYFin_DeberiaCalcularCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        audiencia.Iniciar();
        
        // Simular tiempo transcurrido
        Thread.Sleep(100);
        audiencia.Finalizar();

        // Act
        var duracion = audiencia.CalcularDuracion();

        // Assert
        duracion.Should().NotBeNull();
        duracion.Value.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public void CalcularDuracion_SinFechaFin_DeberiaRetornarNull()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        audiencia.Iniciar();

        // Act
        var duracion = audiencia.CalcularDuracion();

        // Assert
        duracion.Should().BeNull();
    }

    [Fact]
    public void ValidarRequisitos_ConTodosRequisitos_DeberiaRetornarTrue()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        
        audiencia.AgregarParticipante(Guid.NewGuid(), "Juez", RolParticipante.Juez);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Fiscal", RolParticipante.Fiscal);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Defensor", RolParticipante.Defensor);

        // Act
        var tieneRequisitos = audiencia.ValidarRequisitos();

        // Assert
        tieneRequisitos.Should().BeTrue();
    }

    [Fact]
    public void ValidarRequisitos_SinJuez_DeberiaRetornarFalse()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        
        audiencia.AgregarParticipante(Guid.NewGuid(), "Fiscal", RolParticipante.Fiscal);

        // Act
        var tieneRequisitos = audiencia.ValidarRequisitos();

        // Assert
        tieneRequisitos.Should().BeFalse();
    }

    [Fact]
    public void ObtenerParticipantesPorRol_DeberiaFiltrarCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        
        audiencia.AgregarParticipante(Guid.NewGuid(), "Testigo 1", RolParticipante.Testigo);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Testigo 2", RolParticipante.Testigo);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Juez", RolParticipante.Juez);

        // Act
        var testigos = audiencia.ObtenerParticipantesPorRol(RolParticipante.Testigo);

        // Assert
        testigos.Should().HaveCount(2);
        testigos.All(p => p.Rol == RolParticipante.Testigo).Should().BeTrue();
    }

    [Fact]
    public void GenerarActa_AudienciaFinalizada_DeberiaGenerarCorrectamente()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Audiencia importante", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);
        
        audiencia.AgregarParticipante(Guid.NewGuid(), "Juez", RolParticipante.Juez);
        audiencia.Iniciar();
        audiencia.RegistrarActividad("Apertura", TipoActividad.Apertura);
        audiencia.Finalizar();

        // Act
        var acta = audiencia.GenerarActa();

        // Assert
        acta.Should().NotBeNull();
        acta.Should().Contain("Audiencia importante");
        acta.Should().Contain("Juez");
        acta.Should().Contain("Apertura");
    }

    [Fact]
    public void GenerarActa_AudienciaNoFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            audiencia.GenerarActa());
        
        exception.Message.Should().Contain("finalizada");
    }

    [Theory]
    [InlineData(EstadoAudiencia.Programada, true)]
    [InlineData(EstadoAudiencia.EnCurso, false)]
    [InlineData(EstadoAudiencia.Finalizada, false)]
    [InlineData(EstadoAudiencia.Cancelada, false)]
    public void PuedeModificar_SegunEstado_DeberiaRetornarResultadoCorrespondiente(
        EstadoAudiencia estado, bool puedeModificar)
    {
        // Arrange
        var audiencia = Audiencia.Crear(
            Guid.NewGuid(), 
            "Test", 
            DateTime.Now.AddDays(7), 
            TipoAudiencia.JuicioOral);

        // Simular el estado requerido
        switch (estado)
        {
            case EstadoAudiencia.EnCurso:
                audiencia.Iniciar();
                break;
            case EstadoAudiencia.Finalizada:
                audiencia.Iniciar();
                audiencia.Finalizar();
                break;
            case EstadoAudiencia.Cancelada:
                audiencia.Cancelar("Motivo");
                break;
        }

        // Act
        var resultado = audiencia.PuedeModificar();

        // Assert
        resultado.Should().Be(puedeModificar);
    }
}
