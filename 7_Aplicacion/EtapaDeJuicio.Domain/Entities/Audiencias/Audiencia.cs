using EtapaDeJuicio.Domain.Exceptions;
namespace EtapaDeJuicio.Domain.Entities.Audiencias;

public enum EstadoAudiencia
{
    Programada,
    EnCurso,
    Suspendida,
    Finalizada,
    Cancelada
}

public class Audiencia
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public DateTime FechaHoraProgramada { get; private set; }
    public DateTime FechaProgramada => FechaHoraProgramada; // Alias para compatibilidad
    public TipoAudiencia Tipo { get; private set; }
    public int DuracionMinutos { get; private set; }    public EstadoAudiencia Estado { get; internal set; }
    public DateTime? FechaHoraInicio { get; internal set; }
    public DateTime? FechaInicio => FechaHoraInicio; // Alias para compatibilidad
    public DateTime? FechaHoraFin { get; internal set; }
    public DateTime? FechaFin => FechaHoraFin; // Alias para compatibilidad
    public string? MotivoCancelacion { get; internal set; }
    public string? MotivoSuspension { get; internal set; }

    private readonly List<ParticipanteAudiencia> _participantes = new();
    private readonly List<ActividadAudiencia> _actividades = new();

    public IReadOnlyCollection<ParticipanteAudiencia> Participantes => _participantes.AsReadOnly();
    public IReadOnlyCollection<ActividadAudiencia> Actividades => _actividades.AsReadOnly();

    // Constructor sin parámetros para Entity Framework
    private Audiencia() { }

    private Audiencia(Guid id, string titulo, DateTime fechaHoraProgramada, TipoAudiencia tipo)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID de la audiencia no puede estar vacío");

        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("El título de la audiencia es obligatorio");
        if (fechaHoraProgramada <= DateTime.Now)
            throw new DomainException("La audiencia debe tener una fecha futura");

        Id = id;
        Titulo = titulo;
        FechaHoraProgramada = fechaHoraProgramada;
        Tipo = tipo;
        Estado = EstadoAudiencia.Programada;
    }

    public static Audiencia Crear(Guid id, string titulo, DateTime fechaHoraProgramada, TipoAudiencia tipo)
    {
        return new Audiencia(id, titulo, fechaHoraProgramada, tipo);
    }
    public void Iniciar()
    {
        if (Estado != EstadoAudiencia.Programada)
            throw new DomainException("Solo se pueden iniciar audiencias programadas");

        Estado = EstadoAudiencia.EnCurso;
        FechaHoraInicio = DateTime.Now;
    }

    public void Finalizar()
    {
        if (Estado != EstadoAudiencia.EnCurso)
            throw new DomainException("Solo se pueden finalizar audiencias en curso");

        Estado = EstadoAudiencia.Finalizada;
        FechaHoraFin = DateTime.Now;
    }

    public void Suspender(string motivo)
    {
        if (Estado != EstadoAudiencia.EnCurso)
            throw new DomainException("Solo se pueden suspender audiencias en curso");

        if (string.IsNullOrWhiteSpace(motivo))
            throw new DomainException("El motivo de suspensión es obligatorio");

        Estado = EstadoAudiencia.Suspendida;
        MotivoSuspension = motivo;
    }

    public void Reanudar()
    {
        if (Estado != EstadoAudiencia.Suspendida)
            throw new DomainException("Solo se pueden reanudar audiencias suspendidas");

        Estado = EstadoAudiencia.EnCurso;
        MotivoSuspension = null;
    }

    public void Cancelar(string motivo)
    {
        if (Estado == EstadoAudiencia.Finalizada)
            throw new DomainException("No se puede cancelar una audiencia finalizada");

        if (string.IsNullOrWhiteSpace(motivo))
            throw new DomainException("El motivo de cancelación es obligatorio");

        Estado = EstadoAudiencia.Cancelada;
        MotivoCancelacion = motivo;
    }    public void AgregarParticipante(Guid id, string nombre, RolParticipante rol)
    {
        if (_participantes.Any(p => p.Id == id))
            throw new DomainException("Ya existe un participante con el mismo ID");

        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre del participante es obligatorio");

        var participante = new ParticipanteAudiencia(id, nombre, rol);
        participante.SetAudienciaId(this.Id);
        _participantes.Add(participante);
    }    public Guid RegistrarActividad(string descripcion, TipoActividad tipo, string? observaciones = null)
    {
        if (Estado != EstadoAudiencia.EnCurso)
            throw new DomainException("Solo se pueden registrar actividades en audiencias en curso");

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DomainException("La descripción de la actividad es obligatoria");

        var actividad = new ActividadAudiencia(descripcion, tipo, observaciones);
        actividad.SetAudienciaId(this.Id);
        _actividades.Add(actividad);
        return actividad.Id;
    }

    public IEnumerable<ParticipanteAudiencia> ObtenerParticipantesPorRol(RolParticipante rol)
    {
        return _participantes.Where(p => p.Rol == rol);
    }

    public bool ValidarRequisitos()
    {
        // Para una audiencia válida necesita al menos un juez y participantes básicos
        var tieneJuez = _participantes.Any(p => p.Rol == RolParticipante.Juez);
        var tieneFiscal = _participantes.Any(p => p.Rol == RolParticipante.Fiscal);

        return tieneJuez && tieneFiscal;
    }

    public TimeSpan? CalcularDuracion()
    {
        if (FechaHoraInicio == null || FechaHoraFin == null)
            return null;

        return FechaHoraFin.Value - FechaHoraInicio.Value;
    }
    public string GenerarActa()
    {
        if (Estado != EstadoAudiencia.Finalizada)
            throw new DomainException("Solo se puede generar acta de audiencias finalizadas");

        var contenido = $"Acta de {Tipo} - {Titulo}\n" +
                       $"Fecha: {FechaHoraProgramada:dd/MM/yyyy}\n" +
                       $"Inicio: {FechaHoraInicio:HH:mm}\n" +
                       $"Fin: {FechaHoraFin:HH:mm}\n" +
                       $"Participantes: {string.Join(", ", _participantes.Select(p => p.Nombre))}\n" +
                       $"Actividades: {string.Join(", ", _actividades.Select(a => a.Descripcion))}";

        return contenido;
    }    public bool PuedeModificar()
    {
        return Estado == EstadoAudiencia.Programada;
    }
}
