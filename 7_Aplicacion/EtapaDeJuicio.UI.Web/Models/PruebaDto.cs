using EtapaDeJuicio.Domain.Entities.Pruebas;

namespace EtapaDeJuicio.UI.Web.Models
{
    public class PruebaDto
    {
        public Guid Id { get; set; }
        public string Descripcion { get; set; } = "";
        public DateTime FechaPresentacion { get; set; }
        public string TipoPrueba { get; set; } = "";
        public string Estado { get; set; } = "";
        public Guid AudienciaId { get; set; }
        public string? Observaciones { get; set; }
        public bool EsValida { get; set; }

        // Propiedades específicas para diferentes tipos de prueba
        public string? RutaArchivo { get; set; } // Para PruebaDocumental
        public string? TipoDocumento { get; set; } // Para PruebaDocumental
        public string? NombreTestigo { get; set; } // Para PruebaTestimonial
        public string? TipoPericia { get; set; } // Para PruebaPericial
        public string? NombrePerito { get; set; } // Para PruebaPericial

        public static PruebaDto FromPruebaDocumental(PruebaDocumental prueba)
        {
            return new PruebaDto
            {
                Id = prueba.Id,
                Descripcion = prueba.Descripcion,
                FechaPresentacion = prueba.FechaPresentacion,
                TipoPrueba = "Documental",
                Estado = prueba.EsValida ? "Válida" : "Inválida",
                EsValida = prueba.EsValida,
                RutaArchivo = prueba.RutaArchivo,
                TipoDocumento = prueba.Formato.ToString()
            };
        }        public static PruebaDto FromPruebaTestimonial(PruebaTestimonial prueba)
        {
            return new PruebaDto
            {
                Id = prueba.Id,
                Descripcion = prueba.Descripcion,
                FechaPresentacion = prueba.FechaPresentacion,
                TipoPrueba = "Testimonial",
                Estado = prueba.EsValida ? "Válida" : "Inválida",
                EsValida = prueba.EsValida,
                NombreTestigo = $"Testigo ID: {prueba.TestigoId}"
            };
        }        public static PruebaDto FromPruebaPericial(PruebaPericial prueba)
        {
            return new PruebaDto
            {
                Id = prueba.Id,
                Descripcion = prueba.Descripcion,
                FechaPresentacion = prueba.FechaPresentacion,
                TipoPrueba = "Pericial",
                Estado = prueba.EsValida ? "Válida" : "Inválida",
                EsValida = prueba.EsValida,
                TipoPericia = prueba.Especialidad,
                NombrePerito = prueba.ExpertoCertificado
            };
        }
    }
}
