using System.ComponentModel.DataAnnotations;

namespace EtapaDeJuicio.UI.Web.Models
{
    public class SentenciaDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El ID del juez es requerido")]
        public string JuezId { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de sentencia es requerido")]
        public string TipoSentencia { get; set; } = string.Empty;

        public string? AudienciaId { get; set; }

        public string EstadoSentencia { get; set; } = "Borrador";

        [Required(ErrorMessage = "La fecha de emisión es requerida")]
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        public DateTime? FechaNotificacion { get; set; }

        [StringLength(500, ErrorMessage = "El resumen no puede exceder 500 caracteres")]
        public string? ResumenDecision { get; set; }

        [StringLength(1000, ErrorMessage = "Los fundamentos legales no pueden exceder 1000 caracteres")]
        public string? FundamentosLegales { get; set; }

        [StringLength(2000, ErrorMessage = "Los resolutivos no pueden exceder 2000 caracteres")]
        public string? Resolutivos { get; set; }
    }
}
