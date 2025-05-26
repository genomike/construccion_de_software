using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Pruebas;

public class Testimonio
{
    public Guid Id { get; private set; }
    public Guid TestigoId { get; private set; }
    public string Descripcion { get; private set; }
    public DateTime FechaHora { get; private set; }
    public List<Guid> InterrogatoriosIds { get; private set; } = new();
    
    // Nuevas propiedades requeridas por los tests
    public IReadOnlyList<string> Declaraciones => _declaraciones.AsReadOnly();
    public bool EstaCompleto { get; private set; }
    public string? ContenidoTestimonio { get; private set; }
    
    private readonly List<string> _declaraciones = new();

    private Testimonio(Guid id, Guid testigoId, string descripcion, DateTime fechaHora)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID del testimonio no puede estar vacío");

        if (testigoId == Guid.Empty)
            throw new DomainException("El ID del testigo es obligatorio");

        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DomainException("La descripción del testimonio es obligatoria");        Id = id;
        TestigoId = testigoId;
        Descripcion = descripcion;
        FechaHora = fechaHora;
        EstaCompleto = false;
        ContenidoTestimonio = null;
    }

    public static Testimonio Crear(Guid testigoId, string descripcion, DateTime fechaHora)
    {
        return new Testimonio(Guid.NewGuid(), testigoId, descripcion, fechaHora);
    }

    public static Testimonio Crear(Guid id, Guid testigoId, string descripcion, DateTime fechaHora)
    {
        return new Testimonio(id, testigoId, descripcion, fechaHora);
    }    public void AgregarInterrogatorio(Guid interrogatorioId)
    {
        if (interrogatorioId == Guid.Empty)
            throw new DomainException("El ID del interrogatorio no puede ser vacío");

        if (InterrogatoriosIds.Contains(interrogatorioId))
            throw new DomainException("El interrogatorio ya fue agregado");

        InterrogatoriosIds.Add(interrogatorioId);
    }

    public void AgregarDeclaracion(string declaracion)
    {
        if (string.IsNullOrWhiteSpace(declaracion))
            throw new DomainException("La declaración no puede estar vacía");

        _declaraciones.Add(declaracion);
    }

    public void AgregarContenido(string contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido))
            throw new DomainException("El contenido del testimonio no puede estar vacío");

        ContenidoTestimonio = contenido;
        EstaCompleto = true;
    }
}
