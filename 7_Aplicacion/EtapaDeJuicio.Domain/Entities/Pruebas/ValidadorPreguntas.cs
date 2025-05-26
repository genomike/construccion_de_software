using System.Linq;

namespace EtapaDeJuicio.Domain.Entities.Pruebas;

public enum TipoInterrogatorio
{
    Directo,
    Contrainterrogatorio,
    Redirect,
    Recross
}

public static class ValidadorPreguntas
{
    private static readonly string[] PalabrasSugestivas = { 
        "¿no es cierto", "¿no cree", "¿no es verdad", "¿no le parece", 
        "¿verdad que", "¿no está", "¿no fue", "¿acaso no"
    };
    
    private static readonly string[] PalabrasCapciosas = { 
        "miente frecuentemente", "persona deshonesta", "condenado por perjurio",
        "¿no es verdad que usted mintió", "¿acaso no", "¿por qué mintió"
    };    public static bool EsValida(string pregunta, TipoInterrogatorio tipo)
    {
        if (string.IsNullOrWhiteSpace(pregunta))
            return false;

        // Validar longitud mínima y máxima
        if (pregunta.Length <= 5 || pregunta.Length > 500)
            return false;

        // Validar que sea una pregunta (debe terminar con ?)
        if (!pregunta.TrimEnd().EndsWith("?"))
            return false;

        var preguntaLower = pregunta.ToLowerInvariant();

        // Las preguntas capciosas nunca están permitidas
        if (PalabrasCapciosas.Any(palabra => preguntaLower.Contains(palabra)))
            return false;

        // Las preguntas sugestivas no están permitidas en interrogatorio directo
        if (tipo == TipoInterrogatorio.Directo)
        {
            if (PalabrasSugestivas.Any(palabra => preguntaLower.Contains(palabra)))
                return false;
        }

        return true;
    }

    public static void ValidarPregunta(string pregunta, TipoInterrogatorio tipo)
    {
        if (!EsValida(pregunta, tipo))
        {
            var motivo = ObtenerMotivoRechazo(pregunta, tipo);
            throw new EtapaDeJuicio.Domain.Exceptions.DomainException(motivo);
        }
    }    public static string? ObtenerTipoInvalidez(string pregunta, TipoInterrogatorio tipo)
    {
        if (EsValida(pregunta, tipo))
            return null;

        if (string.IsNullOrWhiteSpace(pregunta))
            return "Pregunta vacía";

        var preguntaLower = pregunta.ToLowerInvariant();

        if (PalabrasCapciosas.Any(palabra => preguntaLower.Contains(palabra)))
            return "Pregunta capciosa";

        if (tipo == TipoInterrogatorio.Directo && PalabrasSugestivas.Any(palabra => preguntaLower.Contains(palabra)))
            return "Pregunta sugestiva";

        if (pregunta.Length <= 5)
            return "Pregunta muy corta";

        if (pregunta.Length > 500)
            return "Pregunta muy larga";

        if (!pregunta.TrimEnd().EndsWith("?"))
            return "No es una pregunta";

        return "Pregunta inválida";
    }    public static string ObtenerMotivoRechazo(string pregunta, TipoInterrogatorio tipo)
    {
        if (string.IsNullOrWhiteSpace(pregunta))
            return "La pregunta no puede estar vacía";

        var preguntaLower = pregunta.ToLowerInvariant();

        if (PalabrasCapciosas.Any(palabra => preguntaLower.Contains(palabra)))
            return "La pregunta es capciosa y no está permitida";

        if (tipo == TipoInterrogatorio.Directo && PalabrasSugestivas.Any(palabra => preguntaLower.Contains(palabra)))
            return "La pregunta es sugestiva y no está permitida en interrogatorio directo";

        if (pregunta.Length <= 5)
            return "La pregunta es demasiado corta";

        if (pregunta.Length > 500)
            return "La pregunta es demasiado larga";

        if (!pregunta.TrimEnd().EndsWith("?"))
            return "El texto debe ser una pregunta (terminar con ?)";

        return "La pregunta no es válida";
    }
}
