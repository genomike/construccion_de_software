using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Pruebas;

public class Respuesta
{
    public string Texto { get; private set; }
    public DateTime FechaHora { get; private set; }

    public Respuesta(string texto, DateTime fechaHora)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new DomainException("El texto de la respuesta no puede estar vacío");

        Texto = texto;
        FechaHora = fechaHora;
    }
}
