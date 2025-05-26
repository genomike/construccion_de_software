using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Audiencias;

public enum TipoInterrogatorio
{
    Directo,
    Contrainterrogatorio,
    Redirecto,
    Recontrainterrogatorio
}

public class Interrogatorio
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; }
    public DateTime FechaHora { get; private set; }
    public TipoInterrogatorio Tipo { get; private set; }
    public Guid TestigoId { get; private set; }
    public List<Pregunta> Preguntas { get; private set; }
    public bool EstaFinalizado { get; private set; }

    private Interrogatorio(Guid id, string titulo, DateTime fechaHora, TipoInterrogatorio tipo, Guid testigoId)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID del interrogatorio no puede estar vacío");
            
        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("El título del interrogatorio es obligatorio");
            
        if (testigoId == Guid.Empty)
            throw new DomainException("El ID del testigo es obligatorio");

        Id = id;
        Titulo = titulo;
        FechaHora = fechaHora;
        Tipo = tipo;
        TestigoId = testigoId;
        Preguntas = new List<Pregunta>();
        EstaFinalizado = false;
    }

    public static Interrogatorio Crear(Guid id, string titulo, DateTime fechaHora, TipoInterrogatorio tipo)
    {
        return new Interrogatorio(id, titulo, fechaHora, tipo, Guid.NewGuid());
    }

    public static Interrogatorio Crear(Guid id, string titulo, DateTime fechaHora, TipoInterrogatorio tipo, Guid testigoId)
    {
        return new Interrogatorio(id, titulo, fechaHora, tipo, testigoId);
    }

    public void AgregarPregunta(string textoPregunta)
    {
        if (EstaFinalizado)
            throw new DomainException("No se pueden agregar preguntas a un interrogatorio finalizado");
            
        if (!ValidadorPreguntas.EsValida(textoPregunta, Tipo))
            throw new DomainException($"La pregunta '{textoPregunta}' es sugestiva o improcedente para este tipo de interrogatorio");

        var pregunta = Pregunta.Crear(Guid.NewGuid(), textoPregunta, TipoPregunta.Abierta);
        Preguntas.Add(pregunta);
    }    public void Finalizar()
    {
        EstaFinalizado = true;
    }
}
