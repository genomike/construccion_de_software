namespace EtapaDeJuicio.Domain.Entities.Audiencias;

public class ActaAudiencia
{
    private readonly List<ParticipanteAudiencia> _participantes = new();
    private readonly List<ActividadAudiencia> _actividades = new();

    public Guid Id { get; private set; }
    public Guid AudienciaId { get; private set; }
    public string Contenido { get; private set; } = string.Empty;
    public DateTime FechaGeneracion { get; private set; }
    public IReadOnlyCollection<ParticipanteAudiencia> Participantes => _participantes.AsReadOnly();
    public IReadOnlyCollection<ActividadAudiencia> Actividades => _actividades.AsReadOnly();

    // Constructor sin parámetros para Entity Framework
    private ActaAudiencia() { }

    public ActaAudiencia(Guid audienciaId, string contenido, 
        IEnumerable<ParticipanteAudiencia> participantes, 
        IEnumerable<ActividadAudiencia> actividades)
    {
        Id = Guid.NewGuid();
        AudienciaId = audienciaId;
        Contenido = contenido ?? throw new ArgumentNullException(nameof(contenido));
        FechaGeneracion = DateTime.Now;
        _participantes.AddRange(participantes ?? throw new ArgumentNullException(nameof(participantes)));
        _actividades.AddRange(actividades ?? throw new ArgumentNullException(nameof(actividades)));
    }
}
