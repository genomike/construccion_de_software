namespace EtapaDeJuicio.Domain.Entities.Audiencias;

public class ActaAudiencia
{
    public Guid Id { get; private set; }
    public Guid AudienciaId { get; private set; }
    public string Contenido { get; private set; }
    public DateTime FechaGeneracion { get; private set; }
    public IReadOnlyCollection<ParticipanteAudiencia> Participantes { get; private set; }
    public IReadOnlyCollection<ActividadAudiencia> Actividades { get; private set; }

    public ActaAudiencia(Guid audienciaId, string contenido, 
        IEnumerable<ParticipanteAudiencia> participantes, 
        IEnumerable<ActividadAudiencia> actividades)
    {
        Id = Guid.NewGuid();
        AudienciaId = audienciaId;
        Contenido = contenido;
        FechaGeneracion = DateTime.Now;
        Participantes = participantes.ToList().AsReadOnly();
        Actividades = actividades.ToList().AsReadOnly();
    }
}
