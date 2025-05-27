namespace EtapaDeJuicio.Domain.Entities.Audiencias;

public class ParticipanteAudiencia
{
    public Guid Id { get; private set; }
    public Guid AudienciaId { get; private set; } // FK para Entity Framework
    public string Nombre { get; private set; } = string.Empty;
    public RolParticipante Rol { get; private set; }
    public DateTime FechaRegistro { get; private set; }

    // Constructor sin parámetros para Entity Framework
    private ParticipanteAudiencia() { }

    public ParticipanteAudiencia(Guid id, string nombre, RolParticipante rol)
    {
        Id = id;
        Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        Rol = rol;
        FechaRegistro = DateTime.Now;
    }
    
    // Método interno para establecer AudienciaId (usado por EF)
    internal void SetAudienciaId(Guid audienciaId)
    {
        AudienciaId = audienciaId;
    }
}
