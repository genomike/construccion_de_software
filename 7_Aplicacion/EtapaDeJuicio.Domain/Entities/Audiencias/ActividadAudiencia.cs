namespace EtapaDeJuicio.Domain.Entities.Audiencias;

public class ActividadAudiencia
{
    public Guid Id { get; private set; }
    public string Descripcion { get; private set; }
    public TipoActividad Tipo { get; private set; }
    public DateTime FechaHora { get; private set; }
    public DateTime Timestamp => FechaHora; // Alias para compatibilidad
    public string? Observaciones { get; private set; }

    public ActividadAudiencia(string descripcion, TipoActividad tipo, string? observaciones = null)
    {
        Id = Guid.NewGuid();
        Descripcion = descripcion;
        Tipo = tipo;
        FechaHora = DateTime.Now;
        Observaciones = observaciones;
    }
}
