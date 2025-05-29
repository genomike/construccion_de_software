using EtapaDeJuicio.Domain.Entities.Audiencias;

namespace EtapaDeJuicio.UI.Web.Models
{    public class AudienciaDto
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public string TipoAudiencia { get; set; } = string.Empty;
        public string SalaAudiencia { get; set; } = string.Empty;
        public string JuezId { get; set; } = string.Empty;
        public string EstadoAudiencia { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;

        // Constructor vacío para formularios
        public AudienciaDto() { }

        // Constructor desde entidad de dominio
        public AudienciaDto(Audiencia audiencia)
        {
            Id = audiencia.Id;
            Titulo = audiencia.Titulo;
            FechaHora = audiencia.FechaHoraProgramada;
            TipoAudiencia = audiencia.Tipo.ToString();
            SalaAudiencia = ""; // Esta propiedad no existe en la entidad, se debe manejar por separado
            JuezId = ""; // Esta propiedad no existe en la entidad, se debe manejar por separado
            EstadoAudiencia = audiencia.Estado.ToString();
            Observaciones = ""; // Esta propiedad no existe en la entidad, se debe manejar por separado
        }

        // Método para convertir a entidad de dominio
        public Audiencia ToEntity()
        {
            if (!Enum.TryParse<TipoAudiencia>(TipoAudiencia, out var tipo))
            {
                tipo = Domain.Entities.Audiencias.TipoAudiencia.JuicioOral;
            }

            return Audiencia.Crear(Id, Titulo, FechaHora, tipo);
        }
    }
}
