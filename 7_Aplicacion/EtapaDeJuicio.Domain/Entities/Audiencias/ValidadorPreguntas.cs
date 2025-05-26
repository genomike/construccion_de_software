using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Audiencias;

public static class ValidadorPreguntas
{
    private static readonly string[] PalabrasProhibidasSugestivas = 
    {
        "¿no es cierto que", "¿no cree que", "¿no piensa que", "¿verdad que"
    };    public static bool EsValida(string textoPregunta, TipoInterrogatorio tipo)
    {
        if (string.IsNullOrWhiteSpace(textoPregunta))
            return false;

        var preguntaLower = textoPregunta.ToLowerInvariant();

        // Las preguntas sugestivas no están permitidas en ningún tipo de interrogatorio en audiencias
        if (PalabrasProhibidasSugestivas.Any(palabra => preguntaLower.Contains(palabra)))
            return false;

        return true;
    }

    public static bool EsPreguntaValida(string texto, TipoPregunta tipo)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return false;

        var textoLower = texto.ToLowerInvariant();

        return tipo switch
        {
            TipoPregunta.Sugestiva => false, // Las preguntas sugestivas no son válidas en general
            TipoPregunta.Capciosa => false,  // Las preguntas capciosas tampoco
            TipoPregunta.Compuesta => !textoLower.Contains(" y ") && !textoLower.Contains(" o "), // Evitar preguntas múltiples
            _ => true
        };
    }

    public static bool ValidarPregunta(string texto, TipoPregunta tipo)
    {
        return EsPreguntaValida(texto, tipo);
    }
}
