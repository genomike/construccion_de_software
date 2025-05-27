using EtapaDeJuicio.Domain.Entities.Audiencias;
using EtapaDeJuicio.Domain.Exceptions;
using FluentAssertions;

namespace EtapaDeJuicioTests;

public class GestionDeAudienciasTests
{
    #region Pruebas de Creación y Configuración de Audiencias

    [Fact]
    public void Audiencia_CrearConDatosValidos_DeberiaCrearCorrectamente()
    {
        // Arrange
        var id = Guid.NewGuid();
        var titulo = "Audiencia de juicio oral principal";
        var fechaProgramada = DateTime.Now.AddDays(10);
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

    [Fact]
    public void Audiencia_CrearConIdVacio_DeberiaLanzarExcepcion()
    {
        // Arrange
        var titulo = "Audiencia test";
        var fechaProgramada = DateTime.Now.AddDays(5);
        var tipo = TipoAudiencia.JuicioOral;

        // Act & Assert
        var act = () => Audiencia.Crear(Guid.Empty, titulo, fechaProgramada, tipo);
        act.Should().Throw<DomainException>()
           .WithMessage("*ID*audiencia*vacío*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Audiencia_CrearConTituloInvalido_DeberiaLanzarExcepcion(string titulo)
    {
        // Arrange
        var id = Guid.NewGuid();
        var fechaProgramada = DateTime.Now.AddDays(5);
        var tipo = TipoAudiencia.JuicioOral;

        // Act & Assert
        var act = () => Audiencia.Crear(id, titulo, fechaProgramada, tipo);
        act.Should().Throw<DomainException>()
           .WithMessage("*título*audiencia*obligatorio*");
    }

    [Fact]
    public void Audiencia_CrearConFechaPasada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var id = Guid.NewGuid();
        var titulo = "Audiencia test";
        var fechaPasada = DateTime.Now.AddDays(-1);
        var tipo = TipoAudiencia.JuicioOral;

        // Act & Assert
        var act = () => Audiencia.Crear(id, titulo, fechaPasada, tipo);
        act.Should().Throw<DomainException>()
           .WithMessage("La audiencia debe tener una fecha futura");
    }

    [Theory]
    [InlineData(TipoAudiencia.JuicioOral)]
    [InlineData(TipoAudiencia.AudienciaPreliminar)]
    [InlineData(TipoAudiencia.AudienciaPreparatoria)]
    [InlineData(TipoAudiencia.AudienciaDeApelacion)]
    [InlineData(TipoAudiencia.AudienciaDeRevision)]
    public void Audiencia_CrearConDiferentesTipos_DeberiaCrearCorrectamente(TipoAudiencia tipo)
    {
        // Arrange
        var id = Guid.NewGuid();
        var titulo = $"Audiencia de {tipo}";
        var fechaProgramada = DateTime.Now.AddDays(7);

        // Act
        var audiencia = Audiencia.Crear(id, titulo, fechaProgramada, tipo);

        // Assert
        audiencia.Should().NotBeNull();
        audiencia.Tipo.Should().Be(tipo);
    }

    #endregion

    #region Pruebas de Gestión de Estados

    [Fact]
    public void Audiencia_IniciarDesdeProgramada_DeberiaIniciarCorrectamente()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();

        // Act
        audiencia.Iniciar();

        // Assert
        audiencia.Estado.Should().Be(EstadoAudiencia.EnCurso);
        audiencia.FechaInicio.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Audiencia_IniciarDesdeEstadoIncorrecto_DeberiaLanzarExcepcion()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Cancelar("Motivo de prueba");

        // Act & Assert
        var act = () => audiencia.Iniciar();
        act.Should().Throw<DomainException>()
           .WithMessage("*Solo se pueden iniciar audiencias programadas*");
    }

    [Fact]
    public void Audiencia_FinalizarDesdeEnCurso_DeberiaFinalizarCorrectamente()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Iniciar();

        // Act
        audiencia.Finalizar();

        // Assert
        audiencia.Estado.Should().Be(EstadoAudiencia.Finalizada);
        audiencia.FechaFin.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Audiencia_FinalizarDesdeEstadoIncorrecto_DeberiaLanzarExcepcion()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();

        // Act & Assert
        var act = () => audiencia.Finalizar();
        act.Should().Throw<DomainException>()
           .WithMessage("*Solo se pueden finalizar audiencias en curso*");
    }

