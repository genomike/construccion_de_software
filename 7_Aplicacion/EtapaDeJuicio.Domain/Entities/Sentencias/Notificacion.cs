using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Sentencias;

public enum TipoParteProcesalEnum
{
    Demandante,
    Demandado,
    Ministerio_Publico,
    Tercero_Interviniente
}

public class Notificacion
{
    public Guid Id { get; private set; }
    public Guid SentenciaId { get; private set; }
    public Guid ParteProcesal { get; private set; }
    public TipoParteProcesalEnum TipoParte { get; private set; }
    public string Contenido { get; private set; }
    public DateTime FechaNotificacion { get; private set; }
    public bool Entregada { get; private set; }

    private Notificacion(Guid id, Guid sentenciaId, Guid parteProcesal, TipoParteProcesalEnum tipoParte, string contenido, DateTime fechaNotificacion)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID de la notificación no puede estar vacío");

        if (sentenciaId == Guid.Empty)
            throw new DomainException("El ID de la sentencia es obligatorio");

        if (parteProcesal == Guid.Empty)
            throw new DomainException("El ID de la parte procesal es obligatorio");

        if (string.IsNullOrWhiteSpace(contenido))
            throw new DomainException("El contenido de la notificación es obligatorio");

        Id = id;
        SentenciaId = sentenciaId;
        ParteProcesal = parteProcesal;
        TipoParte = tipoParte;
        Contenido = contenido;
        FechaNotificacion = fechaNotificacion;
        Entregada = false;
    }

    public static Notificacion Crear(Guid id, Guid sentenciaId, string contenido, DateTime fechaNotificacion)
    {
        return new Notificacion(id, sentenciaId, Guid.NewGuid(), TipoParteProcesalEnum.Demandante, contenido, fechaNotificacion);
    }

    public static Notificacion Crear(Guid id, Guid sentenciaId, Guid parteProcesal, TipoParteProcesalEnum tipoParte, string contenido, DateTime fechaNotificacion)
    {
        return new Notificacion(id, sentenciaId, parteProcesal, tipoParte, contenido, fechaNotificacion);
    }

    public void MarcarComoEntregada()
    {
        Entregada = true;
    }
}
