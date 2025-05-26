using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Pruebas;

public class Pregunta
{
    public Guid Id { get; private set; }
    public string Texto { get; private set; }
    public EtapaDeJuicio.Domain.Entities.Pruebas.Respuesta? Respuesta { get; private set; }
    public DateTime FechaHora { get; private set; }

    private Pregunta(Guid id, string texto)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID de la pregunta no puede estar vacío");
            
        if (string.IsNullOrWhiteSpace(texto))
            throw new DomainException("El texto de la pregunta es obligatorio");

        Id = id;
        Texto = texto;
        FechaHora = DateTime.UtcNow;
        Respuesta = null;
    }

    public static Pregunta Crear(Guid id, string texto)
    {
        return new Pregunta(id, texto);
    }    public void Responder(string textoRespuesta)
    {
        if (string.IsNullOrWhiteSpace(textoRespuesta))
            throw new DomainException("La respuesta no puede estar vacía");

        Respuesta = new EtapaDeJuicio.Domain.Entities.Pruebas.Respuesta(textoRespuesta, DateTime.UtcNow);
    }
}