    [Fact]
    public void Audiencia_SuspenderDesdeEnCurso_DeberiaSuspenderCorrectamente()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Iniciar();
        var motivo = "Problema técnico con el sistema";

        // Act
        audiencia.Suspender(motivo);

        // Assert
        audiencia.Estado.Should().Be(EstadoAudiencia.Suspendida);
        audiencia.MotivoSuspension.Should().Be(motivo);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Audiencia_SuspenderSinMotivo_DeberiaLanzarExcepcion(string motivo)
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Iniciar();

        // Act & Assert
        var act = () => audiencia.Suspender(motivo);
        act.Should().Throw<DomainException>()
           .WithMessage("*motivo*suspensión*obligatorio*");
    }

    [Fact]
    public void Audiencia_ReanudarDesdeSuspendida_DeberiaReanudarCorrectamente()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Iniciar();
        audiencia.Suspender("Motivo temporal");

        // Act
        audiencia.Reanudar();

        // Assert
        audiencia.Estado.Should().Be(EstadoAudiencia.EnCurso);
        audiencia.MotivoSuspension.Should().BeNull();
    }

    [Fact]
    public void Audiencia_ReanudarDesdeEstadoIncorrecto_DeberiaLanzarExcepcion()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();

        // Act & Assert
        var act = () => audiencia.Reanudar();
        act.Should().Throw<DomainException>()
           .WithMessage("*Solo se pueden reanudar audiencias suspendidas*");
    }

    [Fact]
    public void Audiencia_CancelarConMotivoValido_DeberiaCancelarCorrectamente()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        var motivo = "Falta de disponibilidad del juez principal";

        // Act
        audiencia.Cancelar(motivo);

        // Assert
        audiencia.Estado.Should().Be(EstadoAudiencia.Cancelada);
        audiencia.MotivoCancelacion.Should().Be(motivo);
    }

    [Fact]
    public void Audiencia_CancelarAudienciaFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Iniciar();
        audiencia.Finalizar();

        // Act & Assert
        var act = () => audiencia.Cancelar("Motivo");
        act.Should().Throw<DomainException>()
           .WithMessage("*No se puede cancelar una audiencia finalizada*");
    }

    #endregion

    #region Pruebas de Gestión de Participantes

    [Fact]
    public void Audiencia_AgregarParticipanteValido_DeberiaAgregarCorrectamente()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        var participanteId = Guid.NewGuid();
        var nombre = "Dr. Carlos Méndez";
        var rol = RolParticipante.Juez;

        // Act
        audiencia.AgregarParticipante(participanteId, nombre, rol);

        // Assert
        audiencia.Participantes.Should().HaveCount(1);
        var participante = audiencia.Participantes.First();
        participante.Id.Should().Be(participanteId);
        participante.Nombre.Should().Be(nombre);
        participante.Rol.Should().Be(rol);
        participante.FechaRegistro.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Audiencia_AgregarParticipanteDuplicado_DeberiaLanzarExcepcion()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        var participanteId = Guid.NewGuid();
        audiencia.AgregarParticipante(participanteId, "Juez Principal", RolParticipante.Juez);

        // Act & Assert
        var act = () => audiencia.AgregarParticipante(participanteId, "Otro Nombre", RolParticipante.Fiscal);
        act.Should().Throw<DomainException>()
           .WithMessage("*Ya existe un participante con el mismo ID*");
    }

    [Theory]
    [InlineData(RolParticipante.Juez)]
    [InlineData(RolParticipante.Fiscal)]
    [InlineData(RolParticipante.Defensor)]
    [InlineData(RolParticipante.Testigo)]
    [InlineData(RolParticipante.Perito)]
    [InlineData(RolParticipante.Imputado)]
    [InlineData(RolParticipante.Victima)]
    [InlineData(RolParticipante.Secretario)]
    public void Audiencia_AgregarParticipantesPorRol_DeberiaAgregarCorrectamente(RolParticipante rol)
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        var participanteId = Guid.NewGuid();
        var nombre = $"Participante {rol}";

        // Act
        audiencia.AgregarParticipante(participanteId, nombre, rol);

        // Assert
        audiencia.Participantes.Should().HaveCount(1);
        audiencia.Participantes.First().Rol.Should().Be(rol);
    }

    [Fact]
    public void Audiencia_ObtenerParticipantesPorRol_DeberiaFiltrarCorrectamente()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.AgregarParticipante(Guid.NewGuid(), "Testigo 1", RolParticipante.Testigo);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Testigo 2", RolParticipante.Testigo);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Juez Principal", RolParticipante.Juez);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Fiscal", RolParticipante.Fiscal);

        // Act
        var testigos = audiencia.ObtenerParticipantesPorRol(RolParticipante.Testigo);
        var jueces = audiencia.ObtenerParticipantesPorRol(RolParticipante.Juez);

        // Assert
        testigos.Should().HaveCount(2);
        testigos.All(p => p.Rol == RolParticipante.Testigo).Should().BeTrue();
        jueces.Should().HaveCount(1);
        jueces.First().Rol.Should().Be(RolParticipante.Juez);
    }

    #endregion

    #region Pruebas de Gestión de Actividades

    [Fact]
    public void Audiencia_RegistrarActividadValida_DeberiaRegistrarCorrectamente()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Iniciar();
        var descripcion = "Apertura de la audiencia de juicio oral";
        var tipo = TipoActividad.Apertura;

        // Act
        audiencia.RegistrarActividad(descripcion, tipo);

        // Assert
        audiencia.Actividades.Should().HaveCount(1);
        var actividad = audiencia.Actividades.First();
        actividad.Descripcion.Should().Be(descripcion);
        actividad.Tipo.Should().Be(tipo);
        actividad.Timestamp.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Audiencia_RegistrarActividadEnAudienciaNoIniciada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();

        // Act & Assert
        var act = () => audiencia.RegistrarActividad("Actividad", TipoActividad.PresentacionPruebas);
        act.Should().Throw<DomainException>()
           .WithMessage("*Solo se pueden registrar actividades en audiencias en curso*");
    }

    [Theory]
    [InlineData(TipoActividad.Apertura)]
    [InlineData(TipoActividad.PresentacionPruebas)]
    [InlineData(TipoActividad.TestimonialDeclaraciones)]
    [InlineData(TipoActividad.AlegacionFinal)]
    [InlineData(TipoActividad.Deliberacion)]
    [InlineData(TipoActividad.LecturaSentencia)]
    [InlineData(TipoActividad.Cierre)]
    public void Audiencia_RegistrarDiferentesTiposActividad_DeberiaRegistrarCorrectamente(TipoActividad tipo)
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Iniciar();
        var descripcion = $"Actividad de {tipo}";

        // Act
        audiencia.RegistrarActividad(descripcion, tipo);

        // Assert
        audiencia.Actividades.Should().HaveCount(1);
        audiencia.Actividades.First().Tipo.Should().Be(tipo);
    }

    [Fact]
    public void Audiencia_RegistrarMultiplesActividades_DeberiaRegistrarEnOrden()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Iniciar();

        // Act
        audiencia.RegistrarActividad("Apertura", TipoActividad.Apertura);
        Thread.Sleep(10); // Asegurar diferencia en timestamp
        audiencia.RegistrarActividad("Presentación de pruebas", TipoActividad.PresentacionPruebas);
        Thread.Sleep(10);
        audiencia.RegistrarActividad("Testimonios", TipoActividad.TestimonialDeclaraciones);

        // Assert
        audiencia.Actividades.Should().HaveCount(3);
        var actividades = audiencia.Actividades.OrderBy(a => a.Timestamp).ToList();
        actividades[0].Tipo.Should().Be(TipoActividad.Apertura);
        actividades[1].Tipo.Should().Be(TipoActividad.PresentacionPruebas);
        actividades[2].Tipo.Should().Be(TipoActividad.TestimonialDeclaraciones);
    }

    #endregion

    #region Pruebas de Validación y Utilidades

    [Fact]
    public void Audiencia_ValidarRequisitosConParticipantesNecesarios_DeberiaRetornarTrue()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.AgregarParticipante(Guid.NewGuid(), "Juez Principal", RolParticipante.Juez);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Fiscal", RolParticipante.Fiscal);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Defensor", RolParticipante.Defensor);

        // Act
        var esValida = audiencia.ValidarRequisitos();

        // Assert
        esValida.Should().BeTrue();
    }

    [Fact]
    public void Audiencia_ValidarRequisitosSinJuez_DeberiaRetornarFalse()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.AgregarParticipante(Guid.NewGuid(), "Fiscal", RolParticipante.Fiscal);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Defensor", RolParticipante.Defensor);

        // Act
        var esValida = audiencia.ValidarRequisitos();

        // Assert
        esValida.Should().BeFalse();
    }

    [Fact]
    public void Audiencia_ValidarRequisitosSinFiscal_DeberiaRetornarFalse()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.AgregarParticipante(Guid.NewGuid(), "Juez Principal", RolParticipante.Juez);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Defensor", RolParticipante.Defensor);

        // Act
        var esValida = audiencia.ValidarRequisitos();

        // Assert
        esValida.Should().BeFalse();
    }

    [Fact]
    public void Audiencia_CalcularDuracionConInicioYFin_DeberiaCalcularCorrectamente()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Iniciar();
        Thread.Sleep(100); // Simular tiempo transcurrido
        audiencia.Finalizar();

        // Act
        var duracion = audiencia.CalcularDuracion();

        // Assert
        duracion.Should().NotBeNull();
        duracion.Value.Should().BeGreaterThan(TimeSpan.Zero);
        duracion.Value.Should().BeLessThan(TimeSpan.FromMinutes(1)); // Verificar que es un tiempo razonable
    }

    [Fact]
    public void Audiencia_CalcularDuracionSinFinalizar_DeberiaRetornarNull()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Iniciar();

        // Act
        var duracion = audiencia.CalcularDuracion();

        // Assert
        duracion.Should().BeNull();
    }

    [Theory]
    [InlineData(EstadoAudiencia.Programada, true)]
    [InlineData(EstadoAudiencia.EnCurso, false)]
    [InlineData(EstadoAudiencia.Finalizada, false)]
    [InlineData(EstadoAudiencia.Cancelada, false)]
    [InlineData(EstadoAudiencia.Suspendida, false)]
    public void Audiencia_PuedeModificarSegunEstado_DeberiaRetornarResultadoCorresto(EstadoAudiencia estado, bool puedeModificar)
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        
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
                audiencia.Cancelar("Motivo de prueba");
                break;
            case EstadoAudiencia.Suspendida:
                audiencia.Iniciar();
                audiencia.Suspender("Motivo de prueba");
                break;
        }

        // Act
        var resultado = audiencia.PuedeModificar();

        // Assert
        resultado.Should().Be(puedeModificar);
    }

    #endregion

    #region Pruebas de Generación de Actas

    [Fact]
    public void Audiencia_GenerarActaDeAudienciaFinalizada_DeberiaGenerarCorrectamente()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.AgregarParticipante(Guid.NewGuid(), "Dr. Juan Pérez", RolParticipante.Juez);
        audiencia.AgregarParticipante(Guid.NewGuid(), "Fiscal García", RolParticipante.Fiscal);
        
        audiencia.Iniciar();
        audiencia.RegistrarActividad("Apertura de audiencia", TipoActividad.Apertura);
        audiencia.RegistrarActividad("Presentación de pruebas", TipoActividad.PresentacionPruebas);
        audiencia.Finalizar();

        // Act
        var acta = audiencia.GenerarActa();

        // Assert
        acta.Should().NotBeNull();
        acta.Should().Contain(audiencia.Titulo);
        acta.Should().Contain("Dr. Juan Pérez");
        acta.Should().Contain("Fiscal García");
        acta.Should().Contain("Apertura de audiencia");
        acta.Should().Contain("Presentación de pruebas");
    }

    [Fact]
    public void Audiencia_GenerarActaDeAudienciaNoFinalizada_DeberiaLanzarExcepcion()
    {
        // Arrange
        var audiencia = CrearAudienciaValida();
        audiencia.Iniciar();

        // Act & Assert
        var act = () => audiencia.GenerarActa();
        act.Should().Throw<DomainException>()
           .WithMessage("*Solo se puede generar acta de audiencias finalizadas*");
    }

    #endregion

    #region Métodos de Ayuda

    private static Audiencia CrearAudienciaValida()
    {
        return Audiencia.Crear(
            Guid.NewGuid(),
            "Audiencia de Prueba",
            DateTime.Now.AddDays(7),
            TipoAudiencia.JuicioOral
        );
    }

    #endregion
}