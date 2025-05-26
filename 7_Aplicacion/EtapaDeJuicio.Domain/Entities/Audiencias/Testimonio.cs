using EtapaDeJuicio.Domain.Exceptions;

namespace EtapaDeJuicio.Domain.Entities.Audiencias;

public class Testimonio
{
    public Guid Id { get; private set; }
    public Guid TestigoId { get; private set; }
    public string Descripcion { get; private set; }
    public DateTime FechaHora { get; private set; }
    public string ContenidoTestimonio { get; private set; }
    public bool EstaCompleto { get; private set; }

    private Testimonio(Guid id, Guid testigoId, string descripcion, DateTime fechaHora)
    {
        if (id == Guid.Empty)
            throw new DomainException("El ID del testimonio no puede estar vacío");
            
        if (testigoId == Guid.Empty)
            throw new DomainException("El ID del testigo es obligatorio");
            
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new DomainException("La descripción del testimonio es obligatoria");

        Id = id;
        TestigoId = testigoId;
        Descripcion = descripcion;
        FechaHora = fechaHora;
        ContenidoTestimonio = string.Empty;
        EstaCompleto = false;
    }

    public static Testimonio Crear(Guid id, Guid testigoId, string descripcion, DateTime fechaHora)
    {
        return new Testimonio(id, testigoId, descripcion, fechaHora);
    }

    public void AgregarContenido(string contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido))
            throw new DomainException("El contenido del testimonio no puede estar vacío");
            
        ContenidoTestimonio = contenido;
        EstaCompleto = true;
    }
}
