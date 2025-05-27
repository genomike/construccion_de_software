namespace EtapaDeJuicio.Domain.Entities.Audiencias;

public class ParticipanteAudiencia
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; }
    public RolParticipante Rol { get; private set; }
    public DateTime FechaRegistro { get; private set; }

    public ParticipanteAudiencia(Guid id, string nombre, RolParticipante rol)
    {
        Id = id;
        Nombre = nombre;
        Rol = rol;
        FechaRegistro = DateTime.Now;
    }
}
