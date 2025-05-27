using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Pruebas;

public class Interrogatorio
{
    public Guid Id { get; private set; }
    public string Descripcion { get; private set; }
    public DateTime FechaHora { get; private set; }
    public TipoInterrogatorio Tipo { get; private set; }
    public List<Pregunta> Preguntas { get; private set; }
    public bool EstaCompleto { get; private set; }
    public DateTime? FechaFinalizacion { get; private set; }

    private Interrogatorio(Guid id, string descripcion, DateTime fechaHora, TipoInterrogatorio tipo)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID del interrogatorio no puede estar vacío");
            
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DomainException("La descripción del interrogatorio es obligatoria");

        Id = id;
        Descripcion = descripcion;
        FechaHora = fechaHora;
        Tipo = tipo;
        Preguntas = new List<Pregunta>();
        EstaCompleto = false;
        FechaFinalizacion = null;
    }

    public static Interrogatorio Crear(Guid id, string descripcion, DateTime fechaHora, TipoInterrogatorio tipo)
    {
        return new Interrogatorio(id, descripcion, fechaHora, tipo);
    }    public void AgregarPregunta(string textoPregunta)
    {
        if (EstaCompleto)
            throw new DomainException("No se pueden agregar preguntas a un interrogatorio completo");
            
        if (string.IsNullOrWhiteSpace(textoPregunta))
            throw new DomainException("El texto de la pregunta no puede estar vacío");

        if (!ValidadorPreguntas.EsValida(textoPregunta, Tipo))
        {
            var motivo = ValidadorPreguntas.ObtenerMotivoRechazo(textoPregunta, Tipo);
            throw new DomainException(motivo);
        }

        var pregunta = Pregunta.Crear(Guid.NewGuid(), textoPregunta);
        Preguntas.Add(pregunta);
    }

    public void ResponderPregunta(int indice, string respuesta)
    {
        if (indice < 0 || indice >= Preguntas.Count)
            throw new DomainException("El índice de la pregunta no es válido");

        if (string.IsNullOrWhiteSpace(respuesta))
            throw new DomainException("La respuesta no puede estar vacía");

        var pregunta = Preguntas[indice];
        pregunta.Responder(respuesta);
    }

    public void MarcarComoCompleto()
    {
        if (!Preguntas.Any())
            throw new DomainException("El interrogatorio debe tener al menos una pregunta para ser completado");

        var preguntasSinResponder = ObtenerPreguntasSinResponder();

        if (preguntasSinResponder.Any())
            throw new DomainException("No se puede marcar como completo un interrogatorio con preguntas sin responder");

        EstaCompleto = true;
        FechaFinalizacion = DateTime.UtcNow;
    }

    public List<Pregunta> ObtenerPreguntasSinResponder()
    {
        return Preguntas.Where(p => p.Respuesta == null).ToList();
    }

    public decimal CalcularPorcentajeCompletado()
    {
        if (!Preguntas.Any())
            return 0;

        var preguntasRespondidas = Preguntas.Count(p => p.Respuesta != null);
        return (decimal)(preguntasRespondidas * 100) / Preguntas.Count;
    }

    public TimeSpan? CalcularDuracion()
    {
        if (FechaFinalizacion.HasValue)
        {
            return FechaFinalizacion.Value - FechaHora;
        }
        return null;
    }
}