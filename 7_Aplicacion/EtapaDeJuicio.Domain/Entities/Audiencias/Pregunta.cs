using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Audiencias;

public enum TipoPregunta
{
    Abierta,
    Cerrada,
    Sugestiva,
    Capciosa,
    Compuesta
}

public class Pregunta
{
    public Guid Id { get; private set; }
    public string Texto { get; private set; }
    public TipoPregunta Tipo { get; private set; }
    public string? Respuesta { get; private set; }
    public DateTime FechaHora { get; private set; }
    public bool EsValida { get; private set; }
    public bool FueRespondida => !string.IsNullOrWhiteSpace(Respuesta);

    private Pregunta(Guid id, string texto, TipoPregunta tipo)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID de la pregunta no puede estar vacío");
            
        if (string.IsNullOrWhiteSpace(texto))
            throw new DomainException("El texto de la pregunta es obligatorio");

        Id = id;
        Texto = texto;
        Tipo = tipo;
        FechaHora = DateTime.UtcNow;
        EsValida = ValidarTipoPregunta(texto, tipo);
    }

    public static Pregunta Crear(string texto, TipoPregunta tipo)
    {
        return new Pregunta(Guid.NewGuid(), texto, tipo);
    }

    public static Pregunta Crear(Guid id, string texto, TipoPregunta tipo)
    {
        return new Pregunta(id, texto, tipo);
    }

    public void Responder(string respuesta)
    {
        if (string.IsNullOrWhiteSpace(respuesta))
            throw new DomainException("La respuesta no puede estar vacía");

        if (!EsValida)
            throw new DomainException("No se puede responder una pregunta inválida");

        Respuesta = respuesta;
    }

    private static bool ValidarTipoPregunta(string texto, TipoPregunta tipo)
    {
        return tipo switch
        {
            TipoPregunta.Sugestiva => DetectarPreguntaSugestiva(texto),
            TipoPregunta.Capciosa => DetectarPreguntaCapciosa(texto),
            TipoPregunta.Compuesta => DetectarPreguntaCompuesta(texto),
            _ => true
        };
    }

    private static bool DetectarPreguntaSugestiva(string texto)
    {
        // Una pregunta sugestiva sugiere la respuesta
        var indicadores = new[] { "¿no es cierto que", "¿es verdad que", "¿no cree que" };
        return indicadores.Any(indicador => texto.ToLowerInvariant().Contains(indicador));
    }

    private static bool DetectarPreguntaCapciosa(string texto)
    {
        // Una pregunta capciosa es engañosa
        var indicadores = new[] { "¿cuándo dejó de", "¿por qué siempre", "¿admite que" };
        return indicadores.Any(indicador => texto.ToLowerInvariant().Contains(indicador));
    }

    private static bool DetectarPreguntaCompuesta(string texto)
    {
        // Una pregunta compuesta tiene múltiples partes
        return texto.Contains(" y ") || texto.Split('¿').Length > 2;
    }
}
