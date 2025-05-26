using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Sentencias;

public enum TipoSentencia
{
    Definitiva,
    Interlocutoria,
    Absolutoria,
    Condenatoria
}

public class Resolutivo
{
    public Guid Id { get; private set; }
    public string Numeracion { get; private set; }
    public string Contenido { get; private set; }
    public int Orden { get; private set; }

    private Resolutivo(string numeracion, string contenido, int orden)
    {
        if (string.IsNullOrWhiteSpace(numeracion))
            throw new DomainException("La numeración del resolutivo es obligatoria");

        if (string.IsNullOrWhiteSpace(contenido))
            throw new DomainException("El contenido del resolutivo es obligatorio");

        if (orden <= 0)
            throw new DomainException("El orden del resolutivo debe ser mayor a cero");

        Id = Guid.NewGuid();
        Numeracion = numeracion;
        Contenido = contenido;
        Orden = orden;
    }

    public static Resolutivo Crear(string numeracion, string contenido, int orden)
    {
        return new Resolutivo(numeracion, contenido, orden);
    }
}

public class Sentencia
{
    private readonly List<Resolutivo> _resolutivos = new();

    public Guid Id { get; private set; }
    public Guid JuezId { get; private set; }
    public string Descripcion { get; private set; }
    public TipoSentencia Tipo { get; private set; }
    public DateTime FechaEmision { get; private set; }
    public IReadOnlyList<Resolutivo> Resolutivos => _resolutivos.AsReadOnly();

    private Sentencia(Guid id, Guid juezId, string descripcion, TipoSentencia tipo)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID de la sentencia no puede estar vacío");

        if (juezId == Guid.Empty)
            throw new DomainException("El ID del juez es obligatorio");

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DomainException("La descripción de la sentencia es obligatoria");

        Id = id;
        JuezId = juezId;
        Descripcion = descripcion;
        Tipo = tipo;
        FechaEmision = DateTime.UtcNow;
    }

    public static Sentencia Crear(Guid id, Guid juezId, string descripcion, TipoSentencia tipo)
    {
        return new Sentencia(id, juezId, descripcion, tipo);
    }

    public void AgregarResolutivo(string contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido))
            throw new DomainException("El contenido del resolutivo no puede estar vacío");

        var orden = _resolutivos.Count + 1;
        var numeraciones = new[] { "PRIMERO", "SEGUNDO", "TERCERO", "CUARTO", "QUINTO", "SEXTO", "SÉPTIMO", "OCTAVO", "NOVENO", "DÉCIMO" };
        var numeracion = orden <= numeraciones.Length ? numeraciones[orden - 1] : $"ARTÍCULO {orden}";

        var resolutivo = Resolutivo.Crear(numeracion, contenido, orden);
        _resolutivos.Add(resolutivo);
    }
}
