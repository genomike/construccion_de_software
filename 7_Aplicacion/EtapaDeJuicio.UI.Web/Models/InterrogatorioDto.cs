using EtapaDeJuicio.Domain.Entities.Pruebas;
using EtapaDeJuicio.Domain.ValueObjects;

namespace EtapaDeJuicio.UI.Web.Models
{
    public class InterrogatorioDto
    {
        public Guid Id { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public bool EstaCompleto { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public List<PreguntaDto> Preguntas { get; set; } = new List<PreguntaDto>();

        public InterrogatorioDto() { }

        public InterrogatorioDto(Interrogatorio interrogatorio)
        {
            Id = interrogatorio.Id;
            Descripcion = interrogatorio.Descripcion;
            FechaHora = interrogatorio.FechaHora;
            Tipo = interrogatorio.Tipo.ToString();
            EstaCompleto = interrogatorio.EstaCompleto;
            FechaFinalizacion = interrogatorio.FechaFinalizacion;
            Preguntas = interrogatorio.Preguntas?.Select(p => new PreguntaDto(p)).ToList() ?? new List<PreguntaDto>();
        }        public Interrogatorio ToEntity()
        {
            if (!Enum.TryParse<TipoInterrogatorio>(Tipo, out var tipoEnum))
            {
                tipoEnum = TipoInterrogatorio.Directo;
            }

            var interrogatorio = Interrogatorio.Crear(Id, Descripcion, FechaHora, tipoEnum);
            
            foreach (var preguntaDto in Preguntas)
            {
                interrogatorio.AgregarPregunta(preguntaDto.Texto);
                // Agregar respuesta si existe
                if (!string.IsNullOrEmpty(preguntaDto.Respuesta))
                {
                    var preguntaAgregada = interrogatorio.Preguntas.LastOrDefault();
                    preguntaAgregada?.Responder(preguntaDto.Respuesta);
                }
            }

            return interrogatorio;
        }
    }    public class PreguntaDto
    {
        public Guid Id { get; set; }
        public string Texto { get; set; } = string.Empty;
        public string? Respuesta { get; set; }
        public DateTime FechaHora { get; set; }

        public PreguntaDto() { }

        public PreguntaDto(Pregunta pregunta)
        {
            Id = pregunta.Id;
            Texto = pregunta.Texto;
            Respuesta = pregunta.Respuesta?.Texto;
            FechaHora = pregunta.FechaHora;
        }

        public Pregunta ToEntity()
        {
            var pregunta = Pregunta.Crear(Id, Texto);
            
            if (!string.IsNullOrEmpty(Respuesta))
            {
                pregunta.Responder(Respuesta);
            }

            return pregunta;
        }
    }
}
