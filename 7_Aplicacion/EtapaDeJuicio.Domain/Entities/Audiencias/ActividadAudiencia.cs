namespace EtapaDeJuicio.Domain.Entities.Audiencias;

public class ActividadAudiencia
{
    public Guid Id { get; private set; }
    public Guid AudienciaId { get; private set; } // FK para Entity Framework
    public string Descripcion { get; private set; } = string.Empty;
    public TipoActividad Tipo { get; private set; }
    public DateTime FechaHora { get; private set; }
    public DateTime Timestamp => FechaHora; // Alias para compatibilidad
    public string? Observaciones { get; private set; }

    // Constructor sin parámetros para Entity Framework
    private ActividadAudiencia() { }

    public ActividadAudiencia(string descripcion, TipoActividad tipo, string? observaciones = null)
    {
        Id = Guid.NewGuid();
        Descripcion = descripcion ?? throw new ArgumentNullException(nameof(descripcion));
        Tipo = tipo;
        FechaHora = DateTime.Now;
        Observaciones = observaciones;
    }
    
    // Método interno para establecer AudienciaId (usado por EF)
    internal void SetAudienciaId(Guid audienciaId)
    {
        AudienciaId = audienciaId;
    }
}
